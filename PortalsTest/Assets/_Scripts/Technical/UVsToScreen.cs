using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVsToScreen : MonoBehaviour
{

    public Camera cmr;
    public MeshFilter rend;

    private void Start()
    {
        cmr = Camera.main;
        rend = GetComponent<MeshFilter>();
    }
    private void Update()
    {
        
        Mesh newMesh = new Mesh();
        newMesh.vertices = rend.mesh.vertices;
        newMesh.triangles = rend.mesh.triangles;

        List<Vector2> u = new List<Vector2>();
        {
        for (int i = 0; i < newMesh.vertices.Length; i++)
            u.Add(cmr.WorldToViewportPoint(newMesh.vertices[i]));
        }

        newMesh.uv = u.ToArray();

        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();


        rend.mesh = newMesh;
    }

}
