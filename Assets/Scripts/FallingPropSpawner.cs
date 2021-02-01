using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FallingPropSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _propPrefabs;
    [SerializeField] private float _spawnInterval = 1f;
    [SerializeField] private float _spawnIntervalRandomnessRange = 0.5f;
    [SerializeField] private Transform _leftLimitPoint;
    [SerializeField] private Transform _rightLimitPoint;

    private Stopwatch _spawnTimer;
    private float _waitTimeBeforeSpawn;

    private void Start()
    {
        _spawnTimer = Stopwatch.StartNew();
    }

    void Update()
    {
        if (_spawnTimer.Elapsed.TotalSeconds > _waitTimeBeforeSpawn)
        {
            SpawnProp();
            _spawnTimer.Restart();
            _waitTimeBeforeSpawn = GenerateWaitTime();
        }
    }

    private void SpawnProp()
    {
        if (_propPrefabs.Length <= 0)
        {
            UnityEngine.Debug.LogError("No prop prefabs added to the list");
            return;
        }

        int propIndex = Random.Range(0, _propPrefabs.Length);
        Vector3 spawnPos = Vector3.zero;
        spawnPos.x = Random.Range(_leftLimitPoint.position.x, _rightLimitPoint.position.x);

        GameObject newProp = Instantiate(_propPrefabs[propIndex], transform);
        newProp.transform.localPosition = spawnPos;
        Destroy(newProp, 3f); // Destroy after 5 s

        // Add rotation to the spawned prop
        var rb = newProp.GetComponent<Rigidbody2D>();
        float randomSpinForce = 0.2f + Random.Range(0, 0.6f);
        rb.AddTorque(randomSpinForce, ForceMode2D.Impulse);
    }

    private float GenerateWaitTime()
    {
        return Random.Range(_spawnInterval - _spawnIntervalRandomnessRange, _spawnInterval + _spawnIntervalRandomnessRange);
    }
}
