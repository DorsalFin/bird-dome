using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBird : Bird
{
    public override void Awake()
    {
        base.Awake();

        // set this simple birds target to the dome which is at 0,0,0
        _target = Vector3.zero;
    }
}
