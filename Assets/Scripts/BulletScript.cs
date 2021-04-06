using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletScript : MonoBehaviour
{
    [SerializeField] LayerMask _enemyLayerMask;

    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int otherLayer = collision.gameObject.layer;
        if ((_enemyLayerMask & 1 << otherLayer) != 0)
        {
            Vector2 knockbackDirection = _rb.velocity.normalized;

            var enemyScript = collision.GetComponent<EnemyScript>();
            enemyScript?.Die(knockbackDirection, true);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
