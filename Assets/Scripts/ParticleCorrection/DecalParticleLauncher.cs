///<summary>
///
/// viewed: https://unity3d.com/learn/tutorials/topics/scripting/particle-collisions
/// Controlling Particles Via Script - Drawing Decals with Particles [7/11] Live 2017/2/8 https://youtu.be/EBofefw2sk8 
/// </summary>

using System.Collections.Generic;
using UnityEngine;

public class DecalParticleLauncher : MonoBehaviour {

    public ParticleSystem particleLauncher;
    public ParticleSystem splatterParticles;
    public Gradient particleColorGradient;
    public ParticleDecalPool splatDecalPool;
    private int particleCol = 512;

    List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleLauncher, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            // for performance, apply decal one in every particleCol collisions
            if (i % particleCol == 0)
            {
                splatDecalPool.ParticleHit(collisionEvents[i], particleColorGradient);
                EmitAtLocation(collisionEvents[i]);
            }
        }

    }

    void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
    {
        splatterParticles.transform.position = particleCollisionEvent.intersection;
        splatterParticles.transform.rotation = Quaternion.LookRotation(particleCollisionEvent.normal);
        ParticleSystem.MainModule psMain = splatterParticles.main;
        psMain.startColor = particleColorGradient.Evaluate(Random.Range(0f, 1f));

        splatterParticles.Emit(1);
    }

    void Update()
    {
        //if (Input.GetButton("Fire1"))
        //{
            ParticleSystem.MainModule psMain = particleLauncher.main;
            psMain.startColor = particleColorGradient.Evaluate(Random.Range(0f, 1f));
            particleLauncher.Emit(1);
        //}

    }
}
