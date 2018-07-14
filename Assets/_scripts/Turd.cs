using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turd : MonoBehaviour
{
    public GameObject splatPrefab;
    public AudioClip[] hitClips;
    public float offset = 1f;


    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint cp = collision.contacts[0];
        Vector3 point = cp.point;
        //Quaternion lookAt = Quaternion.LookRotation(cp.normal);
        //lookAt *= Quaternion.Euler(Vector3.forward * (Random.Range(0, 180)));
        //Vector3 spawnPoint = cp.point;// + (cp.normal.normalized * offset);
        //GameObject turd = Instantiate(splatPrefab, spawnPoint, lookAt, GameManager.Instance.dome.TurdParent);
        ////turd.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);

        Vector3 dir = point - GameManager.Instance.dome.model.transform.position;
        Vector3 hitPoint = GameManager.Instance.dome.model.transform.position + (dir.normalized * 1.01f);
        Quaternion lookAt = Quaternion.LookRotation(point - GameManager.Instance.dome.model.transform.position);
        lookAt *= Quaternion.Euler(Vector3.forward * (Random.Range(0, 180))); // rotate it around it's x axis randomly so they don't all look the same
        GameObject crack = Instantiate(splatPrefab, hitPoint, lookAt, GameManager.Instance.dome.TurdParent);
        //crack.transform.parent = _crackParent;

        AudioSource.PlayClipAtPoint(hitClips[Random.Range(0, hitClips.Length)], transform.position);
        Destroy(gameObject);
    }
}
