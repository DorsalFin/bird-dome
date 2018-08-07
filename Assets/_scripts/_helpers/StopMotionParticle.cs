using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class StopMotionParticle : MonoBehaviour
{
    // the particle will update it's position updateEvery seconds.
    public float updateEvery = 0.1f;
    ParticleSystem _particle;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
        InvokeRepeating("UpdateTime", 0f, updateEvery);
    }

    void UpdateTime()
    {
        _particle.Simulate(updateEvery, true, false);

        // if this particle has finished, destroy it
        if (_particle.time >= _particle.main.duration && !_particle.main.loop)
            Destroy(gameObject);
    }
}
