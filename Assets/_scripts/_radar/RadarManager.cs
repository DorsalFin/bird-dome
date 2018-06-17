using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

class Target
{
    public GameObject target;
    public Color colour = Color.white;
    public Sprite icon;
    public GameObject blip;
    public LineRenderer line;
    public VectorLine vectorLine;
    public Transform[] upDownArrows;
}

public enum RadarType
{
    SimpleLine = 0,
    Spaceship = 1,
    Compass = 2,
}

public class RadarManager : MonoBehaviour
{
    public RadarType radarType;

    public Transform ui;
    public Transform model;
    public Camera cam;
    public Transform blipParent;
    public GameObject blipPrefab;
    public RawImage compassImage;
    public Gradient gradient;
    public AnimationCurve scaleCurve;

    [HideInInspector]
    public bool identifierInstalled;

    List<Target> _targets = new List<Target>();


    private void Update()
    {
        if (GameManager.Instance.dome.Upgrading)
            return;

        compassImage.uvRect = new Rect(GameManager.Instance.dome.domeCamera.transform.localEulerAngles.y / 360f, 0, 1, 1);

        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            Target target = _targets[i];
            if (target.target == null)
            {
                // this target no longer exists - remove unused elements
                if (target.blip != null) Destroy(target.blip);
                if (target.vectorLine != null) VectorLine.Destroy(ref target.vectorLine);
                _targets.RemoveAt(i);
            }
            else
            {
                if (radarType == RadarType.SimpleLine)
                {
                    if (target.vectorLine == null)
                        target.vectorLine = VectorLine.SetRay3D(Color.white, target.target.transform.position, Vector3.down * 50f);

                    target.vectorLine.points3 = new List<Vector3>() { target.target.transform.position, target.target.transform.position + (Vector3.down * 50f) };
                    target.vectorLine.Draw3D();
                }
                else if (radarType == RadarType.Spaceship)
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
                else if (radarType == RadarType.Compass)
                {
                    Vector3 a = Vector3.zero;
                    Vector3 b = new Vector3(target.target.transform.position.x, 0, target.target.transform.position.z);
                    Vector3 lt = Vector3.MoveTowards(Vector3.zero, GameManager.Instance.dome.domeCamera.transform.forward * 10f, 10f);
                    lt.y = 0;
                    Vector3 f = lt - Vector3.zero;

                    float angle = Vector3.Angle(b - a, f);
                    if (angle < 90)
                    {
                        if (target.blip == null)
                        {
                            target.blip = Instantiate(blipPrefab, blipParent);

                            // if we've got a bird identifier installed, use the correct icon
                            if (identifierInstalled && target.icon != null)
                            {
                                target.blip.GetComponent<Image>().sprite = target.icon;
                                target.blip.GetComponent<Image>().SetNativeSize();
                            }

                            target.upDownArrows = new Transform[2];
                            target.upDownArrows[0] = target.blip.transform.GetChild(0);
                            target.upDownArrows[1] = target.blip.transform.GetChild(1);
                        }

                        float width = compassImage.GetPixelAdjustedRect().width / 2f;
                        float multi = Mathf.InverseLerp(0, 90, angle);
                        float value = AngleDir(f, b - a, Vector3.up);
                        float x = (width * multi) * value;
                        target.blip.transform.localPosition = new Vector2(x, 0);

                        // set colour based on distance
                        float dist = Vector3.Distance(GameManager.Instance.dome.transform.position, target.target.transform.position);
                        float gv = 1 - Mathf.InverseLerp(0, GameManager.Instance.worldRadius, dist);
                        target.blip.GetComponent<Image>().color = gradient.Evaluate(gv);
                        target.blip.transform.localScale = Vector3.one * scaleCurve.Evaluate(gv);

                        // display up or down indicators if neccessary
                        Vector3 toTarget = (target.target.transform.position - GameManager.Instance.dome.transform.position).normalized;
                        float va = Vector3.Angle(GameManager.Instance.dome.domeCamera.transform.forward, toTarget);
                        float sign = Mathf.Sign(Vector3.Dot(GameManager.Instance.dome.domeCamera.transform.right, Vector3.Cross(GameManager.Instance.dome.domeCamera.transform.forward, toTarget)));
                        bool up = va * sign < 0;
                        bool show = va > 17f;
                        target.upDownArrows[0].gameObject.SetActive(up && show);
                        target.upDownArrows[1].gameObject.SetActive(!up && show);

                    }
                    else if (target.blip)
                        Destroy(target.blip);
                    }
            }
        }

        if (radarType == RadarType.Spaceship)
        {
            // rotate the radar camera about center point to match the main camera's angle
            float angle = GameManager.Instance.dome.domeCamera.transform.localEulerAngles.y - cam.transform.localEulerAngles.y;
            cam.transform.RotateAround(model.transform.position, Vector3.up, angle);
        }
    }

    public void AddTarget(GameObject targetObj, Color colour, Sprite icon)
    {
        Target target = new Target();
        target.target = targetObj;
        target.colour = colour;
        target.icon = icon;
        _targets.Add(target);
    }

    //The functions return -1 when the target direction is left, +1 when it is right and 0 if the direction is straight ahead or behind
    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }
}
