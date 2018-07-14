using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShock : MonoBehaviour
{
    public float damage = 5f;
    public AudioClip shockClip;
    public GameObject chargedParticle;
    public GameObject shockParticlePrefab;

    bool _used;


    private void OnEnable() {
        GameManager.Instance.birdSpawner.waveEndDelegate += Recharge;
    }

    private void OnDisable() {
        GameManager.Instance.birdSpawner.waveEndDelegate -= Recharge;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_used)
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

            _used = true;
        }
    }

    void Recharge()
    {
        Debug.Log("recharging electric shocker");
        _used = false;

        // turn the charged particle back on
        chargedParticle.SetActive(true);
    }
}
