using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdIdentifier : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.radar.identifierInstalled = true;
    }
}
