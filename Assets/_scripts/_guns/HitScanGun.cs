using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanGun : Gun
{
    public float damage;
    public GameObject hitParticle;
    public AudioClip hitClip;


    public override void Fire()
    {
        base.Fire();

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        //int layerMask = (1 << LayerMask.NameToLayer("Default"));

        if (Physics.Raycast(ray, out hit))//, layerMask))
        {
            if (hitParticle)
                Instantiate(hitParticle, hit.point, Quaternion.identity);

            if (hitClip)
                AudioSource.PlayClipAtPoint(hitClip, transform.position);

            if (hit.transform.CompareTag("Bird"))
            {
                Bird bird = hit.transform.GetComponent<Bird>();
                bird.Hit(damage);
            }
        }
    }
}
