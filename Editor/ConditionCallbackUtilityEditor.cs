using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(ConditionCallbackUtility<>), true)]
public class ConditionCallbackUtilityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the target object
        var targetObject = target as MonoBehaviour;

        // Get the type of the target object
        var targetType = targetObject.GetType();

        // Get the generic type argument
        var genericType = GetGenericType(targetType);

        // Get the conditionCallback field from the base class
        var conditionCallbackField = GetFieldFromBaseClass(targetType, "conditionCallback");

        string labelState = "None";
        GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = Color.red },
            fontSize = 12
        };

        if (conditionCallbackField != null)
        {
            var conditionCallback = conditionCallbackField.GetValue(targetObject);

            if (conditionCallback != null)
            {
                // Get the condition field from the conditionCallback
                var conditionField = conditionCallback.GetType().GetField("condition", BindingFlags.NonPublic | BindingFlags.Instance);

                var condition = conditionField?.GetValue(conditionCallback);

                if (condition != null)
                {

                    var conditionGenericType = GetGenericType(condition.GetType());

                    // Check if the condition is generic in the first place
                    bool isGeneric = conditionGenericType != null;

                    // Check if the condition is of the expected generic type
                    bool isSameType = condition != null && conditionGenericType == genericType;

                    if (isGeneric)
                    {
                        labelState = isSameType ? "Valid" : $"Invalid ({conditionGenericType.Name})";
                        labelStyle.normal.textColor = isSameType ? Color.green : Color.yellow;
                    }
                    else
                    {
                        labelState = "Non-Generic";
                        labelStyle.normal.textColor = Color.yellow;
                    }
                }
            }
        }

        // Display the result in the inspector with custom styles
        var label = $"Condition Callback Type Check ({genericType?.Name ?? "Unknown"})";
        EditorGUILayout.LabelField(label, labelState, labelStyle);
    }

    private FieldInfo GetFieldFromBaseClass(Type type, string fieldName)
    {
        while (type != null)
        {
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field;
            type = type.BaseType;
        }
        return null;
    }

    private Type GetGenericType(Type type)
    {
        while (type != null)
        {
            if (type.IsGenericType)
                return type.GetGenericArguments()[0];
            type = type.BaseType;
        }
        return null;
    }
}
