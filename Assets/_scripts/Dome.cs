using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Dome : MonoBehaviour
{
    public GameObject model;
    public float maxHealth;
    public AudioClip[] hitClips;
    public AudioClip shatterClip;
    public GameObject[] crackPrefabs;
    public CinemachineVirtualCamera virtualCamera;

    float _health;
    AudioSource _audio;
    Transform _crackParent;


    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _health = maxHealth;
        _crackParent = new GameObject().transform;
        _crackParent.transform.parent = model.transform;
    }

    public void Hit(float damage, Vector3 point)
    {
        GameManager.Instance.virtualCameraShaker.Shake();
        AudioSource.PlayClipAtPoint(hitClips[Random.Range(0, hitClips.Length)], point);

        _health = Mathf.Clamp(_health - damage, 0, maxHealth);
        if (_health == 0)
            Broken();
        else
        {
            // we only need to add a crack if we're still alive
            Quaternion lookAt = Quaternion.LookRotation(point - model.transform.position);
            lookAt *= Quaternion.Euler(Vector3.forward * (Random.Range(0, 180)));
            GameObject crack = Instantiate(crackPrefabs[Random.Range(0, crackPrefabs.Length)], point, lookAt);
            crack.transform.parent = _crackParent;
        }
    }

    void Broken()
    {
        _audio.clip = shatterClip;
        _audio.Play();

        // destroy all cracks
        for (int i = _crackParent.childCount - 1; i >= 0; i--)
            Destroy(_crackParent.GetChild(i).gameObject);

        // destroy the dome model itself
        // TODO: a cool destroyed glass thing
        Destroy(model);
    }

    public void ShutDown()
    {
        // destroy all guns
        Gun[] guns = GetComponentsInChildren<Gun>();
        foreach (Gun gun in guns)
            Destroy(gun.gameObject);

        virtualCamera.gameObject.SetActive(false);
        GameManager.Instance.watchCamera.gameObject.SetActive(true);
    }
}
