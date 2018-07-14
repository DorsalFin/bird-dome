using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public AudioClip pointsClip;


    public void WaveCompleteAnimEnd() {
        GameManager.Instance.dome.StartUpgrading();
    }

    public void AddPoints(int points)
    {
        GameManager.Instance.AddPoints(points);
        AudioSource.PlayClipAtPoint(pointsClip, GameManager.Instance.dome.transform.position);
    }
}
