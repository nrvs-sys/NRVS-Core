using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshBuilder
{
    public List<Vector3> vertices = new();

    public List<Vector3> normals = new();

    public List<Vector2> uvs = new();

    private List<int> triangles = new();

    public MeshBuilder() { }

    public void AddTriangle(int index0, int index1, int index2)
    {
        triangles.Add(index0);
        triangles.Add(index1);
        triangles.Add(index2);
    }

    public void Clear()
    {
        vertices.Clear();
        normals.Clear();
        uvs.Clear();
        triangles.Clear();
    }

    public Mesh BuildMesh()
    {
        var mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        // Normals are optional. Only use them if we have the correct amount
        if (normals.Count == vertices.Count)
            mesh.normals = normals.ToArray();

        // UVs are optional. Only use them if we have the correct amount
        if (uvs.Count == vertices.Count)
            mesh.uv = uvs.ToArray();

        mesh.RecalculateBounds();

        return mesh;
    }
}
