using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Gun : MonoBehaviour
{
    public float shotCooldown;
    public ParticleSystem particlePrefab;
    public Transform[] shotOrigins;
    public AudioClip[] clips;

    Dome _dome;
    AudioSource _audio;
    Animator _anim;
    float _lastShot;


    private void Awake()
    {
        _dome = GetComponentInParent<Dome>();
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    void Update ()
    {
        if (!_dome.CanShoot)
            return;

		if (CrossPlatformInputManager.GetButton("Fire1"))
        {
            if (Time.time > _lastShot + shotCooldown)
                Fire();
        }
	}

    public virtual void Fire()
    {
        if (_anim)
            _anim.Play("shoot");

        if (particlePrefab != null)
        {
            foreach (Transform origin in shotOrigins)
                Instantiate(particlePrefab, origin.position, Quaternion.identity);
        }

        if (_audio && clips.Length > 0)
        {
            _audio.clip = clips[Random.Range(0, clips.Length)];
            _audio.Play();
        }

        _lastShot = Time.time;
    }
}
