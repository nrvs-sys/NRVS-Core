using UnityEngine;


public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{

    /// <summary>
    ///		The instance used for calling non-static methods for the class.
    ///		
    ///		When getting this value - If the instance is currently null, this method will search open scenes for a new instance. To get the instance with less overhead, use `InstanceFast` instead.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindFirstObjectByType<T>();

            return _Instance;
        }

        private set
        {
            _Instance = value;
        }
    }

    /// <summary>
    ///		Gets the instance used for calling non-static methods for the class. 
    ///		
    ///		If the instance is currently null, this method will *not* search open scenes for a new instance.
    /// </summary>
    public static T InstanceFast => _Instance;

    private static T _Instance;

    public static void RegisterSingleton(T instance)
    {
        if (instance == null)
        {
            Debug.LogError($"Attempted to register a null instance of {typeof(T).Name}.", instance);
            return;
        }

        if (_Instance == null)
            Instance = instance;
        else
        {
            //	Check if this is a duplicate, or if it was just set prior.  If duplicate, delete.
            if (Instance.gameObject != instance.gameObject)
            {
                instance.gameObject.SendMessage("OnIsDuplicate", null, SendMessageOptions.DontRequireReceiver);
                Destroy(instance.gameObject);
                return;
            }
        }
    }

    public static void UnregisterSingleton(T instance)
    {
        if (Instance == instance)
            _Instance = null;
        else
            Debug.LogWarning($"Attempted to unregister singleton {typeof(T).Name} but it was not the current instance.", instance);
    }

    /// <summary>
    ///		Awake should only exist here.  If the desired script is already attached to a GameObject, uses 'this' for a reference.
    ///		Initialize is then called as a substitute to Awake.
    /// </summary>
    protected virtual void Awake()
    {
        //	Don't allow Instance to be set if an inheritor is running in ExecuteInEditMode.
        //	If executing in edit mode is desired, utilize the RegisterSingleton/UnregisterSingleton methods to manually handle the Singleton's lifecycle.
        if (Application.isPlaying)
        {
            RegisterSingleton(this as T);
        }

        OnSingletonInitialized();
    }



    /// <summary>
    /// Called when the Singleton is initialized, on Awake.
    /// </summary>
    protected abstract void OnSingletonInitialized();
}
