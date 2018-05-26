using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float maxHealth = 1;
    public float speed;
    public float rotationSpeed = 2f;
    public float damage = 1;
    public AudioClip[] cawClips;
    public AudioClip[] deadClips;
    public GameObject featherParticle;

    protected Vector3 _target;
    protected AudioSource _audio;
    protected Animator _anim;

    float _health;


    public virtual void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();
        _health = maxHealth;
        _audio.clip = cawClips[Random.Range(0, cawClips.Length)];
        _audio.loop = true;
        _audio.Play();
    }

    public virtual void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _target, step);
        Quaternion targetRot = Quaternion.LookRotation(_target - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);


        // check if we've reached our target
        float dist = Vector3.Distance(transform.position, _target);
        if (Mathf.Abs(dist) < 0.5f)
            TargetReached();
    }

    // override for functionality
    public virtual void TargetReached() { }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Dome"))
        {
            Dome dome = collision.transform.GetComponentInParent<Dome>();
            dome.Hit(damage, collision.contacts[0].point);
            Dead();
        }
        else if (collision.transform.CompareTag("Pilot"))
        {
            GameManager.Instance.EndGame();
        }
    }

    public void Hit(float damage)
    {
        _health = Mathf.Clamp(_health - damage, 0, maxHealth);

        // spawn a feather particle, and set the burst amount of feathers based on dmg dealt
        int feathers = Mathf.Clamp(Mathf.RoundToInt(damage), 1, 10);
        if (_health == 0) feathers = feathers * 2;
        GameObject particle = Instantiate(featherParticle, transform.position, Quaternion.identity);
        particle.GetComponent<ParticleSystem>().emission.SetBurst(0, new ParticleSystem.Burst(0f, feathers));
        
        if (_health == 0)
            Dead();
    }

    void Dead()
    {
        AudioSource.PlayClipAtPoint(deadClips[Random.Range(0, deadClips.Length)], transform.position);
        Destroy(gameObject);
    }
}
