﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] float _maxJumpPower = 7.0f;
    [SerializeField] float _maxJumpSwipeRadius = 1.0f;
    [SerializeField] LayerMask _groundLayerMask;
    [SerializeField] float _groundedCheckHeight = 1f;
    [SerializeField] float _dragDistanceForSwipe = 25f;
    private bool _isGrounded;

    [Header("Trajectory")]
    [SerializeField] GameObject _trajectoryPointPrefab;
    [SerializeField] int _numberOfTrajectoryPoints = 10;
    [SerializeField] int _numberOfTrajectoryFadePoints = 3;

    [Header("Misc")]
    public bool isDead;
    [SerializeField] Collider2D _playerSpriteCollider;
    [SerializeField] LayerMask _pickUpLayerMask;

    [Header("Lava")]
    [SerializeField] LayerMask _lavaLayerMask;
    

    [Header("Weapon")]
    [SerializeField] WeaponArm _weaponArm;

    private Rigidbody2D _rb;
    private Animator _anim;

    private Vector2 _originalTouchScreenPos;
    private Vector2 _previousPlayerPos;
    private bool _isJump;
    private bool _startedTouchOnPlayer;

    [SerializeField] private GameObject[] _trajectoryPoints;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        // Setup trajectory points
        _trajectoryPoints = new GameObject[_numberOfTrajectoryPoints];

        CreateTrajectoryPoints();
    }

    void Update()
    {
        _isGrounded = IsPlayerGrounded();

        ProcessPlayerTouch();

        UpdatePlayerFacingDirection();
        
        

        _previousPlayerPos = transform.position;
    }

    private void ProcessPlayerTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchPhase touchPhase = touch.phase;

            if (touchPhase == TouchPhase.Began)
            {
                _originalTouchScreenPos = touch.position;
            }

            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);

            CheckIfSwipeIsJump(touch);

            Vector2 direction = touch.position - _originalTouchScreenPos;
            direction = direction.normalized;

            // Calculate the force of jump depending of how far from the player the person drags his finger
            float dragDistance = Vector2.Distance(transform.position, touchWorldPos);
            float powerPercentage = Mathf.Clamp01(dragDistance / _maxJumpSwipeRadius);
            float currentJumpPower = powerPercentage * _maxJumpPower;

            switch (touchPhase)
            {
                case TouchPhase.Began:
                    _startedTouchOnPlayer = _playerSpriteCollider.OverlapPoint(touchWorldPos);
                    break;
                case TouchPhase.Stationary:
                    // Update trajectory
                    if (_isJump)
                    {
                        UpdateTrajectory(direction, currentJumpPower);
                    }
                    break;
                case TouchPhase.Moved:
                    // Update trajectory
                    if (_isJump)
                    {
                        UpdateTrajectory(direction, currentJumpPower);
                    }

                    break;
                case TouchPhase.Ended:
                    if (_isJump)
                    {
                        Debug.Log("It's a jump!");
                        Jump(direction, currentJumpPower);
                    }
                    else
                    {
                        Debug.Log("It's a shot!");
                        _weaponArm.ShootAt(touchWorldPos);
                    }

                    ResetTouchVars();
                    break;
            }
        }
    }

    private void UpdatePlayerFacingDirection()
    {
        if (_rb.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 newScale = transform.localScale;

            // Facing left
            if (_rb.velocity.x < 0)
            {
                newScale.x = -(Mathf.Abs(newScale.x));
            }
            // Facing right
            else
            {
                newScale.x = Mathf.Abs(newScale.x);
            }

            transform.localScale = newScale;
        }
    }

    private bool IsPlayerGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_playerSpriteCollider.bounds.center, _playerSpriteCollider.bounds.size, 0f, Vector2.down, _groundedCheckHeight, _groundLayerMask);

        Color boxColor;

        bool isStopped = _rb.velocity.sqrMagnitude < 0.1f;
        bool isGrounded = isStopped && raycastHit.collider != null;

        if (isGrounded)
        {
            boxColor = Color.green;
        }
        else
        {
            boxColor = Color.red;
        }

        // Draw a box underneath the player. Don't need the top line of the box
        Debug.DrawRay(_playerSpriteCollider.bounds.center + new Vector3(_playerSpriteCollider.bounds.extents.x, 0), Vector2.down * (_playerSpriteCollider.bounds.extents.y + _groundedCheckHeight), boxColor);
        Debug.DrawRay(_playerSpriteCollider.bounds.center - new Vector3(_playerSpriteCollider.bounds.extents.x, 0), Vector2.down * (_playerSpriteCollider.bounds.extents.y + _groundedCheckHeight), boxColor);
        Debug.DrawRay(_playerSpriteCollider.bounds.center - new Vector3(_playerSpriteCollider.bounds.extents.x, _playerSpriteCollider.bounds.extents.y + _groundedCheckHeight), Vector2.right * _playerSpriteCollider.bounds.size.x, boxColor);

        // Animation param set
        _anim.SetBool("isJumping", !isGrounded);

        return isGrounded;
    }

    private void UpdateTrajectory(Vector2 direction, float jumpPower)
    {
        SetTrajectoryPointsActiveState(true);

        for (int i = 0; i < _trajectoryPoints.Length; i++)
        {
            _trajectoryPoints[i].transform.position =  CalculatePointPosition(i * 0.1f, direction, jumpPower);
        }
    }

    private void SetTrajectoryPointsActiveState(bool state)
    {
        foreach (GameObject point in _trajectoryPoints)
        {
            point.SetActive(state);
        }
    }

    private void ResetTouchVars()
    {
        _isJump = false;
        _startedTouchOnPlayer = false;
        SetTrajectoryPointsActiveState(false);
    }

    /// <summary>
    /// Check if the swipe is eligible to be a jump swipe and if it is
    /// then activate the trajectory
    /// </summary>
    /// <param name="currentTouchPos"></param>
    private void CheckIfSwipeIsJump(Touch currentTouch)
    {
        if (!_isJump && _isGrounded && currentTouch.phase != TouchPhase.Began)
        {
            float fingerMoveDistance = Vector2.Distance(currentTouch.position, _originalTouchScreenPos);
            
            if (fingerMoveDistance > _dragDistanceForSwipe)
            {
                _isJump = true;
            }
        }
    }

    private void Jump(Vector2 direction, float jumpPower)
    {
        _rb.AddForce(direction * jumpPower, ForceMode2D.Impulse);

        // Animation triggers
        _anim.SetTrigger("takeOff");
    }

    private Vector2 CalculatePointPosition(float t, Vector2 direction, float jumpPower)
    {
        return ((Vector2) transform.position) + (direction * jumpPower * t) + 0.5f * ((Vector2) Physics.gravity) * (t * t);
    }

    private void CreateTrajectoryPoints()
    {
        var trajectoryParent = new GameObject("TrajectoryPoints");
        for (int i = 0; i < _trajectoryPoints.Length; i++)
        {
            _trajectoryPoints[i] = Instantiate(_trajectoryPointPrefab, trajectoryParent.transform);
            _trajectoryPoints[i].SetActive(false);

            // Make the last few points fade off
            int fadePointsRemaining = _trajectoryPoints.Length - i;
            if (fadePointsRemaining <= _numberOfTrajectoryFadePoints)
            {
                // Calculate what the fade value will be
                float fadeAlphaValue = 1f / (_numberOfTrajectoryFadePoints + 1) * fadePointsRemaining;
                var trajectoryPointRenderer = _trajectoryPoints[i].GetComponent<SpriteRenderer>();
                var newColor = trajectoryPointRenderer.color;
                newColor.a = fadeAlphaValue;
                trajectoryPointRenderer.color = newColor;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int otherLayer = collision.gameObject.layer;

        if ((_pickUpLayerMask & 1 << otherLayer) != 0)
        {
            PickUpScript pickUp = collision.gameObject.GetComponent<PickUpScript>();

            // Determine what the pick up is
            if (pickUp.Type == PickUpType.Weapon)
            {
                _weaponArm.AddWeapon(pickUp.WeaponSpawnPrefab);
            }

            Destroy(collision.gameObject);
        }
    }
}
