using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dome : MonoBehaviour
{
    public GameObject model;
    public float maxHealth;
    public AudioClip[] hitClips;
    public AudioClip shatterClip;

    float _health;
    AudioSource _audio;


    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _health = maxHealth;
    }

    public void Hit(float damage, Vector3 point)
    {
        GameManager.Instance.virtualCameraShaker.Shake();
        AudioSource.PlayClipAtPoint(hitClips[Random.Range(0, hitClips.Length)], point);

        _health = Mathf.Clamp(_health - damage, 0, maxHealth);
        if (_health == 0)
            Dead();
    }

    void Dead()
    {
        _audio.clip = shatterClip;
        _audio.Play();

        Destroy(model);
    }
}
