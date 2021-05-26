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
    [SerializeField] private LayerMask _lavaLayerMask;
    
    private Stopwatch _lavaWarmUpTimer;

    private void Start()
    {
        _lavaWarmUpTimer = Stopwatch.StartNew();
    }

    void Update()
    {
        if (isRising)
        {
            RiseLava();
        }
    }

    private void RiseLava()
    {
        float currentRiseSpeed = 0f;
        if (_lavaWarmUpTimer.Elapsed.TotalSeconds < _warmUpTime)
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
            // Retrieve player script and make player burn
            LavaMeltScript objectMeltScript = other.GetComponent<LavaMeltScript>();
            objectMeltScript?.BurnObject();
        }
    }
}
