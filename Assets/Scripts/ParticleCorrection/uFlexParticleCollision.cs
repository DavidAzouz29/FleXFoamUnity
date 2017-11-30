///<summary>
/// Name: uFlexParticleCollision.cs
/// Author: David Azouz
/// Date Created: 16/11/17
/// Date Modified: 16/11/17
/// --------------------------------------------------
/// Brief:
/// viewed: 
/// https://unity3d.com/learn/tutorials/topics/scripting/particle-collisions
/// https://docs.unity3d.com/ScriptReference/ParticleCollisionEvent.html
/// --------------------------------------------------
/// Edits:
/// - Script Created - David Azouz 16/11/17
/// 
/// TODO:
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uFlexParticleCollision : MonoBehaviour
{ }
/*    //public class SplatOnCollision : MonoBehaviour


    public ParticleSystem particleLauncher;
    public Gradient particleColorGradient;
    public ParticleDecalPool dropletDecalPool;
    public AnimationCurve slowdown;

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
            dropletDecalPool.ParticleHit(collisionEvents[i], particleColorGradient);
            i++;
        }

    }
}

public class ParticleDecalData
{
    public Vector3 position;
    public float size;
    public Vector3 rotation;
    public Color color;

}

public class FleXParticleDecalData
{
    public Vector3 position;
    public float size;
    public float velocity; // based off curve
    public float viscosity; //0f
    public float cohesion; //0.025f
    //public Vector3 rotation;
    public Color color;

}

public class ParticleDecalPool : MonoBehaviour
{

    public int maxDecals = 100;
    public float decalSizeMin = .5f;
    public float decalSizeMax = 1.5f;

    private ParticleSystem decalParticleSystem;
    private int particleDecalDataIndex;
    private ParticleDecalData[] particleData;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        decalParticleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[maxDecals];
        particleData = new ParticleDecalData[maxDecals];
        for (int i = 0; i < maxDecals; i++)
        {
            particleData[i] = new ParticleDecalData();
        }
    }

    public void ParticleHit(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {
        SetParticleData(particleCollisionEvent, colorGradient);
        DisplayParticles();
    }

    void SetParticleData(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {
        if (particleDecalDataIndex >= maxDecals)
        {
            particleDecalDataIndex = 0;
        }

        particleData[particleDecalDataIndex].position = particleCollisionEvent.intersection;
        Vector3 particleRotationEuler = Quaternion.LookRotation(particleCollisionEvent.normal).eulerAngles;
        particleRotationEuler.z = Random.Range(0, 360);
        particleData[particleDecalDataIndex].rotation = particleRotationEuler;
        particleData[particleDecalDataIndex].size = Random.Range(decalSizeMin, decalSizeMax);
        particleData[particleDecalDataIndex].color = colorGradient.Evaluate(Random.Range(0f, 1f));

        particleDecalDataIndex++;
    }

    void DisplayParticles()
    {
        for (int i = 0; i < particleData.Length; i++)
        {
            particles[i].position = particleData[i].position;
            particles[i].rotation3D = particleData[i].rotation;
            particles[i].startSize = particleData[i].size;
            particles[i].startColor = particleData[i].color;
        }

        decalParticleSystem.SetParticles(particles, particles.Length);
    }
}

public class ParticleLauncher : MonoBehaviour
{

    public ParticleSystem particleLauncher;
    public ParticleSystem splatterParticles;
    public Gradient particleColorGradient;
    public ParticleDecalPool splatDecalPool;

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
            splatDecalPool.ParticleHit(collisionEvents[i], particleColorGradient);
            EmitAtLocation(collisionEvents[i]);
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
        if (Input.GetButton("Fire1"))
        {
            ParticleSystem.MainModule psMain = particleLauncher.main;
            psMain.startColor = particleColorGradient.Evaluate(Random.Range(0f, 1f));
            particleLauncher.Emit(1);
        }

    }
}*/
