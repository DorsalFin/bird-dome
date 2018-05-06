using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float worldRadius = 50f;
    public Dome dome;
    public UIManager ui;
    public DayNightManager day;
    public RadarManager radar;
    public VirtualCameraShaker virtualCameraShaker;
    public CinemachineVirtualCamera watchCamera;

    public bool IsPlaying() { return _playing; }
    bool _playing;


    private void Awake()
    {
        Instance = this;
        _playing = true;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
#endif
    }

    public void EndGame()
    {
        dome.ShutDown();
        ui.GameEnded();
        _playing = false;
    }
}
