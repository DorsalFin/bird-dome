using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBird : Bird
{
    public override void Awake()
    {
        base.Awake();

        // set this simple birds target to the dome
        _target = GameManager.Instance.dome.transform.position;
    }

    public override void Update()
    {
        base.Update();
    }
}
