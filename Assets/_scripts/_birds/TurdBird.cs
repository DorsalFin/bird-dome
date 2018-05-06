using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurdBird : Bird
{
    public float height;
    public GameObject turdPrefab;
    public Transform turdOrigin;
    public float turdDistanceFromZero = 5;
    public float turdMinDelay, turdMaxDelay;

    float _lastTurd;
    float _delay;


    public override void Awake()
    {
        base.Awake();

        // the turd bird flies in a straight line forward until reaching the other side of the dome
        _target = new Vector3(-transform.position.x, height, -transform.position.z);
        _delay = Random.Range(turdMinDelay, turdMaxDelay);
    }

    public override void Update()
    {
        base.Update();

        if (Time.time > _lastTurd + _delay)
        {
            // if we're in the turd zone, let fly!
            float fromCenter = Mathf.Abs(Vector3.Distance(new Vector3(0, height, 0), transform.position));
            if (fromCenter <= turdDistanceFromZero)
                Turd();
        }
    }

    public override void TargetReached()
    {
        Destroy(gameObject);
    }

    void Turd()
    {
        Instantiate(turdPrefab, turdOrigin.position, Quaternion.identity);

        _delay = Random.Range(turdMinDelay, turdMaxDelay);
        _lastTurd = Time.time;
    }
}
