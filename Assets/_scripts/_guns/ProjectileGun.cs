using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : Gun
{
    public GameObject shotPrefab;
    public float shotForce;


    public override void Fire()
    {
        base.Fire();

        foreach (Transform origin in shotOrigins)
        {
            Ray ray = GameManager.Instance.dome.domeCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Bird");
            Vector3 aimPoint = ray.GetPoint(GameManager.Instance.worldRadius);
            if (Physics.Raycast(ray, out hit, GameManager.Instance.worldRadius, layerMask))
                aimPoint = hit.point;
            Quaternion dir = Quaternion.LookRotation(aimPoint - origin.position);

            GameObject proj = Instantiate(shotPrefab, origin.position, dir);
            proj.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * shotForce);
        }
    }
}
