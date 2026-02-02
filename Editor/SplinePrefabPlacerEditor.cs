using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public sealed class SplinePrefabPlacerEditor : EditorWindow
{
    GameObject prefab;
    SplineContainer splineContainer;
    float spacing = 1f;
    Vector3 offset = Vector3.zero;
    bool alignToSpline = true;
    Transform parent;

    [MenuItem("Tools/Spline Prefab Placer")]
    static void Open() => GetWindow<SplinePrefabPlacerEditor>("Spline Prefab Placer");

    void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        splineContainer = (SplineContainer)EditorGUILayout.ObjectField("Spline", splineContainer, typeof(SplineContainer), true);
        spacing = Mathf.Max(0.01f, EditorGUILayout.FloatField("Spacing", spacing));
        offset = EditorGUILayout.Vector3Field("Offset", offset);
        alignToSpline = EditorGUILayout.Toggle("Align to Tangent", alignToSpline);
        parent = (Transform)EditorGUILayout.ObjectField("Parent (opt.)", parent, typeof(Transform), true);

        EditorGUI.BeginDisabledGroup(prefab == null || splineContainer == null);
        if (GUILayout.Button("Place Prefabs")) PlacePrefabs();
        EditorGUI.EndDisabledGroup();
    }

    void PlacePrefabs()
    {
        if (parent == null)
            parent = new GameObject($"{prefab.name}_Instances").transform;

        Undo.RegisterCreatedObjectUndo(parent.gameObject, "Place Prefabs");

        var spline = splineContainer.Spline;
        float total = SplineUtility.CalculateLength(spline, splineContainer.transform.localToWorldMatrix);
        int count = Mathf.FloorToInt(total / spacing);

        float t = 0f;                                         // running interpolation
        float d = 0f;                                         // running distance

        for (int i = 0; i <= count; ++i, d += spacing)
        {
            float3 posLocal = SplineUtility.GetPointAtLinearDistance(spline, t, i == 0 ? 0f : spacing, out t);
            float3 tanLocal = SplineUtility.EvaluateTangent(spline, t);

            Vector3 posWorld = splineContainer.transform.TransformPoint((Vector3)posLocal);
            Vector3 tanWorld = splineContainer.transform.TransformDirection((Vector3)tanLocal);

            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.transform.position = posWorld + offset;

            if (alignToSpline && math.lengthsq(tanLocal) > 0f)
                go.transform.rotation = Quaternion.LookRotation(tanWorld.normalized, Vector3.up);
        }
    }
}