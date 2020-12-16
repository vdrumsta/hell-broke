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
    [SerializeField] LayerMask _groundLayerMask;
    [SerializeField] float _groundedCheckHeight = 1f;

    [Header("Trajectory")]
    [SerializeField] GameObject _trajectoryPointPrefab;
    [SerializeField] int _numberOfTrajectoryPoints = 10;
    [SerializeField] int _numberOfTrajectoryFadePoints = 3;

    [Header("Misc")]
    public bool isDead;
    [SerializeField] Collider2D _playerSpriteCollider;

    [Header("Lava")]
    [SerializeField] private LayerMask _lavaLayerMask;

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

        CreateTrajectoryPoints();
    }

    void Update()
    {
        _isGrounded = IsPlayerGrounded();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchPhase touchPhase = touch.phase;

            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Debug.DrawLine(transform.position, touchPos);

            CheckIfSwipeIsJump(touchPos);

            Vector2 direction = touchPos - (Vector2) transform.position;
            direction = direction.normalized;

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
                    }

                    ResetTouchVars();
                    break;
            }
        }

        _previousPlayerPos = transform.position;
    }

    private bool IsPlayerGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_playerSpriteCollider.bounds.center, _playerSpriteCollider.bounds.size, 0f, Vector2.down, _groundedCheckHeight, _groundLayerMask);

        Color boxColor;

        if (raycastHit.collider != null)
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

        return raycastHit.collider != null;
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
        if (!_isJump && _isGrounded)
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

    private void CreateTrajectoryPoints()
    {
        for (int i = 0; i < _trajectoryPoints.Length; i++)
        {
            _trajectoryPoints[i] = Instantiate(_trajectoryPointPrefab);
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
}
