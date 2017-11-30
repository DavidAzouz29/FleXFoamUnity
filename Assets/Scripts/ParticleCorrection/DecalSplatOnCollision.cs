using System.Collections.Generic;
using UnityEngine;

public class DecalSplatOnCollision : MonoBehaviour
{

    public ParticleSystem particleLauncher;
    public Gradient particleColorGradient;
    public ParticleDecalPool dropletDecalPool;
    private int particleCol = 128;

    List<ParticleCollisionEvent> collisionEvents;


    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents(particleLauncher, other, collisionEvents);

        int i = 0;
        while (i < numCollisionEvents)
        {
            if (i % particleCol == 0)
            {
                dropletDecalPool.ParticleHit(collisionEvents[i], particleColorGradient);
            }
            i++;
        }

    }

}