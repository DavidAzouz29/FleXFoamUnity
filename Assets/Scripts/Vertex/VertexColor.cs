using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VertexColor : MonoBehaviour {
    /// <summary>
    /// This is only an example, as it stands it will only correctly work with one object atm.
    /// </summary>
    Camera cam;
    [SerializeField]
    string m_tag = "VertexObject";

    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
        MeshCollider meshCollider = GameObject.FindGameObjectWithTag(m_tag).GetComponent<MeshCollider>();
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        // store the triangles
        int[] triangles = mesh.triangles;
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        mesh.colors = colors;
    }
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;
        // check if there was a hit otherwise return
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            return;
        // store the mesh collider and check if it actually has a mesh collider
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Mesh mesh = meshCollider.sharedMesh;
        // store the vertices
        Vector3[] vertices = mesh.vertices;
        // store the triangles
        int[] triangles = mesh.triangles;
        Color[] colors = new Color[vertices.Length];
        colors = mesh.colors;

        // draw triangle
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        
        // check if click
        if (Input.GetMouseButtonDown(0))
        {
            // paint verts
            colors[triangles[hit.triangleIndex * 3 + 0]] = Color.red;
            colors[triangles[hit.triangleIndex * 3 + 1]] = Color.red;
            colors[triangles[hit.triangleIndex * 3 + 2]] = Color.red;

        }
        mesh.colors = colors;



        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0, p1, mesh.colors[triangles[hit.triangleIndex * 3 + 0]]);
        Debug.DrawLine(p1, p2, mesh.colors[triangles[hit.triangleIndex * 3 + 1]]);
        Debug.DrawLine(p2, p0, mesh.colors[triangles[hit.triangleIndex * 3 + 2]]);
    }
}
