using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using Utility;


[CustomPropertyDrawer(typeof(SerializableDictionary), true)]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    private HashSet<int> collisionHashSet = new HashSet<int>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var listProp = property.FindPropertyRelative("list");
        EditorGUI.PropertyField(position, listProp, new GUIContent(property.displayName), true);

        collisionHashSet.Clear();

        // Check for duplicate Keys (only supported by enums/strings for now)
        for (int i = 0; i < listProp.arraySize; i++)
        {
            var element = listProp.GetArrayElementAtIndex(i);
            if (element != null)
            {
                var keyProp = element.FindPropertyRelative("Key");

                int hash;
                var display = "";
                var showWarning = false;

                switch (keyProp.propertyType)
                {
                    case SerializedPropertyType.Enum:
                        display = keyProp.enumDisplayNames[keyProp.enumValueIndex];
                        hash = keyProp.enumValueIndex.GetHashCode();
                        if (collisionHashSet.Contains(hash)) showWarning = true;
                        else collisionHashSet.Add(hash);
                        break;
                    case SerializedPropertyType.Integer:
                        display = keyProp.intValue.ToString();
                        hash = keyProp.intValue.GetHashCode();
                        if (collisionHashSet.Contains(hash)) showWarning = true;
                        else collisionHashSet.Add(hash);
                        break;
                    case SerializedPropertyType.Float:
                        display = keyProp.floatValue.ToString();
                        hash = keyProp.floatValue.GetHashCode();
                        if (collisionHashSet.Contains(hash)) showWarning = true;
                        else collisionHashSet.Add(hash);
                        break;
                    case SerializedPropertyType.Boolean:
                        display = keyProp.boolValue.ToString();
                        hash = keyProp.boolValue.GetHashCode();
                        if (collisionHashSet.Contains(hash)) showWarning = true;
                        else collisionHashSet.Add(hash);
                        break;
                    case SerializedPropertyType.String:
                        display = keyProp.stringValue;
                        hash = keyProp.stringValue.GetHashCode();
                        if (collisionHashSet.Contains(hash)) showWarning = true;
                        else collisionHashSet.Add(hash);
                        break;
                    case SerializedPropertyType.ObjectReference:
                        var obj = keyProp.objectReferenceValue;
                        display = obj != null ? obj.ToString() : "null";
                        hash = obj != null ? obj.GetHashCode() : 0;
                        if (collisionHashSet.Contains(hash)) showWarning = true;
                        else collisionHashSet.Add(hash);
                        break;
                    case SerializedPropertyType.ManagedReference:
                        var type = keyProp.managedReferenceFullTypename;
                        var value = keyProp.managedReferenceValue;
                        display = value != null ? value.ToString() : "null";
                        hash = value != null ? value.GetHashCode() : 0;
                        if (collisionHashSet.Contains(hash)) showWarning = true;
                        else collisionHashSet.Add(hash);
                        break;
                    case SerializedPropertyType.Color:
                        display = keyProp.colorValue.ToString();
                        hash = keyProp.colorValue.GetHashCode();
                        if (collisionHashSet.Contains(hash)) showWarning = true;
                        else collisionHashSet.Add(hash);
                        break;
                    case SerializedPropertyType.LayerMask:
                        display = LayerMask.LayerToName(keyProp.intValue);
                        hash = keyProp.intValue.GetHashCode();
                        if (collisionHashSet.Contains(hash)) showWarning = true;
                        else collisionHashSet.Add(hash);
                        break;
                }

                if (showWarning) EditorGUILayout.HelpBox("Dictionary Item at index " + i + " (" + display + ") is a duplicate key and will not be set", MessageType.Warning);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var listProp = property.FindPropertyRelative("list");
        if (listProp.isExpanded)
            return EditorGUI.GetPropertyHeight(listProp);
        else
            return EditorGUIUtility.singleLineHeight;
    }
}
