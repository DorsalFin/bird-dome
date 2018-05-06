using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class MeltingObject : MonoBehaviour {
	public MeshRenderer m_MeltingObjectRenderer;
	public ParticleSystemRenderer m_ParticlesRenderer;
	[Space(20)]
	public AnimationCurve m_CutoffValue = AnimationCurve.EaseInOut (0, 1, 1, 0);
	public AnimationCurve m_CollapseStrength = AnimationCurve.EaseInOut (0, 1, 1, 0);
	public AnimationCurve m_EmissionStrength = AnimationCurve.EaseInOut (0, 0.5f, 1, 1);
	private Vector4[] m_ParticlePositions = new Vector4[8];
	private float[] m_ParticleSizes = new float[8];
	private float[] m_ParticleAlpha = new float[8];
	private float[] m_ParticleRed = new float[8];

	ParticleSystem m_System;
	ParticleSystem.Particle[] m_Particles;

	private void LateUpdate()
	{
		InitializeIfNeeded();

		// GetParticles is allocation free because we reuse the m_Particles buffer between updates
		int numParticlesAlive = m_System.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < 8; i++)
		{
			//Grab data from particles and save them into arrays
			m_ParticlePositions [i].x = m_Particles [i].position.x;
			m_ParticlePositions [i].y = m_Particles [i].position.y;
			m_ParticlePositions [i].z = m_Particles [i].position.z;
			//NOTE
			//m_ParticlePositions is vector4 instead of vector3
			//This is only because Unity is expecting a vector4 in SetVectorArray function which we use down below
			//Other than that, W component is not used in any way neither here nor in the shader code

			m_ParticleSizes [i] = m_Particles [i].GetCurrentSize(m_System);
			m_ParticleAlpha [i] = m_Particles [i].GetCurrentColor(m_System).a / 255.0f;
			m_ParticleRed [i] = m_Particles [i].GetCurrentColor(m_System).r / 255.0f;
		}

		if (m_MeltingObjectRenderer)
		{
			//Now we need to use the data we grabbed from particles and actually transfer it to material
			m_MeltingObjectRenderer.sharedMaterial.SetVectorArray ("_ParticlePositions", m_ParticlePositions);
			m_MeltingObjectRenderer.sharedMaterial.SetFloatArray ("_ParticleSizes", m_ParticleSizes);
			m_MeltingObjectRenderer.sharedMaterial.SetFloatArray ("_ParticleAlpha", m_ParticleAlpha);
			m_MeltingObjectRenderer.sharedMaterial.SetFloatArray ("_ParticleRed", m_ParticleRed);
		}

		// Apply the particle changes to the particle system
		m_System.SetParticles(m_Particles, numParticlesAlive);


		m_MeltingObjectRenderer.sharedMaterial.SetFloat ("_CutoffThreshold", m_CutoffValue.Evaluate (m_System.time / m_System.main.duration));
		m_MeltingObjectRenderer.sharedMaterial.SetFloat ("_CollapseStrength", m_CollapseStrength.Evaluate (m_System.time / m_System.main.duration));
		m_MeltingObjectRenderer.sharedMaterial.SetFloat ("_EmissionStrength", m_EmissionStrength.Evaluate (m_System.time / m_System.main.duration));
	}

	void InitializeIfNeeded()
	{
		if (m_System == null)
			m_System = GetComponent<ParticleSystem>();

		if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
			m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
	}
}
