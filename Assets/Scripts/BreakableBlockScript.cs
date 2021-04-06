using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakableBlockScript : MonoBehaviour
{
    [SerializeField] LayerMask _bulletLayerMask;
    BreakApartScript _breakScript;

    private void Start()
    {
        _breakScript = GetComponent<BreakApartScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int otherLayer = collision.gameObject.layer;

        // If hit by bullet, then break
        if ((_bulletLayerMask & 1 << otherLayer) != 0)
        {
            var rb = collision.GetComponent<Rigidbody2D>();
            Vector2 breakDirection = rb.velocity.normalized;
            _breakScript?.BreakApart(breakDirection);

            Destroy(gameObject, 5);
        }

    }

    
}
