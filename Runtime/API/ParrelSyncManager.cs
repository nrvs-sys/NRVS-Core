using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using ParrelSync;
#endif

#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class ParrelSyncManager
	{
		public enum ParrelInstanceType
		{
			Main,
			Server,
			Client
		}

		public static ParrelInstanceType type
		{
			get;
			private set;
		} = ParrelInstanceType.Main;

		public static bool IsAnyCloneRunning()
		{
#if UNITY_EDITOR
			if (!active)
				return false;

			if (ClonesManager.IsClone())
				return true;

			foreach (var clonePath in ClonesManager.GetCloneProjectsPath())
			{
				if (ClonesManager.IsCloneProjectRunning(clonePath))
					return true;
			}
#endif
			return false;
		}

	public static string GetArgument() =>
#if UNITY_EDITOR
        ClonesManager.GetArgument();
#else
        "";
#endif


#if UNITY_EDITOR
	static readonly bool active = true;

		static bool appFocused;

		static ParrelSyncManager()
		{
			if (!active)
			{
				Debug.Log("ParrelSyncManager is inactive - Starting Editor normally");
				type = ParrelInstanceType.Main;
			}

			if (ClonesManager.IsClone())
			{
				if (ClonesManager.GetArgument().Contains("server"))
				{
					Debug.Log("Starting ParrelSync Editor Clone as a Server");
					type = ParrelInstanceType.Server;
				}
				else
				{
					Debug.Log("Starting ParrelSync Editor Clone as a Client");
					type = ParrelInstanceType.Client;
				}
			}
			else
			{
				Debug.Log("Starting ParrelSync Main Editor as a Client");
				type = ParrelInstanceType.Main;
			}

			EditorApplication.playModeStateChanged += OnPlayStateChange;
			EditorApplication.update += OnUpdate;
		}

		static void OnPlayStateChange(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredPlayMode)
			{
				OnPlayModeEntered();
			}
			else if (state == PlayModeStateChange.ExitingPlayMode)
			{
				OnPlayModeExited();
			}
		}

		static void OnPlayModeEntered()
		{
			if (ClonesManager.IsClone())
			{

			}
		}

		static void OnPlayModeExited()
		{
			if (ClonesManager.IsClone())
			{
				
			}
		}

		static void OnUpdate()
		{
			if (!appFocused && UnityEditorInternal.InternalEditorUtility.isApplicationActive)
			{
				appFocused = UnityEditorInternal.InternalEditorUtility.isApplicationActive;
			}
			else if (appFocused && !UnityEditorInternal.InternalEditorUtility.isApplicationActive)
			{
				appFocused = UnityEditorInternal.InternalEditorUtility.isApplicationActive;
			}
		}
#endif
	}

