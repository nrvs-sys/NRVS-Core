using UnityAtoms.BaseAtoms;
using UnityEngine;

public abstract class ConditionBehavior : ScriptableObject
{
    public enum DebugLogLevel
    {
        None,
        Log,
        Warning,
        Error
    }

    [Header("Developer Settings")]

    [SerializeField, TextArea]
    string developerNote;

    [SerializeField]
    protected DebugLogLevel logsWhenTrue;

    [SerializeField]
    protected DebugLogLevel logsWhenFalse;

    [Header("Condition Settings")]

    [SerializeField]
    protected bool inverse;



    public bool If()
    {
        var result = Evaluate();

        if (inverse)
            result = !result;

        if (result)
        {
            switch (logsWhenTrue)
            {
                case DebugLogLevel.Log:
                    Debug.Log($"Condition {name}: Evaluated to true");
                    break;
                case DebugLogLevel.Warning:
                    Debug.LogWarning($"Condition {name}: Evaluated to true");
                    break;
                case DebugLogLevel.Error:
                    Debug.LogError($"Condition {name}: Evaluated to true");
                    break;
            }
        }
        else
        {
            switch (logsWhenFalse)
            {
                case DebugLogLevel.Log:
                    Debug.Log($"Condition {name}: Evaluated to false");
                    break;
                case DebugLogLevel.Warning:
                    Debug.LogWarning($"Condition {name}: Evaluated to false");
                    break;
                case DebugLogLevel.Error:
                    Debug.LogError($"Condition {name}: Evaluated to false");
                    break;
            }
        }

        return result;
    }

    protected abstract bool Evaluate();
}

public abstract class ConditionBehavior<T> : ConditionBehavior
{
    [SerializeField]
    public T defaultValue;

    public virtual bool If(T value)
    {
        var result = Evaluate(value);

        if (inverse)
            result = !result;

        if (result)
        {
            switch (logsWhenTrue)
            {
                case DebugLogLevel.Log:
                    Debug.Log($"Condition {name}: Evaluated to true");
                    break;
                case DebugLogLevel.Warning:
                    Debug.LogWarning($"Condition {name}: Evaluated to true");
                    break;
                case DebugLogLevel.Error:
                    Debug.LogError($"Condition {name}: Evaluated to true");
                    break;
            }
        }
        else
        {
            switch (logsWhenFalse)
            {
                case DebugLogLevel.Log:
                    Debug.Log($"Condition {name}: Evaluated to false");
                    break;
                case DebugLogLevel.Warning:
                    Debug.LogWarning($"Condition {name}: Evaluated to false");
                    break;
                case DebugLogLevel.Error:
                    Debug.LogError($"Condition {name}: Evaluated to false");
                    break;
            }
        }

        return result;
    }

    override protected bool Evaluate() => Evaluate(defaultValue);

    protected abstract bool Evaluate(T value);
}
