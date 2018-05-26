using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    public LayerMask layerMask;

    private void OnTriggerEnter(Collider other)
    {
        bool destroy = layerMask == (layerMask | (1 << other.gameObject.layer));
        if (destroy)
            Destroy(other.gameObject);
    }

}
