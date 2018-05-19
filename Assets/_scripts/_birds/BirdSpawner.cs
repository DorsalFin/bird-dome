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
    public BirdType[] birdTypes;

    public float checkCooldown = 1f;
    public float initialChanceMultiplier = 0.5f;
    public float multiGrowthAmount = 0.25f;
    public float multiIncreaseCooldown = 30f;

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

#region waves
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;


//[System.Serializable]
//public class BirdType
//{
//    public GameObject prefab;
//    public AnimationCurve curve;
//    public float minCooldown;
//    public float lastSpawn;
//}

//[System.Serializable]
//public class Wave
//{
//    public int num;
//    public bool isActive;
//    public float checkCooldown;
//    public float initialChanceMultiplier = 0.5f;
//    public float multiGrowthAmount = 0.25f;
//    public float multiIncreaseCooldown = 30f;

//    /// <summary>
//    /// The multiplier slowly rises as you play, in accordance with multiGrowthRate.
//    /// It multiplies the roll when checking if a bird spawns (higher chance as it rises).
//    /// It also divides the cooldown on each bird so they can spawn closer together as it rises.
//    /// </summary>
//    public float multi;
//    public float lastCheck;
//    public float lastMultiIncrease;
//}

//public class BirdSpawner : MonoBehaviour
//{
//    public BirdType[] birdTypes;

//    public float waveTime;
//    public Text waveTitleText;

//    Wave _wave;

//    float _waveTimer;

//    private void Awake()
//    {
//        //_multi = initialChanceMultiplier;
//        //_waveNum = 1;
//    }

//    private void Update()
//    {
//        if (_wave != null && _wave.isActive)
//        {
//            _waveTimer += Time.deltaTime;

//            if (_waveTimer >= waveTime)
//                EndWave();

//            if (_waveTimer > _wave.lastCheck + _wave.checkCooldown)
//                RunChecks();

//            //if (_waveTimer > _lastMultiIncrease + _wave.multiIncreaseCooldown)
//            //    GrowMultiplier(_wave.multiGrowthAmount);
//        }
//    }

//    public void StartGame()
//    {
//        Wave _wave = GenerateWave(1);
//    }

//    Wave GenerateWave(int num)
//    {
//        Wave wave = new Wave();
//        wave.num = num;

//        return wave;
//    }

//    IEnumerator StartCurrentWave()
//    {
//        // reveal the wave title for x seconds
//        waveTitleText.text = "wave " + _wave.num;
//        waveTitleText.gameObject.SetActive(true);
//        yield return new WaitForSeconds(2f);
//        waveTitleText.gameObject.SetActive(false);

//        // start the wave
//        _wave.isActive = true;
//    }

//    void EndWave()
//    {
//        _wave.isActive = false;
//    }

//    void RunChecks()
//    {
//        foreach (BirdType birdType in birdTypes)
//        {
//            float roll = Random.Range(0, 101) * _wave.multi;
//            float value = birdType.curve.Evaluate(0) * 100f;//GameManager.Instance.day.currentTimeOfDay);
//            if (roll <= value)
//                SpawnBird(birdType);
//        }

//        _wave.lastCheck = _waveTimer;
//    }

//    void SpawnBird(BirdType birdType)
//    {
//        // default check if it's too soon
//        float adjustedCooldown = birdType.minCooldown / _wave.multi;
//        if (Time.time < birdType.lastSpawn + adjustedCooldown)
//            return;

//        Vector3 spawnPos = Vector3.zero;
//        Quaternion spawnRot = Quaternion.identity;

//        if (birdType.prefab.name == "SimpleBird")
//        {
//            spawnPos = Random.onUnitSphere * GameManager.Instance.worldRadius;
//            spawnPos.y = Mathf.Abs(spawnPos.y);
//            spawnRot = Quaternion.LookRotation(Vector3.zero - spawnPos, Vector3.up);
//        }
//        else if (birdType.prefab.name == "TurdBird")
//        {
//            float height = birdType.prefab.GetComponent<TurdBird>().height;
//            Vector3 temp = Random.insideUnitCircle.normalized * GameManager.Instance.worldRadius;
//            spawnPos = new Vector3(temp.x, height, temp.y);
//            spawnRot = Quaternion.LookRotation(new Vector3(0, height, 0) - spawnPos, Vector3.up);
//        }

//        Instantiate(birdType.prefab, spawnPos, spawnRot);
//        birdType.lastSpawn = Time.time;
//    }

//    void GrowMultiplier(float amt)
//    {
//        _wave.multi += amt;
//        Debug.Log("multiplier increased to " + _wave.multi);
//        _wave.lastMultiIncrease = Time.time;
//    }
//}
#endregion