using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwoopBird : Bird
{
    public float height;
    public float swoopSpeedMultiplier = 2f;
    public AudioClip[] swoopClips;

    bool _swooping;


    public override void Awake()
    {
        base.Awake();

        // swoopers have 2 movement lines - 
        // 1) toward the center, slow and high in a straight line
        // 2) straight down toward the dome, at high speed
        // it converts to 2) when it's close enough
        Vector3 target = new Vector3(-transform.position.x, height, -transform.position.z);
        _target = Vector3.MoveTowards(transform.position, target, GameManager.Instance.worldRadius * 0.85f);
    }

    public override void TargetReached()
    {
        base.TargetReached();

        if (!_swooping && !GameManager.Instance.dome.pilot.dead)
        {
            // set the target to be the dome
            _target = GameManager.Instance.dome.transform.position;
            // increase speed
            speed = speed * swoopSpeedMultiplier;
            // switch to the swoop sound
            _audio.clip = swoopClips[Random.Range(0, swoopClips.Length)];
            // switch to the swoop animation
            _anim.Play("swoop");

            _swooping = true;
        }
    }
}
