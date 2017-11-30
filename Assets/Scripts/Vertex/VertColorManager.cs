using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Used to reset vert colors, as they persist through play mode.
/// </summary>
public class VertColorManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        // store all meshes
        MeshCollider[] meshes = GameObject.FindObjectsOfType<MeshCollider>();
        // loop through meshes
        foreach(var mc in meshes)
        {
            // store mesh
            Mesh mesh = mc.sharedMesh;
            // store verticies
            Vector3[] vertices = mesh.vertices;
            // store tris
            int[] triangles = mesh.triangles;
            // create colors array
            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.white;
                colors[i].a = 0f;
            }
            // assign new color array
            mesh.colors = colors;
        }
    }

}
