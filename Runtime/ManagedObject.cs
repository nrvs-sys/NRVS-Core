using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public abstract class ManagedObject : ScriptableObject
    {
        abstract protected void Initialize();
        abstract protected void Cleanup();
#if UNITY_EDITOR
        protected void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayStateChange;
        }

        protected void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayStateChange;
        }

        void OnPlayStateChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Initialize();
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Cleanup();
            }
        }
#else
    protected void OnEnable()
    {
        Initialize();
    }

    protected void OnDisable()
    {
        Cleanup();
    }
#endif
    }
