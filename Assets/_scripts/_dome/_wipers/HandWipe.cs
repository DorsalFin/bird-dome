using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class HandWipe : MonoBehaviour
{
    public GameObject clothPrefab;
    public float depth = 1f;

    public bool IsWiping { get { return _wiping; } }

    public bool _wiping;
    GameObject _cloth;


    private void Update()
    {
        if (_wiping)
        {
            if (Input.GetMouseButtonUp(0))
                StopWiping();
            else
            {
                Vector3 point = GetHitPoint(CrossPlatformInputManager.mousePosition);
                Quaternion dir = Quaternion.LookRotation(point - GameManager.Instance.dome.transform.position).normalized;
                _cloth.transform.position = point;
                _cloth.transform.rotation = dir;
            }
        }
    }

    private void OnMouseDown()
    {
        Vector3 point = GetHitPoint(CrossPlatformInputManager.mousePosition);
        Quaternion dir = Quaternion.LookRotation(point - GameManager.Instance.dome.transform.position).normalized;
        _cloth = Instantiate(clothPrefab, point, dir);
        _wiping = true;
    }

    private void OnMouseUp()
    {
        StopWiping();
    }

    void StopWiping()
    {
        _wiping = false;
        Destroy(_cloth);
    }

    Vector3 GetHitPoint(Vector3 mousePos)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // reverse the ray and shoot from outside since we can't hit the inside of a sphere collider
        ray.origin = ray.GetPoint(10f);
        ray.direction = -ray.direction;

        // set the layermask so we only collide with the dome
        int layerMask = LayerMask.GetMask("Dome");

        if (Physics.Raycast(ray, out hit, 10f, layerMask))
            return hit.point;
        else
            return Vector3.zero;
    }
}
