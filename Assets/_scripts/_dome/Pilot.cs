using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilot : MonoBehaviour
{
    public AudioClip breathingClip;
    public AudioClip deadClip;
    public bool dead;

    AudioSource _audio;


    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void StartBreathing()
    {
        _audio.clip = breathingClip;
        _audio.loop = true;
        _audio.Play();
    }

    public void Dead()
    {
        dead = true;
        _audio.clip = deadClip;
        _audio.loop = false;
        _audio.Play();
    }
}
