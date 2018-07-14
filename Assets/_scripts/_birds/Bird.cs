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
    public float moveTime;
    public float moveSteps;

    public int points;

    protected Vector3 _target;
    protected AudioSource _audio;
    protected Animator _anim;

    float _health;
    bool _diverted;
    bool _dead;
    float _clayTimer;
    float _lastStep;


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
        if (GameManager.Instance.dome.IsDead() && !_diverted)
        {
            // divert bird and simulate it flying into the tube
            _target = GameManager.Instance.dome.transform.position + (Vector3.up * 1.5f);
            _diverted = true;
        }

        //float step = speed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, _target, step);
        _clayTimer += Time.deltaTime;
        if (_clayTimer >= moveTime)
            Step();

        Quaternion targetRot = Quaternion.LookRotation(_target - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        // check if we've reached our target
        float dist = Vector3.Distance(transform.position, _target);
        if ((!_diverted && Mathf.Abs(dist) < 0.5f) || (_diverted && Mathf.Abs(dist) < 1.25f))
            TargetReached();
    }

    void Step()
    {
        float distance = GameManager.Instance.ClayMoveAmt * moveSteps;
        Vector3 pos = Vector3.MoveTowards(transform.position, _target, distance);
        pos += Random.insideUnitSphere * GameManager.Instance.ClayJukeRadius;
        transform.position = pos;
        _clayTimer = 0f;
    }

    // override for functionality
    public virtual void TargetReached()
    {
        if (_diverted)
            _target = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Dome"))
        {
            Dome dome = collision.transform.GetComponentInParent<Dome>();
            dome.Hit(damage, collision.contacts[0].point);
            Dead(false);
        }
        else if (collision.transform.CompareTag("Pilot"))
        {
            GameManager.Instance.EndGame();
        }
    }

    public void Hit(float damage, bool givePoints)
    {
        _health = Mathf.Clamp(_health - damage, 0, maxHealth);

        // spawn a feather particle, and set the burst amount of feathers based on dmg dealt
        int feathers = Mathf.Clamp(Mathf.RoundToInt(damage), 1, 10);
        if (_health == 0) feathers = feathers * 2;
        GameObject particle = Instantiate(featherParticle, transform.position, Quaternion.identity);
        particle.GetComponent<ParticleSystem>().emission.SetBurst(0, new ParticleSystem.Burst(0f, feathers));
        
        if (_health == 0)
            Dead(givePoints);
    }

    void Dead(bool awardPoints)
    {
        if (_dead) return;

        _dead = true;
        AudioSource.PlayClipAtPoint(deadClips[Random.Range(0, deadClips.Length)], transform.position);
        GameManager.Instance.BirdDown(awardPoints ? points : 0);
        Destroy(gameObject);
    }
}
