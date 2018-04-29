using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float maxHealth = 1;
    public float speed;
    public float damage = 1;

    public AudioClip[] cawClips;

    AudioSource _audio;
    float _health;


    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _health = maxHealth;
        _audio.clip = cawClips[Random.Range(0, cawClips.Length)];
        _audio.loop = true;
        _audio.Play();
    }

    private void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, step);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Dome"))
        {
            Dome dome = collision.transform.GetComponentInParent<Dome>();
            dome.Hit(damage, collision.contacts[0].point);
            Dead();
        }
    }

    public void Hit(float damage)
    {
        _health = Mathf.Clamp(_health - damage, 0, maxHealth);
        if (_health == 0)
            Dead();
    }

    void Dead()
    {
        Destroy(gameObject);
    }
}
