﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Apply to anything that should show on the radar. This gameobject will
/// automatically register itself to RadarManager.cs
/// </summary>
public class MarkOnRadar : MonoBehaviour
{
    public Color colour;


	void Start ()
    {
        GameManager.Instance.radar.AddTarget(gameObject, colour);
	}
}
