using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public enum UpgradeParent
{
    Dome = 1,
    Pilot = 2,
    Gun = 3,
    Base = 4,
    Camera = 5,
    DomeModel = 6,
    Radar = 7,
}

[System.Serializable]
public class Upgrade
{
    public string uName;
    public string uDescription;
    public GameObject prefab;
    public UpgradeParent parent;
}

public class UpgradeManager : MonoBehaviour
{
    public Upgrade[] upgrades;
    public GameObject upgradeButtonPrefab;
    public Transform upgradeButtonParent;

    public int testUpgradeIdx = 0;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            UpgradeChosen(upgrades[testUpgradeIdx]);
    }

    public void CreateUpgrades(int count)
    {
        var rng = new System.Random();
        var values = Enumerable.Range(0, upgrades.Length).OrderBy(x => rng.Next()).ToArray();

        for (int i = 0; i < count; i++)
        {
            Upgrade upgrade = upgrades[values[i]];
            GameObject upgradeObj = Instantiate(upgradeButtonPrefab, upgradeButtonParent);
            upgradeObj.transform.Find("Text - title").GetComponent<Text>().text = upgrade.uName;
            upgradeObj.transform.Find("Text - description").GetComponent<Text>().text = upgrade.uDescription;
            upgradeObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { UpgradeChosen(upgrade); });
        }
    }

    public void UpgradeChosen(Upgrade upgrade)
    {
        // clear the upgrade buttons
        for (int i = upgradeButtonParent.childCount - 1; i >= 0; i--)
            Destroy(upgradeButtonParent.GetChild(i).gameObject);

        // start the upgrade coroutine so we can watch it applied before progressing
        StartCoroutine(ApplyUpgrade(upgrade));
    }

    IEnumerator ApplyUpgrade(Upgrade upgrade)
    {
        // set parent transform
        Transform parent = upgrade.parent == UpgradeParent.Dome ? GameManager.Instance.dome.transform
            : upgrade.parent == UpgradeParent.Camera ? GameManager.Instance.dome.domeCamera.transform
            : upgrade.parent == UpgradeParent.Base ? GameManager.Instance.dome.baseTransform
            : upgrade.parent == UpgradeParent.DomeModel ? GameManager.Instance.dome.model.transform
            : upgrade.parent == UpgradeParent.Radar ? GameManager.Instance.radar.transform
            : upgrade.parent == UpgradeParent.Gun ? GameManager.Instance.dome.gun.transform
            : null;

        // instantiate the object on the parent
        GameObject upgradeObj = Instantiate(upgrade.prefab, parent);

        // remove from upgrades so we don't get it again
        RemoveAt(ref upgrades, System.Array.IndexOf(upgrades, upgrade));

        Analytics.CustomEvent("upgrade_" + upgrade.uName);

        // TODO: fancy flash or something

        yield return new WaitForSeconds(2f);
        GameManager.Instance.dome.StopUpgrading();
    }

    void RemoveAt<T>(ref T[] arr, int index)
    {
        // replace the element at index with the last element
        arr[index] = arr[arr.Length - 1];
        // and let's decrement Array's size by one
        System.Array.Resize(ref arr, arr.Length - 1);
    }
}
