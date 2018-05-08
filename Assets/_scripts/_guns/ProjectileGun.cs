using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : Gun
{
    public Transform[] shotOrigins;
    public GameObject shotPrefab;
    public float shotForce;


    public override void Fire()
    {
        base.Fire();

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 lookPoint = ray.GetPoint(30);

        foreach (Transform origin in shotOrigins)
        {
            GameObject proj = Instantiate(shotPrefab, origin.position, Quaternion.LookRotation(lookPoint - transform.position));
            proj.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * shotForce);
        }
    }
}
