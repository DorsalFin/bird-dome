using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MenuClickToFocus : MonoBehaviour
{
    public CinemachineVirtualCamera targetCamera;
    public string eventName = string.Empty;


    private void OnMouseDown()
    {
        MenuManager.Instance.FocusCamera(targetCamera, gameObject);

        // if there's an event when pressing this object, send it to MenuManager
        if (eventName != string.Empty)
            MenuManager.Instance.SendMessage(eventName);
    }
}
