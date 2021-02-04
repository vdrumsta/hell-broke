using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakableBlockScript : MonoBehaviour
{
    [SerializeField] GameObject[] _breakablePieces;
    [SerializeField] LayerMask _bulletLayerMask;
    [SerializeField] float _breakApartForce = 1f;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int otherLayer = collision.gameObject.layer;

        // If hit by bullet, then break
        if ((_bulletLayerMask & 1 << otherLayer) != 0)
        {
            var rb = collision.GetComponent<Rigidbody2D>();
            Vector2 breakDirection = rb.velocity.normalized;
            BreakApart(breakDirection);

            Destroy(gameObject, 5);
        }

    }

    private void BreakApart(Vector2 direction)
    {
        // Enable breakable pieces and toss them in break direction
        foreach (var piece in _breakablePieces)
        {
            piece.SetActive(true);
            piece.transform.SetParent(null);
            Destroy(piece, 4);

            var rb = piece.GetComponent<Rigidbody2D>();
            if (!rb) continue;

            // add force to them in the opposite direction with slight variance
            float randomForceVariance = _breakApartForce * Random.Range(0f, 0.2f);
            rb.AddForce(direction * (_breakApartForce + randomForceVariance), ForceMode2D.Impulse);
        }

        // disable self. does it still destroy after disable?
        gameObject.SetActive(false);
    }
}
