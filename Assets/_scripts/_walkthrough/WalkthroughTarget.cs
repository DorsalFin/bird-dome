using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkthroughTarget : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Walkthrough.Instance.ShotTarget();
    }
}
