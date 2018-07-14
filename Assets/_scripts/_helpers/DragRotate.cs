using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragRotate : MonoBehaviour
{
    public float rotateSpeed = 2f;

    bool _rotating;
    Vector2 _touchPos;

	
	void Update ()
    {
		if (_rotating)
        {
            float xDelta = _touchPos.x - Input.mousePosition.x;
            transform.Rotate(Vector3.up, xDelta);
            _touchPos = Input.mousePosition;
        }
	}

    private void OnMouseDown()
    {
        if (GameManager.Instance.dome.Cleaning)
        {
            _touchPos = Input.mousePosition;
            _rotating = true;
        }
    }

    private void OnMouseUp() {
        _rotating = false;
    }

    private void OnMouseExit() {
        _rotating = false;
    }
}
