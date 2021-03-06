﻿using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[AddComponentMenu("Camera/Simple Smooth Mouse Look ")]
public class SimpleSmoothMouseLook : MonoBehaviour
{
    public float yClampMin = 90;
    public float yClampMax = 180;
    public bool lockCursor;
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    // Assign this if there's a parent object controlling motion, such as a Character Controller.
    // Yaw rotation will affect this object instead of the camera if set.
    public GameObject characterBody;

    Dome _dome;
    AudioSource _audio;
    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;


    private void Awake()
    {
        _dome = GetComponentInParent<Dome>();
        _audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody)
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
    }

    void Update()
    {
        if (!_dome.CanLook)
            return;

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(CrossPlatformInputManager.GetAxisRaw("Mouse X"), CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // clamp y values relative to initial orientation
        _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, yClampMin, yClampMax);

        if (mouseDelta.magnitude > 0.1f)
        {
            // record on walkthrough if need be
            if (Walkthrough.Instance.IsRunning)
                Walkthrough.Instance.FillStepBar(0.0075f, 0);

            _dome.SetWiggle(1);

            _audio.volume = Mathf.InverseLerp(0.1f, 5f, mouseDelta.magnitude);
            _audio.pitch = (Mathf.InverseLerp(0.1f, 5f, mouseDelta.magnitude) / 2f) + 0.75f;
            _audio.Play();
        }
        else
            _dome.SetWiggle(0);

        transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        // If there's a character body that acts as a parent to the camera
        if (characterBody)
        {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
            characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
        }
        else
        {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
}