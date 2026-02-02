// Assets/Editor/SelectMaterialsByShader.cs
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class SelectMaterialsByShader : EditorWindow
{
    [SerializeField] private Shader shaderRef;
    [SerializeField] private string shaderNameContains = "";
    [SerializeField] private DefaultAsset searchFolder; // optional folder to scope search
    [SerializeField] private string materialNameContains = ""; // NEW: optional material-name filter
    [SerializeField] private bool caseSensitive = false;
    [SerializeField] private bool focusProjectWindow = true;
    [SerializeField] private bool pingFirstFound = false; // optional
    [SerializeField] private bool includeSubAssets = false; // off => excludes SG preview/importer + FBX-embedded mats


    // property names in your unified shader (adjust if needed)
    const string PROP_NORMAL_TEX = "_Normal";
    const string PROP_MS_TEX = "_Smoothness_and_Metallic";
    const string PROP_EMISSIVE_SCROLL_T = "_Emissive_Scroll_Texture";



    [MenuItem("Tools/Select Materials Using Shader…")]
    private static void Open() => GetWindow<SelectMaterialsByShader>("Select Materials by Shader");

    void OnGUI()
    {
        GUILayout.Label("Find materials by shader", EditorStyles.boldLabel);
        shaderRef = (Shader)EditorGUILayout.ObjectField("Shader (exact)", shaderRef, typeof(Shader), false);
        shaderNameContains = EditorGUILayout.TextField(new GUIContent("Shader name contains"), shaderNameContains);
        caseSensitive = EditorGUILayout.ToggleLeft("Case sensitive (name search)", caseSensitive);

        EditorGUILayout.Space();
        includeSubAssets = EditorGUILayout.ToggleLeft("Include sub-asset materials (.shadergraph, .fbx)", includeSubAssets);
        focusProjectWindow = EditorGUILayout.ToggleLeft("Focus Project window after select", focusProjectWindow);
        pingFirstFound = EditorGUILayout.ToggleLeft("Ping first result (optional)", pingFirstFound);

        // NEW: optional folder scope
        searchFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            new GUIContent("Search Folder (optional)", "Drag a folder to limit the search. Leave empty to search entire project."),
            searchFolder, typeof(DefaultAsset), false);

        // NEW: material-name filter
        materialNameContains = EditorGUILayout.TextField(
            new GUIContent("Material name contains (optional)", "Filters by material asset name. Empty = no filter."),
            materialNameContains);

        EditorGUILayout.Space();
        if (GUILayout.Button("Find & Select"))
        {
            if (shaderRef == null && string.IsNullOrEmpty(shaderNameContains))
            {
                EditorUtility.DisplayDialog("Need input", "Assign a Shader or enter a shader name substring.", "OK");
                return;
            }

            string path = null;
            if (searchFolder != null)
            {
                path = AssetDatabase.GetAssetPath(searchFolder);
                if (!AssetDatabase.IsValidFolder(path)) path = null; // ignore non-folders
            }

            FindAndSelect(shaderRef, shaderNameContains, caseSensitive, focusProjectWindow, pingFirstFound, includeSubAssets, path, materialNameContains);
        }

        EditorGUILayout.Space(10);
        GUILayout.Label("Convert selected materials", EditorStyles.boldLabel);
        convertToShader = (Shader)EditorGUILayout.ObjectField("Convert to Shader", convertToShader, typeof(Shader), false);

        using (new EditorGUI.DisabledScope(convertToShader == null))
        {
            if (GUILayout.Button("Convert Selected Materials"))
                ConvertSelectedMaterials(convertToShader);
        }

        EditorGUILayout.Space(10);
        GUILayout.Label("Feature keywords (set on selected materials)", EditorStyles.boldLabel);
        if (GUILayout.Button("Set Keywords on Selected (.mat)"))
            SetKeywordsOnSelected();
    }

    [MenuItem("Tools/Select Materials Using Shader/From Selection")]
    private static void FromSelection()
    {
        Shader s = null;
        string searchPath = null;

        var active = Selection.activeObject;

        if (active is Shader selShader) s = selShader;
        else if (active is Material mat && mat.shader != null) s = mat.shader;

        // if a folder is selected, use it as search scope
        if (active != null)
        {
            var maybePath = AssetDatabase.GetAssetPath(active);
            if (AssetDatabase.IsValidFolder(maybePath))
                searchPath = maybePath;
        }

        if (s == null)
        {
            EditorUtility.DisplayDialog("Select Materials", "Select a Shader or a Material first.", "OK");
            return;
        }

        FindAndSelect(s, null, false, focusProjectWindow: true, pingFirstFound: false, includeSubAssets: false, searchPath: searchPath, materialNameFilter: null);
    }

    private static void FindAndSelect(Shader shaderRef, string nameContains, bool caseSensitive, bool focusProjectWindow, bool pingFirstFound, bool includeSubAssets, string searchPath, string materialNameFilter)
    {
        var searchIn = (!string.IsNullOrEmpty(searchPath) && AssetDatabase.IsValidFolder(searchPath))
            ? new[] { searchPath }
            : new[] { "Assets" };

        var guids = AssetDatabase.FindAssets("t:Material", searchIn);
        var results = new List<Object>(Mathf.Min(guids.Length, 5000));
        var sb = new StringBuilder();

        string searchDesc = shaderRef != null
            ? $"Shader: {shaderRef.name} ({AssetDatabase.GetAssetPath(shaderRef)})"
            : $"Shader name contains: \"{nameContains}\" (case {(caseSensitive ? "sensitive" : "insensitive")})";
        searchDesc += $" | Folder: {searchIn[0]}";
        if (!string.IsNullOrEmpty(materialNameFilter))
            searchDesc += $" | Mat name contains: \"{materialNameFilter}\"";

        // prep filters
        string nameFilterNorm = null;
        if (!string.IsNullOrEmpty(materialNameFilter))
            nameFilterNorm = caseSensitive ? materialNameFilter : materialNameFilter.ToLowerInvariant();

        int skippedSubAssets = 0;

        try
        {
            EditorUtility.DisplayProgressBar("Searching materials", "Scanning project…", 0f);

            for (int i = 0; i < guids.Length; i++)
            {
                if (i % 64 == 0)
                    EditorUtility.DisplayProgressBar("Searching materials", $"Checking {i}/{guids.Length}", i / (float)guids.Length);

                var path = AssetDatabase.GUIDToAssetPath(guids[i]);

                // exclude non-.mat containers unless explicitly included (filters out SG importer preview mats, FBX-embedded mats)
                if (!includeSubAssets && !path.EndsWith(".mat", System.StringComparison.OrdinalIgnoreCase))
                {
                    skippedSubAssets++;
                    continue;
                }

                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (!mat || !mat.shader) continue;

                bool match = false;

                // shader ref / name substring
                if (shaderRef != null)
                    match = mat.shader == shaderRef || mat.shader.name == shaderRef.name;

                if (!match && !string.IsNullOrEmpty(nameContains))
                {
                    var a = caseSensitive ? mat.shader.name : mat.shader.name.ToLowerInvariant();
                    var b = caseSensitive ? nameContains : nameContains.ToLowerInvariant();
                    match = a.Contains(b);
                }

                // NEW: material-name filter (only if previously matched shader criteria)
                if (match && !string.IsNullOrEmpty(nameFilterNorm))
                {
                    var matName = caseSensitive ? mat.name : mat.name.ToLowerInvariant();
                    if (!matName.Contains(nameFilterNorm))
                        match = false;
                }

                if (match)
                {
                    results.Add(mat);
                    sb.AppendLine($"- {path}   (shader: {mat.shader.name})");
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        if (focusProjectWindow)
            EditorUtility.FocusProjectWindow();

        Selection.objects = results.ToArray();
        if (pingFirstFound && results.Count > 0)
            EditorGUIUtility.PingObject(results[0]);

        Debug.Log(
            $"SelectMaterialsByShader\n" +
            $"Search: {searchDesc}\n" +
            $"Include sub-assets: {includeSubAssets}\n" +
            $"Found: {results.Count} material(s)\n" +
            (skippedSubAssets > 0 && !includeSubAssets ? $"Skipped sub-asset materials: {skippedSubAssets}\n" : "") +
            (results.Count > 0 ? sb.ToString() : "(no matches)")
        );

        if (results.Count == 0)
            EditorUtility.DisplayDialog("No matches", "No materials matched your shader criteria.", "OK");
    }



    [SerializeField] private Shader convertToShader; // target shader for conversion

    private static void ConvertSelectedMaterials(Shader targetShader)
    {
        if (targetShader == null)
        {
            EditorUtility.DisplayDialog("No target shader", "Assign a shader to convert to.", "OK");
            return;
        }

        // collect .mat assets only
        var mats = new List<Material>();
        foreach (var obj in Selection.objects)
        {
            if (obj is Material m)
            {
                var path = AssetDatabase.GetAssetPath(m);
                if (!path.EndsWith(".mat", System.StringComparison.OrdinalIgnoreCase))
                    continue; // skip sub-asset/preview materials
                mats.Add(m);
            }
        }

        if (mats.Count == 0)
        {
            EditorUtility.DisplayDialog("No materials", "Select one or more .mat assets to convert.", "OK");
            return;
        }

        // URP/Lit sources
        const string SRC_BaseMap = "_BaseMap";
        const string SRC_BaseColor = "_BaseColor";
        const string SRC_BumpMap = "_BumpMap";
        const string SRC_MetallicGlossMap = "_MetallicGlossMap";
        const string SRC_Metallic = "_Metallic";
        const string SRC_Smoothness = "_Smoothness";
        const string SRC_EmissionMap = "_EmissionMap";
        const string SRC_EmissionColor = "_EmissionColor";

        // target SG destinations
        const string DST_MainTex = "_Main_Texture";
        const string DST_MainTint = "_Main_Texture_Tint";
        const string DST_Normal = "_Normal";
        const string DST_MS_PackedTex = "_Smoothness_and_Metallic";
        const string DST_Metalness = "_Metalness";
        const string DST_Smoothness = "_Smoothness";
        const string DST_EmissiveTex = "_Emissive_Texture";
        const string DST_EmissiveCol = "_Emissive_Color";
        const string DST_EmScrollTex = "_Emissive_Scroll_Texture";
        const string DST_EmScrollSpd = "_Emissive_Scroll_Speed";
        const string DST_Alpha = "_Alpha";

        var log = new StringBuilder();
        log.AppendLine($"Converting {mats.Count} material(s) → {targetShader.name}");

        try
        {
            for (int i = 0; i < mats.Count; i++)
            {
                var src = mats[i];
                EditorUtility.DisplayProgressBar("Converting materials",
                    $"{i + 1}/{mats.Count}: {src.name}", (i + 1f) / mats.Count);

                Undo.RecordObject(src, "Convert Material Shader");

                // --- capture BEFORE swap ---
                Texture baseMap = GetTexSafe(src, SRC_BaseMap);
                if (!baseMap) baseMap = GetTexSafe(src, "_MainTex");
                var basePropForST = baseMap ? (src.HasProperty(SRC_BaseMap) ? SRC_BaseMap : "_MainTex") : SRC_BaseMap;
                var baseScale = GetScaleSafe(src, basePropForST);
                var baseOffset = GetOffsetSafe(src, basePropForST);
                var baseColor = GetColorSafe(src, SRC_BaseColor, Color.white);

                Texture bumpMap = GetTexSafe(src, SRC_BumpMap);
                var bumpScale = GetScaleSafe(src, SRC_BumpMap);
                var bumpOffset = GetOffsetSafe(src, SRC_BumpMap);

                Texture msTex = GetTexSafe(src, SRC_MetallicGlossMap);
                var msScale = GetScaleSafe(src, SRC_MetallicGlossMap);
                var msOffset = GetOffsetSafe(src, SRC_MetallicGlossMap);
                float metallic = GetFloatSafe(src, SRC_Metallic, 0f);
                float smoothness = GetFloatSafe(src, SRC_Smoothness, 0.5f);

                Texture emisTex = GetTexSafe(src, SRC_EmissionMap);
                var emisScale = GetScaleSafe(src, SRC_EmissionMap);
                var emisOffset = GetOffsetSafe(src, SRC_EmissionMap);
                var emisColor = GetColorSafe(src, SRC_EmissionColor, Color.black);

                // decide emissive mode from URP inputs
                bool hasEmissiveStatic = (emisTex != null) || (emisColor.maxColorComponent > 0.001f);
                int emissiveMode = hasEmissiveStatic ? 1 : 0; // 0=Off, 1=Static, 2=Scrolling

                // --- swap shader ---
                src.shader = targetShader;

                // --- write to SG material ---
                SetTexSTSafe(src, DST_MainTex, baseMap, baseScale, baseOffset);
                SetColorSafe(src, DST_MainTint, baseColor);

                SetTexSTSafe(src, DST_Normal, bumpMap, bumpScale, bumpOffset);

                SetTexSTSafe(src, DST_MS_PackedTex, msTex, msScale, msOffset);
                SetFloatSafe(src, DST_Metalness, metallic);
                SetFloatSafe(src, DST_Smoothness, smoothness);

                SetTexSTSafe(src, DST_EmissiveTex, emisTex, emisScale, emisOffset);
                SetColorSafe(src, DST_EmissiveCol, emisColor);

                // defaults for scroll path
                if (src.HasProperty(DST_EmScrollTex)) src.SetTexture(DST_EmScrollTex, null);
                if (src.HasProperty(DST_EmScrollSpd)) src.SetVector(DST_EmScrollSpd, Vector4.zero);
                if (src.HasProperty(DST_Alpha)) src.SetFloat(DST_Alpha, 1f);

                if (src.HasProperty("_Emissive_Pulse_Speed"))
				{
                    src.SetFloat("_Emissive_Pulse_Speed", 0f);
				}

                // keywords: normal/ms booleans as before
                bool hasNormal = bumpMap != null;
                bool hasMS = msTex != null;
                SetSgBoolKeyword(src, KW_USE_NORMAL_MAP, PROP_USE_NORMAL_MAP, hasNormal);
                SetSgBoolKeyword(src, KW_USE_MS_MAP, PROP_USE_MS_MAP, hasMS);

                // enum keyword for emissive mode
                SetSgEnumKeyword(src, KW_EMISSIVE_MODE, emissiveMode);
                SetEnumPropSafe(src, DST_EmissiveModeProp, emissiveMode); // keep SG inspector in sync

                EditorUtility.SetDirty(src);
                log.AppendLine($"- {AssetDatabase.GetAssetPath(src)}  | N:{hasNormal} MS:{hasMS} EmMode:{(emissiveMode == 0 ? "Off" : emissiveMode == 1 ? "Static" : "Scrolling")}");
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
        Debug.Log(log.ToString());
    }


    static Texture GetTexSafe(Material m, string prop) => m.HasProperty(prop) ? m.GetTexture(prop) : null;
    static Vector2 GetScaleSafe(Material m, string prop) => m.HasProperty(prop) ? m.GetTextureScale(prop) : Vector2.one;
    static Vector2 GetOffsetSafe(Material m, string prop) => m.HasProperty(prop) ? m.GetTextureOffset(prop) : Vector2.zero;
    static void SetTexSTSafe(Material m, string prop, Texture t, Vector2 scale, Vector2 offset)
    {
        if (!m.HasProperty(prop)) return;
        m.SetTexture(prop, t);
        m.SetTextureScale(prop, scale);
        m.SetTextureOffset(prop, offset);
    }
    static float GetFloatSafe(Material m, string prop, float fallback)
    {
        return (m != null && !string.IsNullOrEmpty(prop) && m.HasProperty(prop))
            ? m.GetFloat(prop)
            : fallback;
    }

    static Color GetColorSafe(Material m, string prop, Color fallback)
    {
        return (m != null && !string.IsNullOrEmpty(prop) && m.HasProperty(prop))
            ? m.GetColor(prop)
            : fallback;
    }
    static void SetColorSafe(Material m, string prop, Color c) { if (m.HasProperty(prop)) m.SetColor(prop, c); }
    static void SetFloatSafe(Material m, string prop, float v) { if (m.HasProperty(prop)) m.SetFloat(prop, v); }

    static void SetEnumPropSafe(Material m, string prop, int value)
    {
        if (m.HasProperty(prop)) m.SetFloat(prop, value);
    }




    // keyword candidates (Shader Graph often prefixes with _)
    static readonly string[] KW_USE_NORMAL_MAP = { "_USE_NORMAL_MAP", "USE_NORMAL_MAP" };
    static readonly string[] KW_USE_MS_MAP = { "_USE_MS_MAP", "USE_MS_MAP" };
    static readonly string[] KW_USE_EMISSIVE_SCROLL = { "_USE_EMISSIVE_SCROLL", "USE_EMISSIVE_SCROLL" };

    // set a local keyword only if the shader actually defines it
    static bool TrySetLocalKeyword(Material mat, string[] candidates, bool enabled)
    {
#if UNITY_2021_2_OR_NEWER
        foreach (var name in candidates)
        {
            var kw = mat.shader.keywordSpace.FindKeyword(name);
            if (kw.isValid)
            {
                mat.SetKeyword(kw, enabled);
                return true;
            }
        }
        return false;
#else
    // older APIs don't expose keywordSpace; silently no-op if unknown
    foreach (var name in candidates)
    {
        if (enabled) mat.EnableKeyword(name); else mat.DisableKeyword(name);
        return true;
    }
#endif
    }

    // serialized float properties created by SG for exposed boolean keywords
    static readonly string[] PROP_USE_NORMAL_MAP = { "_USE_NORMAL_MAP", "USE_NORMAL_MAP" };
    static readonly string[] PROP_USE_MS_MAP = { "_USE_MS_MAP", "USE_MS_MAP" };
    static readonly string[] PROP_USE_EMISSIVE_SCROLL = { "_USE_EMISSIVE_SCROLL", "USE_EMISSIVE_SCROLL" };

    // Enum keyword candidates for Shader Graph (Local Shader Feature - Enum)
    // Assumes an enum named EMISSIVE_MODE with entries: OFF, STATIC, SCROLLING.
    // Includes common alt spellings just in case.
    static readonly string[][] KW_EMISSIVE_MODE = new[]
    {
    new[] { "_EMISSIVE_MODE_OFF", "EMISSIVE_MODE_OFF", "_EMISSIVE_OFF", "EMISSIVE_OFF" },              // index 0
    new[] { "_EMISSIVE_MODE_STATIC", "EMISSIVE_MODE_STATIC", "_EMISSIVE_STATIC", "EMISSIVE_STATIC" },  // index 1
    new[] { "_EMISSIVE_MODE_SCROLLING", "EMISSIVE_MODE_SCROLLING", "_EMISSIVE_SCROLLING", "EMISSIVE_SCROLLING",
            "_EMISSIVE_SCROLL", "EMISSIVE_SCROLL" }                                                    // index 2
};

    // set SG boolean keyword + its backing float (so it shows in inspector and persists)
    static void SetSgBoolKeyword(Material mat, string[] kwCandidates, string[] propCandidates, bool enabled)
    {
        // set serialized float (0/1) if present
        foreach (var p in propCandidates)
        {
            if (mat.HasProperty(p)) { mat.SetFloat(p, enabled ? 1f : 0f); break; }
        }
        // set actual keyword
        TrySetLocalKeyword(mat, kwCandidates, enabled);
    }

    // Enable exactly one enum keyword (and disable the others). Returns true if any option matched.
    static bool SetSgEnumKeyword(Material mat, string[][] enumCandidates, int activeIndex)
    {
        if (mat == null || mat.shader == null || enumCandidates == null || enumCandidates.Length == 0)
            return false;

#if UNITY_2021_2_OR_NEWER
        // find the first valid LocalKeyword for each option
        var found = new System.ValueTuple<bool, LocalKeyword, string>[enumCandidates.Length]; // (has, kw, name)
        for (int i = 0; i < enumCandidates.Length; i++)
        {
            LocalKeyword chosen = default;
            string chosenName = null;
            foreach (var name in enumCandidates[i])
            {
                var kw = mat.shader.keywordSpace.FindKeyword(name);
                if (kw.isValid) { chosen = kw; chosenName = name; break; }
            }
            found[i] = (chosen.isValid, chosen, chosenName);
        }

        bool any = false;
        // disable all found options
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i].Item1)
            {
                mat.SetKeyword(found[i].Item2, false);
                any = true;
            }
        }

        // enable the active one
        if (activeIndex >= 0 && activeIndex < found.Length && found[activeIndex].Item1)
            mat.SetKeyword(found[activeIndex].Item2, true);

        return any;
#else
    // best-effort for older APIs
    bool any = false;
    for (int i = 0; i < enumCandidates.Length; i++)
    {
        string chosenName = null;
        foreach (var name in enumCandidates[i]) { chosenName = name; break; }
        if (string.IsNullOrEmpty(chosenName)) continue;
        any = true;
        if (i == activeIndex) mat.EnableKeyword(chosenName); else mat.DisableKeyword(chosenName);
    }
    return any;
#endif
    }


    const string DST_EmissiveModeProp = "_EMISSIVE_MODE";

    private static void SetKeywordsOnSelected()
    {
        var mats = new List<Material>();
        foreach (var obj in Selection.objects)
        {
            if (obj is Material m)
            {
                var path = AssetDatabase.GetAssetPath(m);
                if (!path.EndsWith(".mat", System.StringComparison.OrdinalIgnoreCase))
                    continue; // skip sub-assets/preview mats
                mats.Add(m);
            }
        }

        if (mats.Count == 0)
        {
            EditorUtility.DisplayDialog("No materials", "Select one or more .mat assets.", "OK");
            return;
        }

        // target SG props we’ll read for detection
        const string DST_Normal = "_Normal";
        const string DST_MS_PackedTex = "_Smoothness_and_Metallic";
        const string DST_EmissiveTex = "_Emissive_Texture";
        const string DST_EmissiveCol = "_Emissive_Color";
        const string DST_EmScrollTex = "_Emissive_Scroll_Texture";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Setting keywords on {mats.Count} material(s): NORMAL, MS, EMISSIVE_MODE (Off/Static/Scrolling)");

        try
        {
            for (int i = 0; i < mats.Count; i++)
            {
                var mat = mats[i];
                EditorUtility.DisplayProgressBar("Setting keywords",
                    $"{i + 1}/{mats.Count}: {mat.name}", (i + 1f) / mats.Count);

                Undo.RecordObject(mat, "Set Material Keywords");

                bool hasNormal = mat.HasProperty(DST_Normal) && mat.GetTexture(DST_Normal) != null;
                bool hasMS = mat.HasProperty(DST_MS_PackedTex) && mat.GetTexture(DST_MS_PackedTex) != null;

                bool hasEmissiveTex = mat.HasProperty(DST_EmissiveTex) && mat.GetTexture(DST_EmissiveTex) != null;
                Color emCol = mat.HasProperty(DST_EmissiveCol) ? mat.GetColor(DST_EmissiveCol) : Color.black;
                bool hasEmissiveStatic = hasEmissiveTex || (emCol.maxColorComponent > 0.001f);

                bool hasScroll = mat.HasProperty(DST_EmScrollTex) && mat.GetTexture(DST_EmScrollTex) != null;

                int emissiveMode = hasScroll ? 2 : (hasEmissiveStatic ? 1 : 0); // 0 Off, 1 Static, 2 Scrolling

                SetSgBoolKeyword(mat, KW_USE_NORMAL_MAP, PROP_USE_NORMAL_MAP, hasNormal);
                SetSgBoolKeyword(mat, KW_USE_MS_MAP, PROP_USE_MS_MAP, hasMS);
                SetSgEnumKeyword(mat, KW_EMISSIVE_MODE, emissiveMode);
                SetEnumPropSafe(mat, "_EMISSIVE_MODE", emissiveMode);

                EditorUtility.SetDirty(mat);

                var path = AssetDatabase.GetAssetPath(mat);
                sb.AppendLine($"- {path}  | N:{hasNormal}  MS:{hasMS}  EmMode:{(emissiveMode == 0 ? "Off" : emissiveMode == 1 ? "Static" : "Scrolling")}  | {DumpKw(mat)}");
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        Debug.Log(sb.ToString());
    }

    static string DumpKw(Material mat)
    {
#if UNITY_2021_2_OR_NEWER
        var sb = new System.Text.StringBuilder();

        // booleans you already use
        var boolNames = new[] { "_USE_NORMAL_MAP", "USE_NORMAL_MAP", "_USE_MS_MAP", "USE_MS_MAP" };
        foreach (var n in boolNames)
        {
            var kw = mat.shader.keywordSpace.FindKeyword(n);
            if (kw.isValid) sb.Append($"{n}:{mat.IsKeywordEnabled(kw)} ");
        }

        // enum state (report which option is enabled)
        string[] labels = { "EM_OFF", "EM_STATIC", "EM_SCROLL" };
        for (int i = 0; i < KW_EMISSIVE_MODE.Length; i++)
        {
            // find first valid kw for this option
            LocalKeyword chosen = default;
            foreach (var name in KW_EMISSIVE_MODE[i])
            {
                var kw = mat.shader.keywordSpace.FindKeyword(name);
                if (kw.isValid) { chosen = kw; break; }
            }
            if (chosen.isValid && mat.IsKeywordEnabled(chosen))
            {
                sb.Append($"EMISSIVE_MODE:{labels[i]} ");
                break;
            }
        }

        if (mat.HasProperty("_EMISSIVE_MODE")) sb.Append($"EM_PROP:{mat.GetFloat("_EMISSIVE_MODE")} ");

        return sb.Length > 0 ? sb.ToString() : "(no SG keywords found)";
#else
    return string.Join(",", mat.shaderKeywords);
#endif
    }
}