using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float shotCooldown;
    public ParticleSystem particle;
    public AudioClip[] clips;

    AudioSource _audio;
    Animator _anim;
    float _lastShot;


    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    void Update ()
    {
		if (Input.GetButton("Fire1"))
        {
            if (Time.time > _lastShot + shotCooldown)
                Fire();
        }
	}

    public virtual void Fire()
    {
        if (_anim)
            _anim.Play("shoot");

        if (particle)
            particle.Play();

        if (_audio && clips.Length > 0)
        {
            _audio.clip = clips[Random.Range(0, clips.Length)];
            _audio.Play();
        }

        _lastShot = Time.time;
    }
}
