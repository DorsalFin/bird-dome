using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilot : MonoBehaviour
{
    public AudioClip breathingClip;
    public AudioClip deadClip;
    public GameObject model;
    public bool dead;

    AudioSource _audio;


    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.Instance.dome.CanLook)
        {
            // the pilot should rotate with the guns only on the y axis
            model.transform.eulerAngles = new Vector3(
                model.transform.eulerAngles.x,
                GameManager.Instance.dome.domeCamera.transform.eulerAngles.y,
                model.transform.eulerAngles.z);
        }
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
