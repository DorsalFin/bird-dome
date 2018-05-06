using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BirdType
{
    public GameObject prefab;
    public AnimationCurve curve;
    public float minCooldown;
    public float lastSpawn;
}

public class BirdSpawner : MonoBehaviour
{
    public float checkCooldown = 1f;
    public float initialChanceMultiplier = 0.5f;
    public float multiGrowthAmount = 0.25f;
    public float multiIncreaseCooldown = 30f;
    public BirdType[] birdTypes;

    /// <summary>
    /// The multiplier slowly rises as you play, in accordance with multiGrowthRate.
    /// It multiplies the roll when checking if a bird spawns (higher chance as it rises).
    /// It also divides the cooldown on each bird so they can spawn closer together as it rises.
    /// </summary>
    float _multi;
    float _lastCheck;
    float _lastMultiIncrease;


    private void Awake()
    {
        _multi = initialChanceMultiplier;
    }

    private void Update()
    {
        if (Time.time > _lastCheck + checkCooldown)
            RunChecks();

        if (Time.time > _lastMultiIncrease + multiIncreaseCooldown)
            GrowMultiplier(multiGrowthAmount);
    }

    void RunChecks()
    {
        foreach (BirdType birdType in birdTypes)
        {
            float roll = Random.Range(0, 101) * _multi;
            float value = birdType.curve.Evaluate(0) * 100f;//GameManager.Instance.day.currentTimeOfDay);
            if (roll <= value)
                SpawnBird(birdType);
        }

        _lastCheck = Time.time;
    }

    void SpawnBird(BirdType birdType)
    {
        // default check if it's too soon
        float adjustedCooldown = birdType.minCooldown / _multi;
        if (Time.time < birdType.lastSpawn + adjustedCooldown)
            return;

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (birdType.prefab.name == "SimpleBird")
        {
            spawnPos = Random.onUnitSphere * GameManager.Instance.worldRadius;
            spawnPos.y = Mathf.Abs(spawnPos.y);
            spawnRot = Quaternion.LookRotation(Vector3.zero - spawnPos, Vector3.up);
        }
        else if (birdType.prefab.name == "TurdBird")
        {
            float height = birdType.prefab.GetComponent<TurdBird>().height;
            Vector3 temp = Random.insideUnitCircle.normalized * GameManager.Instance.worldRadius;
            spawnPos = new Vector3(temp.x, height, temp.y);
            spawnRot = Quaternion.LookRotation(new Vector3(0, height, 0) - spawnPos, Vector3.up);
        }

        Instantiate(birdType.prefab, spawnPos, spawnRot);
        birdType.lastSpawn = Time.time;
    }

    void GrowMultiplier(float amt)
    {
        _multi += amt;
        Debug.Log("multiplier increased to " + _multi);
        _lastMultiIncrease = Time.time;
    }
}
