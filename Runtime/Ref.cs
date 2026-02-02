using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref : Singleton<Ref>
{
    static List<(object, Type)> pendingRegistrations = new();

    [SerializeField]
    List<ScriptableObject> scriptableObjectReferences = new();

    readonly Dictionary<Type, object> instances = new();

    public delegate void RegistrationHandler(Type type, object instance);
    public event RegistrationHandler OnRegistered;
    public event RegistrationHandler OnUnregistered;

    protected override void OnSingletonInitialized() 
    {
        for (int i = 0; i < pendingRegistrations.Count; i++)
        {
            var (instance, type) = pendingRegistrations[i];
            Register(instance, type);
        }

        pendingRegistrations.Clear();

        foreach (var scriptableObject in scriptableObjectReferences)
        {
            Register(scriptableObject, scriptableObject.GetType());
        }
    }

    private void OnDestroy()
    {
        foreach (var scriptableObject in scriptableObjectReferences)
        {
            Unregister(scriptableObject.GetType());
        }
    }

    public static void Register(object instance, Type type)
    {
        if (Instance == null)
        {
            pendingRegistrations.Add((instance, type));
            return;
        }    

        if (Instance.instances.ContainsKey(type))
        {
            Instance.instances[type] = instance;
            Instance.OnRegistered?.Invoke(type, instance);
        }
        else
        {
            Instance.instances.Add(type, instance);
            Instance.OnRegistered?.Invoke(type, instance);
        }
    }

    public static void Register<T>(object instance) => Register(instance, typeof(T));

    public static void Unregister(Type type)
    {
        if (Instance == null)
        {
            pendingRegistrations.RemoveAll(pr => pr.Item2 == type);
            return;
        }

        if (Instance.instances.TryGetValue(type, out object instance))
        {
            Instance.instances.Remove(type);
            Instance.OnUnregistered?.Invoke(type, instance);
        }
    }

    public static void Unregister<T>() => Unregister(typeof(T));

    public static void Unregister(object instance, Type type)
    {
        if (Instance == null)
        {
            pendingRegistrations.RemoveAll(pr => pr.Item1 == instance && pr.Item2 == type);
            return;
        }

        if (Instance.instances.TryGetValue(type, out object registeredInstance) && registeredInstance == instance)
        {
            Instance.instances.Remove(type);
            Instance.OnUnregistered?.Invoke(type, registeredInstance);
        }
    }

    public static void Unregister<T>(object instance) => Unregister(instance, typeof(T));

    public static void Unregister(object instance) => Unregister(instance, instance.GetType());

    public static T Get<T>()
    {
        if (Instance == null)
            return default;

        var type = typeof(T);
        if (Instance.instances.TryGetValue(type, out object instance))
            return (T)instance;

        return default;
    }

    public static bool TryGet<T>(out T instance)
    {
        var type = typeof(T);

        if (Instance != null && Instance.instances.TryGetValue(type, out object obj))
        {
            instance = (T)obj;
            return true;
        }

        instance = default;
        return false;
    }

    public static bool TryGet<T>(out T instance, LogType logIfNotFound)
    {
        if (TryGet(out instance))
            return true;
        
        switch (logIfNotFound)
        {
            case LogType.Error:
                Debug.LogError($"Ref: {typeof(T)} not found.");
                break;
            case LogType.Warning:
                Debug.LogWarning($"Ref: {typeof(T)} not found.");
                break;
            case LogType.Log:
                Debug.Log($"Ref: {typeof(T)} not found.");
                break;
        }

        return false;
    }
}
