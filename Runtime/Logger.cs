using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Logger_ New", menuName = "Utilities/Logger")]
public class Logger : ScriptableObject
{
    [Header("Settings")]
    public bool isEnabled = true;
    [Tooltip("Inserted before the log")]
    public string description = "Logger: ";


    [Obsolete]
    public void String(string str) => Debug.Log(str);
    [Obsolete]
    public void Float(float num) => Debug.Log(num);
    [Obsolete]
    public void Vector3(Vector3 vec) => Debug.Log(vec);


    // These overloads are used for UnityEvents
    #region Value Overloads

    public void Log(bool value) => Log<object>(value);
    public void LogWarning(bool value) => LogWarning<object>(value);
    public void LogError(bool value) => LogError<object>(value);

    public void Log(string value)             => Log<object>(value);
    public void LogWarning(string value)      => LogWarning<object>(value);
    public void LogError(string value)        => LogError<object>(value);

    public void Log(float value)              => Log<object>(value);
    public void LogWarning(float value)       => LogWarning<object>(value);
    public void LogError(float value)         => LogError<object>(value);

    public void Log(int value)                => Log<object>(value);
    public void LogWarning(int value)         => LogWarning<object>(value);
    public void LogError(int value)           => LogError<object>(value);

    public void Log(Vector3 value)            => Log<object>(value);
    public void LogWarning(Vector3 value)     => LogWarning<object>(value);
    public void LogError(Vector3 value)       => LogError<object>(value);

    public void Log(Vector2 value)            => Log<object>(value);
    public void LogWarning(Vector2 value)     => LogWarning<object>(value);
    public void LogError(Vector2 value)       => LogError<object>(value);

    public void Log(Quaternion value)         => Log<object>(value);
    public void LogWarning(Quaternion value)  => LogWarning<object>(value);
    public void LogError(Quaternion value)    => LogError<object>(value);

    public void Log(Color value)              => Log<object>(value);
    public void LogWarning(Color value)       => LogWarning<object>(value);
    public void LogError(Color value)         => LogError<object>(value);

    public void Log(GameObject value)         => Log<object>(value);
    public void LogWarning(GameObject value)  => LogWarning<object>(value);
    public void LogError(GameObject value)    => LogError<object>(value);

    public void Log(ContactInfo value)        => Log<object>(value);
    public void LogWarning(ContactInfo value) => LogWarning<object>(value);
    public void LogError(ContactInfo value)   => LogError<object>(value);

    public void Log(DamageInfo value) => Log<object>(value);
    public void LogWarning(DamageInfo value) => LogWarning<object>(value);
    public void LogError(DamageInfo value) => LogError<object>(value);

    #endregion


    public void Log<T>(T value)
    {
        if (isEnabled)
            Debug.Log(description + value);
    }

    public void LogWarning<T>(T value)
    {
        if (isEnabled)
            Debug.LogWarning(description + value);
    }

    public void LogError<T>(T value)
    {
        if (isEnabled)
            Debug.LogError(description + value);
    }
}
