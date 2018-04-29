using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab;
    public float cooldown;
    public float radius;

    float _lastSpawn;


    private void Update()
    {
        if (Time.time > _lastSpawn + cooldown)
            SpawnBird();
    }

    void SpawnBird()
    {
        Vector3 point = Random.onUnitSphere * radius;
        point.y = Mathf.Abs(point.y);
        Quaternion rot = Quaternion.LookRotation(Vector3.zero - point, Vector3.up);
        Instantiate(birdPrefab, point, rot);
        _lastSpawn = Time.time;
    }
}
