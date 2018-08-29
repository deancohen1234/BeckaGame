using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMove : MonoBehaviour {

    public ParticleSystem m_System;
    public ParticleSystem.Particle[] m_Particles;
    public Transform m_Destination;

    public float m_MoveSpeed = 4f;

    public bool m_ParticlesAreConverging;
	// Use this for initialization
	void Start ()
    {
		
	}

    void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    }

    // Update is called once per frame
    void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Z))
        {
            m_ParticlesAreConverging = true;
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            m_ParticlesAreConverging = false;
        }

        if (m_ParticlesAreConverging)
        {
            InitializeIfNeeded();

            int numParticlesAlive = m_System.GetParticles(m_Particles);
            Debug.Log("Particles Moving: " + numParticlesAlive);


            // Change only the particles that are alive
            for (int i = 0; i < numParticlesAlive; i++)
            {
                Vector3 newPosition = Vector3.Lerp(
                    m_Particles[i].position,
                    m_System.transform.InverseTransformPoint(m_Destination.position),
                    Time.deltaTime * m_MoveSpeed);

                m_Particles[i].position = newPosition;
            }

            m_System.SetParticles(m_Particles, numParticlesAlive);
        }
    }


}
