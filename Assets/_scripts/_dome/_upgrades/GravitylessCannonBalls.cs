using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitylessCannonBalls : MonoBehaviour
{
    public GameObject gravitylessCannonBallPrefab;


    private void Start()
    {
        ProjectileGun cannon = GameManager.Instance.dome.gun as ProjectileGun;
        cannon.shotPrefab = gravitylessCannonBallPrefab;
    }
}
