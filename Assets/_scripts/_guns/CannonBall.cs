using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float damage = 1;
    public float gravityStartTime;
    public GameObject explodePrefab;
    public AudioClip[] hitClips;
    public bool usesGravity = true;

    float _radius;
    float _timer;
    bool _gravityOn;


    private void Awake()
    {
        _radius = GetComponent<SphereCollider>().radius;
    }

    private void Update()
    {
        if (usesGravity)
        {
            if (!_gravityOn && _timer >= gravityStartTime)
            {
                GetComponent<Rigidbody>().useGravity = true;
                _gravityOn = true;
            }
            else
                _timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // the boundary will destroy this object, and Upgrade colliders shouldn't block projectiles
        if (other.CompareTag("Boundary") || other.CompareTag("Upgrade"))
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
        foreach (Collider hit in hits)
        {
            if (hit.transform == transform) continue;

            if (hit.CompareTag("Bird"))
            {
                Bird bird = hit.GetComponent<Bird>();
                bird.Hit(damage, true);
            }
        }

        if (explodePrefab)
        {
            Instantiate(explodePrefab, transform.position, Quaternion.LookRotation(transform.position - GameManager.Instance.dome.model.transform.position));
        }

        if (hitClips.Length > 0)
            AudioSource.PlayClipAtPoint(hitClips[Random.Range(0, hitClips.Length)], transform.position);

        Destroy(gameObject);
    }
}
