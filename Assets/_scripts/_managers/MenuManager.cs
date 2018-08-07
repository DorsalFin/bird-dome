using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera[] objectCameras;

    public GameObject pilotObject;
    public GameObject domeObject;
    public GameObject computerObject;
    public GameObject dollyTarget;

    GameObject _focused = null;
    float _initialPilotLocalX;
    float _initialComputerLocalX;


    private void Awake()
    {
        Instance = this;
        _initialPilotLocalX = pilotObject.transform.localPosition.x;
        _initialComputerLocalX = computerObject.transform.localPosition.x;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            BackToMainCamera();
    }

    public void FocusCamera(CinemachineVirtualCamera targetCam, GameObject focusedObject)
    {
        targetCam.gameObject.SetActive(true);
        _focused = focusedObject;
    }

    public void BackToMainCamera()
    {
        foreach (CinemachineVirtualCamera cmvc in objectCameras)
            cmvc.gameObject.SetActive(false);

        if (_focused != null)
        {
            if (_focused == domeObject)
            {
                LeanTween.moveLocalX(pilotObject, _initialPilotLocalX, 0.5f).setEase(LeanTweenType.easeOutCirc);
                LeanTween.moveLocalX(computerObject, _initialComputerLocalX, 0.5f).setEase(LeanTweenType.easeOutCirc);
            }
        }

        _focused = null;
    }

    public void FocusDome()
    {
        // when focusing the dome, the computer and pilot are pulled off the sides
        // to make room for the left and right dome options (upgrade and customize)
        LeanTween.moveLocalX(pilotObject, _initialPilotLocalX + 5f, 0.5f).setEase(LeanTweenType.easeInCirc);
        LeanTween.moveLocalX(computerObject, _initialComputerLocalX - 5f, 0.5f).setEase(LeanTweenType.easeInCirc);
    }

    public void StartDolly() {
        dollyTarget.SetActive(true);
    }
}
