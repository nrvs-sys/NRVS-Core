using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build;

[InitializeOnLoad]
public static class EditorMenuItems
{
	const string menuRoot = "Utilities";
	const string sceneMenuRoot = menuRoot + "/Load Scenes";
	const string contentSceneMenuRoot = sceneMenuRoot + "/Content Scenes";

	const string sceneRoot = "Assets/_Project/Scenes";
	const string appSceneRoot = sceneRoot + "/Application";
	const string playerSceneRoot = sceneRoot + "/Player";
	const string levelSceneRoot = sceneRoot + "/Game/Levels";

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

	[MenuItem(sceneMenuRoot + "/Player Scenes", priority = 24)]
	public static void LoadPlayerScenes()
	{
		OpenScene($"{playerSceneRoot}/XR/XR Player.unity");
        OpenScene($"{playerSceneRoot}/XR/XR Pause Menu.unity", OpenSceneMode.Additive);

        OpenScene($"{playerSceneRoot}/Flat/Flat Player.unity", OpenSceneMode.Additive);
        OpenScene($"{playerSceneRoot}/Flat/Flat Pause Menu.unity", OpenSceneMode.Additive);

        OpenScene($"{playerSceneRoot}/Spectator Camera.unity", OpenSceneMode.Additive);
	}

    [MenuItem(sceneMenuRoot + "/Level Scenes", priority = 24)]
    public static void LoadLevelScenes()
    {
        // open all level scenes in the Levels directory (and subdirectories)
		var sceneFiles = System.IO.Directory.GetFiles(levelSceneRoot, "*.unity", System.IO.SearchOption.AllDirectories);
		var firstOpen = true;

		foreach (var sceneFile in sceneFiles)
		{
			OpenScene(sceneFile, firstOpen ? OpenSceneMode.Single : OpenSceneMode.Additive);
			firstOpen = false;
		}
    }

	//[MenuItem(sceneMenuRoot + "/Core Scenes", priority = 0)]
	//public static void LoadCoreScenes()
	//{
	//	OpenScene($"{sceneRoot}/Core/Entities.unity");
	//	OpenScene($"{sceneRoot}/Core/Voip.unity", OpenSceneMode.Additive);
	//}

	//[MenuItem(sceneMenuRoot + "/Client Scenes")]
	//public static void LoadClientScenes()
	//{
	//	LoadCoreScenes();
	//	OpenScene($"{sceneRoot}/Player.unity", OpenSceneMode.Additive);
	//	OpenScene($"{sceneRoot}/UI/Pause UI.unity", OpenSceneMode.Additive);
	//	OpenScene($"{sceneRoot}/Connection Lobby.unity", OpenSceneMode.Additive);
	//}

	//[MenuItem(sceneMenuRoot + "/Server Scenes")]
	//public static void LoadServerScenes()
	//{
	//	LoadCoreScenes();
	//	OpenScene($"{sceneRoot}/Connection Lobby.unity", OpenSceneMode.Additive);
	//	OpenScene($"{sceneRoot}/Server Camera.unity", OpenSceneMode.Additive);
	//}

    private static void OpenScene(string scenePath, OpenSceneMode openSceneMode = OpenSceneMode.Single)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scenePath, openSceneMode);
    }

	#endregion
}
