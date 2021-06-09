using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

public class LavaController : MonoBehaviour
{
    public bool isRising;

    [SerializeField] private float _warmUpTime;
    [SerializeField] private AnimationCurve _warmUpSpeedCurve;
    [SerializeField] private float _riseSpeed;
    [SerializeField] private Transform _topLavaPoint;
    [SerializeField] private LayerMask _lavaLayerMask;
    
    private Stopwatch _lavaWarmUpTimer;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (isRising && !isLavaAboveScreen())
        {
            RiseLava();
        }
    }

    public void StartRising()
    {
        isRising = true;
        _lavaWarmUpTimer = Stopwatch.StartNew();
    }

    private bool isLavaAboveScreen()
    {
        if (!_mainCamera)
        {
            Debug.LogWarning("No camera found to check top lava point against");
        }

        return _mainCamera.WorldToViewportPoint(_topLavaPoint.position).y > 1;
    }

    private void RiseLava()
    {
        float currentRiseSpeed = 0f;
        if (_lavaWarmUpTimer != null && _lavaWarmUpTimer.Elapsed.TotalSeconds < _warmUpTime)
        {
            float percentageWarmedUp = (float) _lavaWarmUpTimer.Elapsed.TotalSeconds / _warmUpTime;
            currentRiseSpeed = _warmUpSpeedCurve.Evaluate(percentageWarmedUp) * _riseSpeed;
        }
        else
        {
            currentRiseSpeed = _riseSpeed;
        }

        Vector2 newPos = transform.position;
        newPos.y += currentRiseSpeed * Time.deltaTime;
        transform.position = newPos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var colliderLayer = other.gameObject.layer;
        if ((_lavaLayerMask & 1 << colliderLayer) != 0)
        {
            LavaMeltScript objectMeltScript = other.GetComponent<LavaMeltScript>();
            objectMeltScript?.BurnObject();
        }
    }
}
