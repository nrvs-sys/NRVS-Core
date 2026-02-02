#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

public class ServerTextureFixAssetProcessor //: AssetPostprocessor
{
    // Some Unity versions used "DedicatedServer" in metas; we'll treat either as "present".
    private static readonly string[] ServerNames = { "Server", "DedicatedServer" };
    private const string DefaultTargetName = "DefaultTexturePlatform";

    //void OnPostprocessTexture(Texture2D texture)
    //{
    //    // Run after import, ownerless so the coroutine survives even if this instance is GC'd.
    //    EditorCoroutineUtility.StartCoroutineOwnerless(FixForAsset(assetPath));
    //}

    [MenuItem("Utilities/Apply Server Texture Meta Fix")]
    private static void RepairAllTextureMetas()
    {
        try
        {
            AssetDatabase.StartAssetEditing();
            int changed = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(path);
                if (string.IsNullOrEmpty(metaPath) || !File.Exists(metaPath))
                    continue;

                string original = ReadAllTextUnlocked(metaPath);
                if (string.IsNullOrEmpty(original))
                    continue;

                if (HasAnyServerBlock(original))
                    continue;

                var updated = AddServerBlock(original);
                if (updated != null && !ReferenceEquals(updated, original) && updated != original)
                {
                    MakeEditable(metaPath);
                    WriteAllTextUnlocked(metaPath, updated);
                    changed++;
                }
            }
            if (changed > 0)
                AssetDatabase.Refresh();
            Debug.Log($"[ServerTextureFix] Repaired {changed} texture meta file(s).");
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    #region Asset Processing

    private static IEnumerator FixForAsset(string assetPath)
    {
        // Resolve meta path reliably
        string metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);
        if (string.IsNullOrEmpty(metaPath))
            yield break;

        // Wait until the meta exists and has platformSettings (avoid racing the importer write)
        const int maxFrames = 600; // ~10 seconds worst case
        int frames = 0;
        while (frames++ < maxFrames)
        {
            if (File.Exists(metaPath))
            {
                string txt = SafeRead(metaPath);
                if (!string.IsNullOrEmpty(txt) && txt.Contains("platformSettings:"))
                {
                    if (!HasAnyServerBlock(txt))
                    {
                        var updated = AddServerBlock(txt);
                        if (updated != null && updated != txt)
                        {
                            MakeEditable(metaPath);
                            WriteAllTextUnlocked(metaPath, updated);
                            // Reimport the asset to pick up the new platform block
                            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                        }
                    }
                    break;
                }
            }
            yield return null;
        }
    }

    private static bool HasAnyServerBlock(string metaText)
        => ServerNames.Any(n => metaText.Contains($"buildTarget: {n}"));

    private static string SafeRead(string path)
    {
        // File may be locked for a moment by the importer; retry a few frames.
        for (int i = 0; i < 8; i++)
        {
            try { return ReadAllTextUnlocked(path); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
        return null;
    }

    private static string AddServerBlock(string metaText)
    {
        // Normalize line endings and split
        bool useCRLF = metaText.Contains("\r\n");
        string[] lines = metaText.Replace("\r\n", "\n").Split('\n');

        int platformSettingsIdx = IndexOfLine(lines, l => l.TrimStart().StartsWith("platformSettings:"));
        if (platformSettingsIdx < 0)
            return metaText; // nothing to do

        int sectionIndent = LeadingSpaces(lines[platformSettingsIdx]);

        // Scan list items under platformSettings
        int defaultStart = -1, defaultEnd = -1, itemIndent = -1, listEnd = lines.Length;

        int i = platformSettingsIdx + 1;
        while (i < lines.Length)
        {
            string t = lines[i].TrimStart();
            int indent = LeadingSpaces(lines[i]);

            // End of this YAML list if we hit another top-level key
            if (!string.IsNullOrWhiteSpace(t) && indent <= sectionIndent && !t.StartsWith("- "))
            {
                listEnd = i;
                break;
            }

            if (t.StartsWith("- serializedVersion:"))
            {
                int itemStart = i;
                itemIndent = indent;
                int j = i + 1;
                bool isDefault = false;

                // Walk until next item or section end
                for (; j < lines.Length; j++)
                {
                    string tj = lines[j].TrimStart();
                    int indj = LeadingSpaces(lines[j]);

                    bool nextItem = (indj == itemIndent && tj.StartsWith("- serializedVersion:"));
                    bool outOfSection = (!string.IsNullOrWhiteSpace(tj) && indj <= sectionIndent && !tj.StartsWith("- "));

                    if (tj.StartsWith($"buildTarget: {DefaultTargetName}"))
                        isDefault = true;

                    if (nextItem || outOfSection)
                        break;
                }

                if (isDefault && defaultStart == -1)
                {
                    defaultStart = itemStart;
                    defaultEnd = j;
                }

                i = j;
                continue;
            }

            i++;
        }

        // If Default not found, fallback to first item in the list
        if (defaultStart < 0)
        {
            int firstStart = -1, firstEnd = -1, firstIndent = -1;
            i = platformSettingsIdx + 1;
            while (i < lines.Length)
            {
                string t = lines[i].TrimStart();
                int indent = LeadingSpaces(lines[i]);

                if (!string.IsNullOrWhiteSpace(t) && indent <= sectionIndent && !t.StartsWith("- "))
                    break;

                if (t.StartsWith("- serializedVersion:"))
                {
                    firstStart = i;
                    firstIndent = indent;
                    int j = i + 1;
                    for (; j < lines.Length; j++)
                    {
                        string tj = lines[j].TrimStart();
                        int indj = LeadingSpaces(lines[j]);
                        bool nextItem = (indj == firstIndent && tj.StartsWith("- serializedVersion:"));
                        bool outOfSection = (!string.IsNullOrWhiteSpace(tj) && indj <= sectionIndent && !tj.StartsWith("- "));
                        if (nextItem || outOfSection) break;
                    }
                    firstEnd = j;
                    break;
                }
                i++;
            }
            if (firstStart < 0)
                return metaText; // can't safely patch
            defaultStart = firstStart;
            defaultEnd = firstEnd;
        }

        // Duplicate the block and rewrite the buildTarget line
        var block = lines.Skip(defaultStart).Take(defaultEnd - defaultStart).Select(s =>
        {
            string ts = s.TrimStart();
            if (ts.StartsWith("buildTarget: "))
            {
                string prefix = s.Substring(0, s.Length - ts.Length);
                return prefix + "buildTarget: Server";
            }
            return s;
        }).ToArray();

        // Insert just after the default block
        int insertIndex = defaultEnd;
        var newLines = new List<string>(lines.Length + block.Length + 1);
        newLines.AddRange(lines.Take(insertIndex));
        newLines.AddRange(block);
        newLines.AddRange(lines.Skip(insertIndex));

        string nl = useCRLF ? "\r\n" : "\n";
        return string.Join(nl, newLines);
    }

    #endregion

    #region IO helpers

    private static string ReadAllTextUnlocked(string path)
    {
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var sr = new StreamReader(fs, Encoding.UTF8, true))
            return sr.ReadToEnd();
    }

    private static void WriteAllTextUnlocked(string path, string text)
    {
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
        using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            sw.Write(text);
    }

    private static void MakeEditable(string path)
    {
        try
        {
            if (UnityEditor.VersionControl.Provider.enabled && UnityEditor.VersionControl.Provider.isActive)
            {
                var task = UnityEditor.VersionControl.Provider.Checkout(new UnityEditor.VersionControl.AssetList() { new UnityEditor.VersionControl.Asset(path) }, UnityEditor.VersionControl.CheckoutMode.Both);
                task.Wait();
            }
            // Clear read-only if present (e.g., Perforce/readonly metas)
            var attrs = File.GetAttributes(path);
            if ((attrs & FileAttributes.ReadOnly) != 0)
                File.SetAttributes(path, attrs & ~FileAttributes.ReadOnly);
        }
        catch { /* non-fatal */ }
    }

    #endregion

    #region Parse helpers

    private static int LeadingSpaces(string s)
    {
        int i = 0;
        while (i < s.Length && s[i] == ' ') i++;
        return i;
    }

    private static int IndexOfLine(string[] lines, Func<string, bool> pred)
    {
        for (int i = 0; i < lines.Length; i++)
            if (pred(lines[i])) return i;
        return -1;
    }

    #endregion
}
#endif
