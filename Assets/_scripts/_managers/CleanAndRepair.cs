using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public enum CleanTool
{
    Cloth = 0,
    Hammer = 1,
}

public class CleanAndRepair : MonoBehaviour
{
    public CleanTool cleanTool = CleanTool.Cloth;
    public GameObject cleanRepairPanel;
    public LayerMask layerMask;
    public GameObject selectionNoodles;
    public Button clothButton, hammerButton;
    public GameObject clothPrefab, hammerPrefab;
    public float toolOffset = 1f;

    public bool Cleaning { get; set; }

    GameObject _tool;


    private void Update()
    {
        if (Cleaning)
        {
            if (CrossPlatformInputManager.GetButton("Fire1"))
            {
                Ray ray = GameManager.Instance.dome.upgradeCamera.ScreenPointToRay(CrossPlatformInputManager.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Dome"))
                    {
                        Vector3 toCamera = (hit.point - GameManager.Instance.dome.upgradeCamera.transform.position).normalized * -toolOffset;
                        Vector3 pos = hit.point + toCamera;
                        Quaternion rot = Quaternion.LookRotation(hit.normal);

                        if (_tool == null)
                            CreateTool(pos, rot);
                        else
                        {
                            _tool.transform.position = pos;
                            _tool.transform.rotation = rot;
                        }
                    }
                }
            }
            else if (CrossPlatformInputManager.GetButtonUp("Fire1"))
            {
                if (_tool != null)
                    Destroy(_tool);
            }
        }
    }

    void CreateTool(Vector3 pos, Quaternion rot) {
        _tool = Instantiate(cleanTool == CleanTool.Cloth ? clothPrefab : hammerPrefab, pos, rot);
    }

    public void ToggleCleaning(bool isCleaning)
    {
        Cleaning = isCleaning;
        cleanRepairPanel.SetActive(isCleaning);
        if (isCleaning)
            StartCoroutine("CleanTimer");
    }

    IEnumerator CleanTimer()
    {
        yield return new WaitForSeconds(10f);
        StopCleaning();        
    }

    public void StopCleaning()
    {
        StopCoroutine("CleanTimer");
        if (_tool != null)
            Destroy(_tool);
        Cleaning = false;
        ToggleCleaning(false);
        GameManager.Instance.dome.UpgradingAndCleaningDone();
    }

    public void SelectCleanButton(Button button)
    {
        selectionNoodles.transform.parent = button.transform;
        selectionNoodles.transform.localPosition = Vector2.zero;
        cleanTool = button == clothButton ? CleanTool.Cloth : CleanTool.Hammer;
    }
}
