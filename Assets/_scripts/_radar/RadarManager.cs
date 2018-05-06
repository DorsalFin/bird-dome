using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Target
{
    public GameObject target;
    public Color colour = Color.white;
    public GameObject blip;
    public LineRenderer line;
}

public class RadarManager : MonoBehaviour
{
    public Transform model;
    public Camera cam;
    public Transform blipParent;
    public GameObject blipPrefab;

    List<Target> _targets = new List<Target>();


    private void Update()
    {
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            Target target = _targets[i];
            if (target.target == null)
            {
                // this target no longer exists - remove it
                if (target.blip != null)
                    Destroy(target.blip);
                _targets.RemoveAt(i);
            }
            else
            {
                // calculate the scaled position relative to the size of our radar
                Vector3 radarPos = new Vector3(
                     model.localScale.x / (GameManager.Instance.worldRadius / target.target.transform.position.x),
                     model.localScale.y / (GameManager.Instance.worldRadius / target.target.transform.position.y),
                     model.localScale.z / (GameManager.Instance.worldRadius / target.target.transform.position.z)
                     );

                if (target.blip == null)
                {
                    target.blip = Instantiate(blipPrefab, blipParent);
                    target.blip.GetComponent<Renderer>().material.color = target.colour;
                    target.line = target.blip.GetComponent<LineRenderer>();
                    target.line.material.color = target.colour;
                }

                // set the blips position
                target.blip.transform.localPosition = radarPos;

                // adjust the length of the line renderer
                float lineY = (radarPos.y * GameManager.Instance.worldRadius) / 2f;
                target.line.SetPosition(1, new Vector3(0f, -lineY, 0f));
            }
        }

        // rotate the radar camera about center point to match the main camera's angle
        float angle = Camera.main.transform.localEulerAngles.y - cam.transform.localEulerAngles.y;
        cam.transform.RotateAround(model.transform.position, Vector3.up, angle);
    }

    public void AddTarget(GameObject targetObj, Color colour)
    {
        Target target = new Target();
        target.target = targetObj;
        target.colour = colour;
        _targets.Add(target);
    }
}
