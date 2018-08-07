using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShock : MonoBehaviour
{
    public float damage = 5f;
    public float rechargeTime = 10f;
    public AudioClip shockClip;
    public GameObject chargedParticle;
    public GameObject shockParticlePrefab;

    float _timer = 0f;
    bool _charged = true;


    private void Awake() {
        Recharge();
    }

    private void Update()
    {
        if (!_charged)
            _timer -= Time.deltaTime;

        // are we recharging?
        if (_timer <= 0)
            Recharge();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_charged)
            return;

        if (other.tag == "Bird")
        {
            Bird bird = other.GetComponent<Bird>();

            // create a shock particle pointing at the bird
            Vector3 dir = bird.transform.position - transform.position;
            Quaternion lookDir = Quaternion.LookRotation(dir);
            lookDir *= Quaternion.Euler(-90f, 0, 0);
            Instantiate(shockParticlePrefab, transform.position, lookDir);

            // play shock audio
            AudioSource.PlayClipAtPoint(shockClip, bird.transform.position);

            // damage the bird
            bird.Hit(damage, false);

            // disable the charged particle for now
            chargedParticle.SetActive(false);

            // reset timer
            _timer = rechargeTime;
            _charged = false;
        }
    }

    void Recharge()
    {
        _charged = true;
        chargedParticle.SetActive(true);
    }
}
