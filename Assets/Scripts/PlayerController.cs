using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] float _maxJumpPower = 7.0f;
    [SerializeField] float _maxJumpSwipeRadius = 1.0f;

    [Header("Misc")]
    [SerializeField] float _gravityMultiplier = 2.7f;
    [SerializeField] Collider2D _playerSpriteCollider;
    [SerializeField] GameObject _trajectoryPointPrefab;
    [SerializeField] int _numberOfTrajectoryPoints = 10;

    private Rigidbody2D _rb;
    private bool _isGrounded;

    private Vector2 _previousPlayerPos;
    private bool _isJump;
    private bool _startedTouchOnPlayer;

    private GameObject[] _trajectoryPoints;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Setup trajectory points
        _trajectoryPoints = new GameObject[_numberOfTrajectoryPoints];
        for (int i = 0; i < _numberOfTrajectoryPoints; i++)
        {
            _trajectoryPoints[i] = Instantiate(_trajectoryPointPrefab);
            _trajectoryPoints[i].SetActive(false);
        }
    }

    void Update()
    {
        // Increase the speed of the player falling so he doesn't look floaty
        //_rb.AddForce(_gravityMultiplier * Physics2D.gravity * _rb.mass, ForceMode2D.Force);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchPhase touchPhase = touch.phase;

            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Debug.DrawLine(transform.position, touchPos);

            CheckIfSwipeIsJump(touchPos);

            Vector2 direction = touchPos - (Vector2) transform.position;
            direction = direction.normalized;
            Debug.Log(direction);

            // Calculate the force of jump depending of how far from the player the person drags his finger
            float dragDistance = Vector2.Distance((Vector2)transform.position, touchPos);
            float powerPercentage = Mathf.Clamp01(dragDistance / _maxJumpSwipeRadius);
            float currentJumpPower = powerPercentage * _maxJumpPower;

            switch (touchPhase)
            {
                case TouchPhase.Began:
                    _startedTouchOnPlayer = _playerSpriteCollider.OverlapPoint(touchPos);
                    break;
                case TouchPhase.Stationary:
                    // Nothing
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
                    }

                    ResetTouchVars();
                    break;
            }
        }

        _previousPlayerPos = transform.position;
    }

    private void UpdateTrajectory(Vector2 direction, float jumpPower)
    {
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
    private void CheckIfSwipeIsJump(Vector2 currentTouchPos)
    {
        if (!_isJump)
        {
            _isJump = (_startedTouchOnPlayer && !_playerSpriteCollider.OverlapPoint(currentTouchPos));

            if (_isJump)
            {
                SetTrajectoryPointsActiveState(true);
            }
        }
    }

    private void Jump(Vector2 direction, float jumpPower)
    {
        _rb.AddForce(direction * jumpPower, ForceMode2D.Impulse);
    }

    private Vector2 CalculatePointPosition(float t, Vector2 direction, float jumpPower)
    {
        return ((Vector2) transform.position) + (direction * jumpPower * t) + 0.5f * ((Vector2) Physics.gravity) * (t * t);
    }
}
