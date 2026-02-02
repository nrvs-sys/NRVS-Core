#if UNITY_EDITOR
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

public static class LocalizationWordCounter
{
    private static readonly Regex WordRegex = new(@"\b\p{L}+\b", RegexOptions.Compiled);

    [MenuItem("Tools/Localization/Word Count")]
    private static void CountWords()
    {
        var collections = LocalizationEditorSettings.GetStringTableCollections();
        int grandTotal = 0;

        foreach (var collection in collections)
        {
            // each locale has its own StringTable in the collection
            foreach (var table in collection.StringTables)
            {
                int tableWords = table.Values.Sum(e => WordCount(e.Value));
                grandTotal += tableWords;

                Debug.Log($"[{table.LocaleIdentifier}] {collection.TableCollectionName}: {tableWords} words");
            }
        }

        Debug.Log($"--- total words across all tables & locales: {grandTotal} ---");
    }

    private static int WordCount(string text) =>
        string.IsNullOrWhiteSpace(text) ? 0 : WordRegex.Matches(text).Count;
}
#endif