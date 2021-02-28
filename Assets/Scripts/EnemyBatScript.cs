using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBatScript : EnemyScript
{
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _bounceBackSpeed;
    [SerializeField] private float _dieKnockbackSpeed;
    [SerializeField] private float _dieTorqueSpeed;
    [SerializeField] private float _distanceToActivate; // Distance to player before activating
    [SerializeField] private float _playerStunTime = 1f;


    private Rigidbody2D _rb;
    private GameObject _playerRef;
    private PlayerController _playerController;
    private bool _foundPlayer;
    private bool _isDead;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerController = FindObjectOfType<PlayerController>();
        _playerRef = _playerController.gameObject;
    }

    void Update()
    {
        if (!_playerRef || _isDead) return;

        if (!_foundPlayer && Vector2.Distance(transform.position, _playerRef.transform.position) < _distanceToActivate)
        {
            _foundPlayer = true;

            // Add initial force to get the rigidbody up to speed
            _rb.AddForce(GetPlayerDirection() * _flySpeed * 2, ForceMode2D.Impulse);
        }

        if (_foundPlayer)
        {
            Vector2 directionToPlayer = GetPlayerDirection();

            // Add continuous force towards player
            _rb.AddForce(directionToPlayer * _flySpeed, ForceMode2D.Force);

            // Add corrective force so that the enemy doesn't 'orbit' around the player 
            Vector2 correctiveDirection = Vector2.zero;
            if (Vector3.Cross(directionToPlayer, _rb.velocity.normalized).z > 0)
            {
                correctiveDirection = Utils2D.PerpendicularClockwise(directionToPlayer);
            }
            else
            {
                correctiveDirection = Utils2D.PerpendicularCounterClockwise(directionToPlayer);
            }
            
            _rb.AddForce(correctiveDirection, ForceMode2D.Force);
        }
    }

    public override void Hit(Vector2 knockbackDirection)
    {
        _isDead = true;
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.AddForce(knockbackDirection * _dieKnockbackSpeed, ForceMode2D.Impulse);

        // If knockbacked left, then spin left
        float torqueForce = knockbackDirection.x > 0 ? -_dieTorqueSpeed : _dieTorqueSpeed;
        _rb.AddTorque(torqueForce, ForceMode2D.Impulse);
        _rb.gravityScale = 1f;
    }

    private Vector2 GetPlayerDirection()
    {
        Vector2 directionTowardsPlayer = _playerRef.transform.position - transform.position;
        directionTowardsPlayer = directionTowardsPlayer.normalized;

        return directionTowardsPlayer;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isDead) return;

        if (collision.gameObject == _playerRef)
        {
            // Bounce back upon colliding with the player
            _rb.AddForce(-(GetPlayerDirection()) * _bounceBackSpeed, ForceMode2D.Impulse);
            _playerController.StunPlayer(_playerStunTime);
        }
    }
}
