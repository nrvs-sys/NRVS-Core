using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Text;
using System.IO;
using UnityEngine;

public partial class EditorMenuSceneBuilder
{
    private const string ASSETS_SCENE_PATH = "Assets/_Project/Scenes/";

    private const string PATH_TO_SCENES_FOLDER = "/_Project/Scenes/";
    private const string PATH_TO_OUTPUT_SCRIPT_FILE = "/NRVS/Editor/Generated/EditorMenuSceneBuilder.cs";

    [MenuItem("Utilities/Generate Scene Load Menu Code", priority = 25)]
    public static void GenerateSceneLoadMenuCode()
    {
        StringBuilder result = new StringBuilder();
        string basePath = Application.dataPath + PATH_TO_SCENES_FOLDER;
        AddClassHeader(result);
        AddOpenSceneMethod(result);
        AddCodeForDirectory(new DirectoryInfo(basePath), result);
        AddClassFooter(result);

        // Ensure directory exists
        string outputDirectory = Path.GetDirectoryName(Application.dataPath + PATH_TO_OUTPUT_SCRIPT_FILE);
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        string scriptPath = Application.dataPath + PATH_TO_OUTPUT_SCRIPT_FILE;
        File.WriteAllText(scriptPath, result.ToString());

        void AddCodeForDirectory(DirectoryInfo directoryInfo, StringBuilder result)
        {
            FileInfo[] fileInfoList = directoryInfo.GetFiles();
            for (int i = 0; i < fileInfoList.Length; i++)
            {
                FileInfo fileInfo = fileInfoList[i];
                if (fileInfo.Extension == ".unity")
                {
                    AddCodeForFile(fileInfo, result);
                }
            }
            DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
            for (int i = 0; i < subDirectories.Length; i++)
            {
                AddCodeForDirectory(subDirectories[i], result);
            }

            void AddCodeForFile(FileInfo fileInfo, StringBuilder result)
            {
                string subPath = fileInfo.FullName.Replace('\\', '/').Replace(basePath, "");
                string assetPath = ASSETS_SCENE_PATH + subPath;

                string functionName = fileInfo.Name.Replace(".unity", "").Replace(" ", "").Replace("-", "").Replace("_", "").Replace("(", "").Replace(")","");

                result.Append("        [MenuItem(\"Utilities/Load Scenes/").Append(subPath.Replace(".unity", "")).Append("\", priority = 35)]").Append(Environment.NewLine);
                result.Append("        public static void Load").Append(functionName).Append("() { OpenScene(\"").Append(assetPath).Append("\"); }").Append(Environment.NewLine); ;
            }
        }
    }

    private static void AddClassHeader(StringBuilder result)
    {
        result.Append(@"using UnityEditor;
                public partial class EditorMenuSceneBuilder
                {");
    }

    private static void AddOpenSceneMethod(StringBuilder result)
    {
        result.Append(@"
        private static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }");
    }

    private static void AddClassFooter(StringBuilder result)
    {
        result.Append("}");
    }
}
