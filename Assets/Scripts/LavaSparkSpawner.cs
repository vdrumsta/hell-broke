using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public struct Bubble
{
    public Bubble(float startTime, float endTime, float startSize, float endSize, GameObject bubbleGameobject)
    {
        _startTime = startTime;
        _endTime = endTime;
        _startSize = startSize;
        _endSize = endSize;
        _bubbleGameObject = bubbleGameobject;
    }

    public GameObject _bubbleGameObject { get; }
    public float _startTime { get; }
    public float _endTime { get; }
    public float _startSize { get; }
    public float _endSize { get; }
}
public class LavaSparkSpawner : MonoBehaviour
{
    [SerializeField] GameObject _bubblePrefab;
    [SerializeField] Transform _rightLimit;
    [SerializeField] Transform _leftLimit;
    [SerializeField] int _maxBubbles;
    
    [SerializeField] float _minStartSize;
    [SerializeField] float _maxStartSize;
    [SerializeField] float _minEndSize; // Will be added onto start size
    [SerializeField] float _maxEndSize; // Will be added onto start size
    [SerializeField] float _minGrowTime;
    [SerializeField] float _maxGrowTime;

    [SerializeField] Transform _sparksParent;
    [SerializeField] GameObject _sparksPrefab;
    [SerializeField] float _sizePerSpark;

    private List<Bubble> _bubbles;
    private bool _spawningBubble;
    private Stopwatch _bubbleTimer;

    void Start()
    {
        _bubbles = new List<Bubble>();
        _bubbleTimer = Stopwatch.StartNew();
    }

    void Update()
    {
        if (_bubbles.Count < _maxBubbles && !_spawningBubble)
        {
            _spawningBubble = true;
            StartCoroutine(WaitBeforeSpawningBubble(Random.Range(0.01f, 0.5f)));
        }

        // Gather a list of bubbles that we need to remove
        List<Bubble> tempBubblesToRemove = new List<Bubble>();
        foreach (var bubble in _bubbles)
        {
            if (_bubbleTimer.Elapsed.TotalSeconds >= bubble._endTime)
            {
                tempBubblesToRemove.Add(bubble);
            }
        }

        // Remove the bubbles that have grown past their predetermined grow time
        foreach (var bubble in tempBubblesToRemove)
        {
            // Calculate the number of sparks to spawn
            int numOfSparksToSpawn = (int) (bubble._endSize / _sizePerSpark);

            if (numOfSparksToSpawn > 0)
            {
                var sparksInstance = Instantiate(_sparksPrefab, bubble._bubbleGameObject.transform.position, Quaternion.identity, _sparksParent);
                var sparksEmitter = sparksInstance.GetComponent<ParticleSystem>();

                // Add a burst of sparks
                sparksEmitter.emission.SetBurst(0, new ParticleSystem.Burst(0f, numOfSparksToSpawn));

                sparksEmitter.Play();

                Destroy(sparksInstance, 2f);
            }

            _bubbles.Remove(bubble);
            Destroy(bubble._bubbleGameObject);
        }

        // Grow the remaining bubbles
        foreach (var bubble in _bubbles)
        {
            float timePassedRatio = ((float) _bubbleTimer.Elapsed.TotalSeconds - bubble._startTime) / (bubble._endTime - bubble._startTime);
            float newSize = Mathf.Lerp(bubble._startSize, bubble._endSize, timePassedRatio);
            bubble._bubbleGameObject.transform.localScale = new Vector2(newSize, newSize);
        }
    }

    IEnumerator WaitBeforeSpawningBubble(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SpawnBubble();
        _spawningBubble = false;
    }

    private void SpawnBubble()
    {
        float randomXPos = Random.Range(_leftLimit.position.x, _rightLimit.position.x);
        Vector2 spawnPos = new Vector2(randomXPos, transform.position.y);
        float randomStartSize = Random.Range(_minStartSize, _maxStartSize);
        Vector2 spawnScale = new Vector2(randomStartSize, randomStartSize);
        float randomEndSize = Random.Range(randomStartSize + _minEndSize, randomStartSize + _maxEndSize);

        var bubbleInstance = Instantiate(_bubblePrefab, spawnPos, Quaternion.identity, transform);
        bubbleInstance.transform.localScale = spawnScale;

        float startTime = (float) _bubbleTimer.Elapsed.TotalSeconds;
        float randomEndTime = Random.Range(startTime + _minGrowTime, startTime + _maxGrowTime);
        _bubbles.Add(new Bubble(startTime, randomEndTime, randomStartSize, randomEndSize, bubbleInstance));
    }
}
