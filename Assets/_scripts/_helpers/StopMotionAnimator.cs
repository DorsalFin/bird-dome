using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StopMotionAnimator : MonoBehaviour
{
    // the particle will update it's position updateEvery seconds.
    public float updateEvery = 0.1f;
    Animator _anim;
    float _currentTotal = 0f;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        InvokeRepeating("UpdateTime", 0f, updateEvery);
    }

    void UpdateTime()
    {
        // Fetch the current Animation clip information for the base layer
        AnimatorClipInfo[] clipInfo = _anim.GetCurrentAnimatorClipInfo(0);
        float animLength = clipInfo[0].clip.length;
        string animName = clipInfo[0].clip.name;

        // calculate our noew normalized time
        float normalizedTime = _currentTotal / animLength;

        // play and stop the same animation at the newly calculated time
        _anim.Play(animName, 0, normalizedTime);
        _anim.speed = 0;

        // update our running total, and reset if we've exceeded the length of this animation
        _currentTotal += updateEvery;
        if (_currentTotal > animLength)
            _currentTotal = 0f;
    }
}
