using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Used for vertex painting with particle collisions.
/// Be sure to have sendmessages inside the particle system ticked.
/// </summary>
public class VertexColorParticle : MonoBehaviour {

    // particle collision detection stuff
    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;
    public Color m_vertColor;
    // this could be moved on the object we are colliding with for a flamable rate?
    public float m_vertPaintSpeed = 0.25f;

    // Use this for initialization
    void Start ()
    {
        // fetch the component from our object
        part = GetComponent<ParticleSystem>();
        // create a new list for use.
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {

        // the amount of collision events we have during this frame
        if (collisionEvents == null)
            return;
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
        // attempt to fetch the objects meshcollider if it has one
        MeshCollider meshCollider = other.GetComponent<Collider>() as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        // store the mesh
        Mesh mesh = meshCollider.sharedMesh;
        // store the vertices
        Vector3[] vertices = mesh.vertices;
        // store the tris
        int[] triangles = mesh.triangles;
        // create new colors
        Color[] colors = new Color[vertices.Length];
        // assign colors
        colors = mesh.colors;
        
        // iterate through the collisions
        int i = 0;
        while (i < numCollisionEvents)
        {
            // get the data from the collision
            Vector3 pos = collisionEvents[i].intersection;
			Vector3 dir = (other.transform.position - pos).normalized;
			// thing.transform.position - transform.position;

            // draw a debug ray
            Debug.DrawRay(pos, dir);
            // hitinfo to store our data we collect from the raycast
            RaycastHit hitinfo;

            // check if there is a raycast collisions
            // if there isn't just increment and continue to the next iteration
            if (!Physics.Raycast(pos, dir, out hitinfo, 1f))
            {
                i++;
                continue;
            }


            // color the edges in a triangle

            //TODO: fix subscipt error, it's still persistent.
            if (hitinfo.triangleIndex >= 0 && (hitinfo.triangleIndex * 3 + 2) < (triangles.Length - 1))
            {
                    colors[triangles[hitinfo.triangleIndex * 3 + 0]] -= m_vertColor * m_vertPaintSpeed;
                    colors[triangles[hitinfo.triangleIndex * 3 + 1]] -= m_vertColor * m_vertPaintSpeed;
                    colors[triangles[hitinfo.triangleIndex * 3 + 2]] -= m_vertColor * m_vertPaintSpeed;
            }
            // assign the new colors
            mesh.colors = colors;

           // Debug.Break();
            // increment
            i++;
        }
    }
}
