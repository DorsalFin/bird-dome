using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSpeed : MonoBehaviour
{
    public float speedMultiplier = 0.5f;
    public AudioClip reloadClip;


    private void Start()
    {
        GameManager.Instance.dome.gun.shotCooldown *= speedMultiplier;
        GameManager.Instance.dome.gun.reloadClips = new AudioClip[] { reloadClip };
    }
}
