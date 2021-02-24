using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBatScript : MonoBehaviour
{
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _distanceToActivate; // Distance to player before activating

    private Rigidbody2D _rb;
    private GameObject _playerRef;
    private bool _foundPlayer;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerRef = FindObjectOfType<PlayerController>().gameObject;
    }

    void Update()
    {
        if (!_playerRef) return;

        if (!_foundPlayer && Vector2.Distance(transform.position, _playerRef.transform.position) < _distanceToActivate)
        {
            _foundPlayer = true;

            // Add initial force to get the rigidbody up to speed
            _rb.AddForce(GetPlayerDirection() * _flySpeed * 2, ForceMode2D.Impulse);
        }

        if (_foundPlayer)
        {
            // Add continuous force towards player
            _rb.AddForce(GetPlayerDirection() * _flySpeed, ForceMode2D.Force);
        }
    }

    private Vector2 GetPlayerDirection()
    {
        Vector2 directionTowardsPlayer = _playerRef.transform.position - transform.position;
        directionTowardsPlayer = directionTowardsPlayer.normalized;

        return directionTowardsPlayer;
    }
}
