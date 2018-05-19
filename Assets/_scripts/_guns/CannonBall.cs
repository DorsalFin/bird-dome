using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float damage = 1;
    public float gravityStartTime;
    public GameObject explodePrefab;
    public AudioClip[] hitClips;

    float _radius;
    float _timer;
    bool _gravityOn;


    private void Awake()
    {
        _radius = GetComponent<SphereCollider>().radius;
    }

    private void Update()
    {
        if (!_gravityOn && _timer >= gravityStartTime)
        {
            GetComponent<Rigidbody>().useGravity = true;
            _gravityOn = true;
        }
        else
            _timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary")) // the boundary will destroy this object
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
        foreach (Collider hit in hits)
        {
            if (hit.transform == transform) continue;

            if (hit.CompareTag("Bird"))
            {
                Bird bird = hit.GetComponent<Bird>();
                bird.Hit(damage);
            }
        }

        if (explodePrefab)
            Instantiate(explodePrefab, transform.position, Quaternion.identity);

        if (hitClips.Length > 0)
            AudioSource.PlayClipAtPoint(hitClips[Random.Range(0, hitClips.Length)], transform.position);

        Destroy(gameObject);
    }
}
