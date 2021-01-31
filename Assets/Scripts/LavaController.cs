using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    public bool isRising;

    [SerializeField] private float _riseSpeed;
    [SerializeField] private LayerMask _lavaLayerMask;


    void Update()
    {
        if (isRising)
        {
            Vector2 newPos = transform.position;
            newPos.y += _riseSpeed * Time.deltaTime;
            transform.position = newPos;
        }
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
