using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float maxHealth = 1;
    public float speed;
    public float damage = 1;
    public AudioClip[] cawClips;

    protected Vector3 _target;

    AudioSource _audio;
    float _health;


    public virtual void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _health = maxHealth;
        _audio.clip = cawClips[Random.Range(0, cawClips.Length)];
        _audio.loop = true;
        _audio.Play();
    }

    public virtual void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _target, step);

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
        if (_health == 0)
            Dead();
    }

    void Dead()
    {
        Destroy(gameObject);
    }
}
