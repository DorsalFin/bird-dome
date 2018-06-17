using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGlass : MonoBehaviour
{
    public Material domeMaterial;
    public Sprite healthOverlaySprite;


    private void Start()
    {
        // visually set the crystal glass
        Material[] mats = new Material[2];
        mats[0] = domeMaterial;
        mats[1] = GameManager.Instance.dome.model.GetComponent<MeshRenderer>().materials[1];
        GameManager.Instance.dome.model.GetComponent<MeshRenderer>().materials = mats;

        // update the dome stats
        GameManager.Instance.dome.healthSlider.maxValue = 4;
        GameManager.Instance.dome.maxHealth += 1;
        GameManager.Instance.dome.healthSlider.value += 1;
        GameManager.Instance.dome.healthSliderOverlay.sprite = healthOverlaySprite;
    }
}
