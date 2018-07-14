using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Claymation : MonoBehaviour
{
    public int materialIdx;
    public float matShiftDelay;
    public float matMinOffset, matMaxOffset;
    public Mesh[] meshes;

    SkinnedMeshRenderer _renderer;
    //ParticleSystem m_System;
    //ParticleSystem.Particle[] m_Particles;
    Material _mat;
    //Vector4[] m_ParticlePositions = new Vector4[8];


    private void Awake()
    {
        _renderer = GetComponent<SkinnedMeshRenderer>();
        _mat = _renderer.sharedMaterial;
    }

    private void Start() {
        InvokeRepeating("Move", matShiftDelay, matShiftDelay);
    }

    //private void LateUpdate()
    //{
    //    InitializeIfNeeded();

    //    // GetParticles is allocation free because we reuse the m_Particles buffer between updates
    //    int numParticlesAlive = m_System.GetParticles(m_Particles);

    //    // Change only the particles that are alive
    //    for (int i = 0; i < 8; i++)
    //    {
    //        //Grab data from particles and save them into arrays
    //        m_ParticlePositions[i].x = m_Particles[i].position.x;
    //        m_ParticlePositions[i].y = m_Particles[i].position.y;
    //        m_ParticlePositions[i].z = m_Particles[i].position.z;
    //        //NOTE
    //        //m_ParticlePositions is vector4 instead of vector3
    //        //This is only because Unity is expecting a vector4 in SetVectorArray function which we use down below
    //        //Other than that, W component is not used in any way neither here nor in the shader code

    //        //m_ParticleSizes[i] = m_Particles[i].GetCurrentSize(m_System);
    //        //m_ParticleAlpha[i] = m_Particles[i].GetCurrentColor(m_System).a / 255.0f;
    //        //m_ParticleRed[i] = m_Particles[i].GetCurrentColor(m_System).r / 255.0f;
    //    }

    //    if (_meshRenderer)
    //    {
    //        //Now we need to use the data we grabbed from particles and actually transfer it to material
    //        _meshRenderer.sharedMaterial.SetVectorArray("_ParticlePositions", m_ParticlePositions);
    //        //_meshRenderer.sharedMaterial.SetFloatArray("_ParticleSizes", m_ParticleSizes);
    //        //_meshRenderer.sharedMaterial.SetFloatArray("_ParticleAlpha", m_ParticleAlpha);
    //        //_meshRenderer.sharedMaterial.SetFloatArray("_ParticleRed", m_ParticleRed);
    //    }

    //    // Apply the particle changes to the particle system
    //    m_System.SetParticles(m_Particles, numParticlesAlive);

    //    //_meshRenderer.sharedMaterial.SetFloat("_CutoffThreshold", m_CutoffValue.Evaluate(m_System.time / m_System.main.duration));
    //    //_meshRenderer.sharedMaterial.SetFloat("_CollapseStrength", m_CollapseStrength.Evaluate(m_System.time / m_System.main.duration));
    //    //_meshRenderer.sharedMaterial.SetFloat("_EmissionStrength", m_EmissionStrength.Evaluate(m_System.time / m_System.main.duration));
    //}

    //void InitializeIfNeeded()
    //{
    //    if (_meshRenderer == null)
    //        _meshRenderer = GetComponent<MeshRenderer>();

    //    if (_mat == null)
    //        _mat = _meshRenderer.sharedMaterials[materialIdx];

    //    if (m_System == null)
    //        m_System = claymationParticleTransform.GetComponent<ParticleSystem>();

    //    if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
    //        m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    //}

    void Move()
    {
        float offset = Random.Range(matMinOffset, matMaxOffset);
        _mat.SetTextureOffset("_MainTex", new Vector2(offset, offset));

        int idx = System.Array.IndexOf(meshes, _renderer.sharedMesh) + 1;
        if (idx >= meshes.Length)
            idx = 0;
        _renderer.sharedMesh = meshes[idx];
    }
}
