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
    public AudioClip[] reloadClips;

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
        // record on walkthrough if need be
        if (Walkthrough.Instance.IsRunning)
            Walkthrough.Instance.FillStepBar(0.251f, 1);

        if (_anim)
            _anim.Play("shoot");

        if (particlePrefab != null)
        {
            foreach (Transform origin in shotOrigins)
                Instantiate(particlePrefab, origin.position, Quaternion.identity);
        }

        if (clips.Length > 0)
            AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], transform.position);

        if (_audio && reloadClips.Length > 0)
        {
            _audio.clip = reloadClips[Random.Range(0, reloadClips.Length)];
            _audio.Play();
        }

        _lastShot = Time.time;
    }
}
