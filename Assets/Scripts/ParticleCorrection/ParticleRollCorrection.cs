using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleRollCorrection : MonoBehaviour {

    private float lastRotation = 0f;

    private ParticleSystem ps = new ParticleSystem();
    private ParticleSystem.Particle[] particles;


	// Use this for initialization
	void Start () {
     
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (!Camera.main)
            return;

        InitializeIfNeeded();


        float currentRotation = Camera.main.transform.rotation.eulerAngles.z;
        if (lastRotation == currentRotation)
            return;

        float rotationDifference = currentRotation - lastRotation;

        int num = ps.GetParticles((ParticleSystem.Particle[])particles);
        for (int i = 0; i < num; i++)
			particles[i].rotation += rotationDifference;

		ps.SetParticles(particles, num);
        //particleSystem.startRotation += rotationDifference * Mathf.Deg2Rad;

        lastRotation = currentRotation;
    }

    void InitializeIfNeeded()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }
}
