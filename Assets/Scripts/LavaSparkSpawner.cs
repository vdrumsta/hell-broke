using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSparkSpawner : MonoBehaviour
{
    [SerializeField] GameObject _bubblePrefab;
    [SerializeField] Transform _rightLimit;
    [SerializeField] Transform _leftLimit;
    [SerializeField] int _maxBubbles;

    private List<GameObject> _bubbles;
    private bool _spawningBubble;

    void Start()
    {
        _bubbles = new List<GameObject>();
    }

    void Update()
    {
        if (_bubbles.Count < _maxBubbles && !_spawningBubble)
        {
            _spawningBubble = true;
            StartCoroutine(WaitBeforeSpawningBubble(Random.Range(0.01f, 0.5f)));
        }
    }

    IEnumerator WaitBeforeSpawningBubble(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SpawnBubble();
    }

    private void SpawnBubble()
    {
        float randomXPos = Random.Range(_leftLimit.position.x, _rightLimit.position.x);
        Vector2 spawnPos = new Vector2(randomXPos, transform.position.y);
        var bubbleInstance = Instantiate(_bubblePrefab, spawnPos, Quaternion.identity, transform);
        _bubbles.Add(bubbleInstance);
        _spawningBubble = false;
    }
}
