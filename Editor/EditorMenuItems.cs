using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class EditorMenuItems
{
	const string menuRoot = "Utilities";
	const string sceneRoot = "Assets/_Project/Scenes";

	#region Scene Management

	static EditorMenuItems()
	{
		EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
	}

	private static void EditorApplication_playModeStateChanged(PlayModeStateChange playModeStateChange)
	{
		string previousEditorScene = EditorPrefs.GetString(Constants.EditorPrefs.EditorPreviousScene);
		bool bootInLevelTestMode = EditorPrefs.GetInt(Constants.EditorPrefs.EditorPlayMode) != (int)EditorPlayMode.Default;

		switch (playModeStateChange)
		{
			case PlayModeStateChange.EnteredEditMode:
				if (bootInLevelTestMode)
				{
					EditorPrefs.SetInt(Constants.EditorPrefs.EditorPlayMode, (int)EditorPlayMode.Default);
					EditorSceneManager.OpenScene(previousEditorScene);
				}
				break;
			case PlayModeStateChange.ExitingEditMode:
				break;
			case PlayModeStateChange.EnteredPlayMode:
				break;
			case PlayModeStateChange.ExitingPlayMode:				
				break;
			default:
				break;
		}
	}

    [MenuItem(menuRoot + "/Play From Boot Scene", priority = 0)]
    public static void PlayFromBootScene()
    {
        PlayEditorMode(EditorPlayMode.PlayFromBoot);
    }

    [MenuItem(menuRoot + "/Play From Current Scene", priority = 11)]
	public static void PlayFromCurrentScene()
	{
		PlayEditorMode(EditorPlayMode.PlayFromScene);
	}

	[MenuItem(menuRoot + "/Observe From Current Scene", priority = 12)]
	public static void PlayFromObserveScene()
	{
		PlayEditorMode(EditorPlayMode.ObserveFromScene);
    }

	public static void PlayEditorMode(EditorPlayMode mode)
	{
        string currentScene = EditorSceneManager.GetActiveScene().path;

        if (UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(currentScene) < 0)
        {
            EditorApplication.Beep();
            Debug.LogError($"{currentScene} is not in the build scene list! Add it to play from this scene.");
            return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorPrefs.SetString(Constants.EditorPrefs.EditorPreviousScene, currentScene);
            EditorPrefs.SetInt(Constants.EditorPrefs.EditorPlayMode, (int)mode);

            EditorSceneManager.OpenScene($"{sceneRoot}/Boot.unity");
            EditorApplication.isPlaying = true;
        }
    }

    private static void OpenScene(string scenePath, OpenSceneMode openSceneMode = OpenSceneMode.Single)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scenePath, openSceneMode);
    }

	#endregion
}
