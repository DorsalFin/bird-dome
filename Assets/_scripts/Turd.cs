using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turd : MonoBehaviour
{
    public GameObject splatPrefab;
    public AudioClip[] hitClips;


    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint cp = collision.contacts[0];
        Quaternion lookAt = Quaternion.LookRotation(cp.normal);
        lookAt *= Quaternion.Euler(Vector3.forward * (Random.Range(0, 180)));
        GameObject turd = Instantiate(splatPrefab, cp.point, lookAt, collision.transform);
        turd.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);

        AudioSource.PlayClipAtPoint(hitClips[Random.Range(0, hitClips.Length)], transform.position);
        Destroy(gameObject);
    }
}
