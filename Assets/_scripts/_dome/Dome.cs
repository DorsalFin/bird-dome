using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using EZCameraShake;

public class Dome : MonoBehaviour
{
    public Pilot pilot;
    public UpgradeManager upgradeManager;
    public Transform baseTransform;
    public GameObject model;
    public GameObject brokenModel;
    public Transform ui;
    public float maxHealth;
    public AudioClip[] hitClips;
    public AudioClip shatterClip;
    public GameObject[] crackPrefabs;
    public CameraFilterPack_Vision_Tunnel visionTunnel;
    public Camera domeCamera;
    public Camera upgradeCamera;
    public Slider healthSlider;
    public Image healthSliderOverlay;
    //public CinemachineVirtualCamera virtualCamera;

    public bool IsDead() { return pilot.dead; }
    public bool CanLook { get { return !Upgrading; } }
    public bool CanShoot { get { return !Upgrading && !Walkthrough.Instance.ShootLocked; } }
    public bool Upgrading {
        get { return _upgrading; }
        set { _upgrading = value; }
    }

    float _health;
    AudioSource _audio;
    Transform _crackParent;
    CameraShaker _cameraShaker;
    bool _broken;
    bool _upgrading;


    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _health = maxHealth;
        _crackParent = new GameObject().transform;
        _crackParent.transform.parent = model.transform;
        _cameraShaker = GetComponentInChildren<CameraShaker>();
    }

    private void Update()
    {
//#if UNITY_EDITOR
//        if (Input.GetKeyDown(KeyCode.K))
//        {
//            if (!_broken)
//                Broken();
//            else if (!pilot.dead)
//                GameManager.Instance.EndGame();
//        }
//#endif
    }

    public void Hit(float damage, Vector3 point)
    {
        ShakeDome();
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
            healthSlider.value = healthSlider.value - 1;
        }
    }

    void Broken()
    {
        if (_broken)
            return;

        _audio.clip = shatterClip;
        _audio.Play();

        // turn off health slider
        healthSlider.gameObject.SetActive(false);

        visionTunnel.enabled = true;
        pilot.StartBreathing();

        // destroy all cracks
        for (int i = _crackParent.childCount - 1; i >= 0; i--)
            Destroy(_crackParent.GetChild(i).gameObject);

        // destroy the dome model itself
        // TODO: a cool destroyed glass thing
        Destroy(model);
        brokenModel.SetActive(true);

        // turn off the radar
        if (GameManager.Instance.radar != null)
            GameManager.Instance.radar.gameObject.SetActive(false);

        _broken = true;
    }

    public void Dead()
    {
        if (pilot.dead)
            return;

        pilot.Dead();

        // destroy all guns
        Gun[] guns = GetComponentsInChildren<Gun>();
        foreach (Gun gun in guns)
            Destroy(gun.gameObject);

        GameManager.Instance.watchCamera.gameObject.SetActive(true);
        GameManager.Instance.birdSpawner.SpawnDeathBirds();
    }

    public void ShakeDome()
    {
        _cameraShaker.ShakeOnce();
    }

    public void StartUpgrading()
    {
        Upgrading = true;

        // set the camera states
        upgradeCamera.gameObject.SetActive(true);
        GameManager.Instance.dome.domeCamera.gameObject.SetActive(false);

        // turn off dome UI things
        ui.gameObject.SetActive(false);
        GameManager.Instance.radar.ui.gameObject.SetActive(false);

        upgradeManager.CreateUpgrades(3);
        //StartCoroutine("UpgradeTimer");
    }

    IEnumerator UpgradeTimer()
    {
        yield return new WaitForSeconds(10f);
        StopUpgrading();
    }

    public void StopUpgrading()
    {
        // set the camera states
        GameManager.Instance.dome.domeCamera.gameObject.SetActive(true);
        upgradeCamera.gameObject.SetActive(false);

        // turn on the dome ui stuff if not broken
        if (!_broken)
        {
            ui.gameObject.SetActive(true);
            GameManager.Instance.radar.ui.gameObject.SetActive(true);
        }

        Upgrading = false;
        GameManager.Instance.birdSpawner.IntroduceWave();
    }
}
