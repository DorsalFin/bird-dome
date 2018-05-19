using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraShaker : MonoBehaviour
{
    public AnimationCurve curveShake;
    public float shakeTime = 1f;
    CinemachineVirtualCamera _vcam;
    CinemachineBasicMultiChannelPerlin _noise;

    private void Awake()
    {
        _vcam = GetComponent<CinemachineVirtualCamera>();
        _noise = _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake()
    {
        StopCoroutine("ShakeItBaby");
        StartCoroutine("ShakeItBaby");
    }

    IEnumerator ShakeItBaby()
    {
        float timer = 0f;
        while (timer < shakeTime)
        {
            timer += Time.deltaTime;
            _noise.m_AmplitudeGain = curveShake.Evaluate(timer);
            yield return null;
        }
    }
}