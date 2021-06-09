using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] float _maxJumpPower = 7.0f;
    [SerializeField] float _maxJumpSwipeRadius = 1.0f;
    [SerializeField] LayerMask _groundLayerMask;
    [SerializeField] float _groundedCheckHeight = 1f;
    [SerializeField] float _dragDistanceForSwipe = 25f;
    private bool _isGrounded;
    private bool _isJumpTouch;
    private bool _playerHasJumpedOnce;

    [Header("Wall Grab")]
    [SerializeField] LayerMask _grabbableWallMask;
    public bool _isGrabbingWall;
    private bool _limitVelocityOnWallGrabbing;
    private List<GameObject> _touchedGrabbableWalls;

    [Header("Jump Trajectory")]
    [SerializeField] GameObject _trajectoryPointPrefab;
    [SerializeField] int _numberOfTrajectoryPoints = 10;
    [SerializeField] int _numberOfTrajectoryFadePoints = 3;
    private GameObject[] _trajectoryPoints;

    [Header("Lava")]
    [SerializeField] LayerMask _lavaLayerMask;

    [Header("Weapon")]
    [SerializeField] WeaponArm _weaponArm;

    [Header("Stun")]
    [SerializeField] GameObject _stunEffectRef;
    private bool _isStunned = false;
    private float _stunTimeLength;
    private Stopwatch _stunTimer;

    [Header("Misc")]
    [HideInInspector] public bool isAlive = true;
    [SerializeField] Collider2D _playerSpriteCollider;
    [SerializeField] LayerMask _pickUpLayerMask;
    [SerializeField] ParticleSystem _bloodPS;
    [SerializeField] ParticleSystem _dustPS;
    [SerializeField] float _uprightTorque;
    private Vector2 _originalTouchScreenPos;
    private bool _isFacingRight;

    [Header("Portal")]
    private float _timeToEnterPortal;
    private bool _movingTowardsPortal;
    private Stopwatch _portalEnterTimer;
    private Vector2 _portalEnteringStartingPosition;
    private Vector2 _portalPosition;


    private Rigidbody2D _rb;
    private Animator _anim;
    private ScoreMng _uiController;

    void Start()
    {
        _trajectoryPoints = new GameObject[_numberOfTrajectoryPoints];
        _touchedGrabbableWalls = new List<GameObject>();
        _stunTimer = new Stopwatch();
        _portalEnterTimer = new Stopwatch();

        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _uiController = FindObjectOfType<ScoreMng>();

        CreateTrajectoryPoints();
    }

    void Update()
    {
        _isGrounded = IsPlayerGrounded();

        if (_isGrounded && _rb.velocity.y < 0.1f)
        {
            //_rb.velocity = new Vector2(0, _rb.velocity.y);
        }

        // Don't allow any player controls once he's dead
        if (!isAlive) return;

        CheckStunState();

        ProcessPlayerTouch();

        KeepPlayerUpright();

        ProcessWallGrabbing();

        UpdatePlayerFacingDirection();

        if (_movingTowardsPortal)
        {
            MoveTowardsPortal();
        }
    }

    public void EnterPortal(Vector2 portalPosition, float timeToEnterPortal)
    {
        if (!isAlive) return;

        _anim.enabled = false;
        _rb.simulated = false;
        _portalEnteringStartingPosition = transform.position;
        _timeToEnterPortal = timeToEnterPortal;
        _portalPosition = portalPosition;
        _rb.simulated = false;
        _movingTowardsPortal = true;
        _portalEnterTimer.Start();
    }

    public void KillPlayer(bool emitBlood = false)
    {
        if (!isAlive) return;
        
        Debug.Log("Player has been killed");

        isAlive = false;
        _anim.SetTrigger("killPlayer");

        // Make the player face in the right direction so that his dead body is close to the floor
        FacePlayerInDirection(faceRight: true);

        if (emitBlood)
        {
            _bloodPS.Play();
        }

        if (_uiController)
        {
            _uiController.GameOver();
        }
    }

    public void StunPlayer(float stunTime = 1f)
    {
        _isStunned = true;
        _stunTimeLength = stunTime;
        _stunTimer.Restart();
        _stunEffectRef?.SetActive(true);
    }

    private void CheckStunState()
    {
        if (_stunTimer.Elapsed.TotalSeconds >= _stunTimeLength)
        {
            _isStunned = false;
            _stunEffectRef?.SetActive(false);
        }
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

            _isJumpTouch = CheckIfSwipeIsJump(touch);

            Vector2 direction = touch.position - _originalTouchScreenPos;
            direction = direction.normalized;

            // Calculate the force of jump depending of how far from the player the person drags his finger
            float dragDistance = Vector2.Distance(transform.position, touchWorldPos);
            float powerPercentage = Mathf.Clamp01(dragDistance / _maxJumpSwipeRadius);
            float currentJumpPower = powerPercentage * _maxJumpPower;

            switch (touchPhase)
            {
                case TouchPhase.Began:
                    break;
                case TouchPhase.Stationary:
                    UpdateTrajectory(direction, currentJumpPower);
                    break;
                case TouchPhase.Moved:
                    UpdateTrajectory(direction, currentJumpPower);
                    break;
                case TouchPhase.Ended:
                    if (_isJumpTouch)
                    {
                        Jump(direction, currentJumpPower);
                    }
                    else
                    {
                        _weaponArm.ShootAt(touchWorldPos);
                    }

                    ResetTouchVars();
                    break;
            }
        }
    }

    private void CreateDust()
    {
        _dustPS.Play();
    }

    private void KeepPlayerUpright()
    {
        var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
        _rb.AddTorque(rot.z * _uprightTorque);
    }

    private bool IsPlayerGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_playerSpriteCollider.bounds.center, _playerSpriteCollider.bounds.size, 0f, Vector2.down, _groundedCheckHeight, _groundLayerMask);
        
        Color boxColor;

        bool isStable = _rb.velocity.y < 0.1f && _rb.velocity.y > -0.1f;
        bool isGrounded = isStable && raycastHit.collider != null;
        //Debug.Log("hit = " + (bool) (raycastHit.collider != null) + "; isMovingDown = " + isStable + "; center = " + _playerSpriteCollider.bounds.center + "; size = " + _playerSpriteCollider.bounds.size);
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
        if (!isViableJump())
        {
            SetTrajectoryPointsActiveState(false);
            return;
        }

        SetTrajectoryPointsActiveState(true);
        for (int i = 0; i < _trajectoryPoints.Length; i++)
        {
            _trajectoryPoints[i].transform.position =  CalculatePointPosition(i * 0.1f, direction, jumpPower);
        }
    }

    private bool isViableJump()
    {
        return _isJumpTouch && (_isGrounded || _isGrabbingWall) && !_isStunned;
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
        _isJumpTouch = false;
        SetTrajectoryPointsActiveState(false);
    }

    /// <summary>
    /// Check if the swipe is eligible to be a jump swipe and if it is
    /// then activate the trajectory
    /// </summary>
    /// <param name="currentTouchPos"></param>
    private bool CheckIfSwipeIsJump(Touch currentTouch)
    {
        if (!_isJumpTouch && !_isStunned && (_isGrounded || _isGrabbingWall) && currentTouch.phase != TouchPhase.Began)
        {
            float fingerMoveDistance = Vector2.Distance(currentTouch.position, _originalTouchScreenPos);
            
            if (fingerMoveDistance > _dragDistanceForSwipe)
            {
                return true;
            }
        }

        // Otherwise return the previous state
        return _isJumpTouch;
    }

    private void Jump(Vector2 direction, float jumpPower)
    {
        if (!isViableJump()) return;

        if (!_playerHasJumpedOnce)
        {
            _playerHasJumpedOnce = true;

            LavaController lavaScript = FindObjectOfType<LavaController>();
            if (lavaScript) 
            {
                lavaScript.StartRising();
            }
            _uiController.LevelTimer = Stopwatch.StartNew();
        }

        _rb.AddForce(direction * jumpPower, ForceMode2D.Impulse);

        // Animation triggers
        _anim.SetTrigger("takeOff");

        // If the player was grabbing a wall, we want to stop limiting y velocity so that he can jump upwards
        _limitVelocityOnWallGrabbing = false;

        CreateDust();
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

    /// <summary>
    /// Determine which direction the player is facing based on his velocity and whether he's grabbing a wall.
    /// Wall grabbing takes precedence over velocity check.
    /// </summary>
    private void UpdatePlayerFacingDirection()
    {
        if (_isGrabbingWall && _touchedGrabbableWalls.Count > 0)
        {
            _isFacingRight = _touchedGrabbableWalls[0].transform.position.x < transform.position.x;
        }
        else if (_rb.velocity.sqrMagnitude > 0.1f)
        {
            _isFacingRight = _rb.velocity.x > 0;
        }

        FacePlayerInDirection(faceRight: _isFacingRight);
    }

    private void FacePlayerInDirection(bool faceRight)
    {
        Vector3 newScale = transform.localScale;

        // Facing right
        if (faceRight)
        {
            newScale.x = Mathf.Abs(newScale.x);
        }
        // Facing left
        else
        {
            newScale.x = -(Mathf.Abs(newScale.x));
        }

        transform.localScale = newScale;
    }

    private void ProcessWallGrabbing()
    {
        if (_touchedGrabbableWalls.Count > 0)
        {
            _isGrabbingWall = true;

            if (_limitVelocityOnWallGrabbing)
            {
                // Freeze the player so he doesn't fall
                Vector2 newVelocity = _rb.velocity;
                newVelocity.y = 0;
                _rb.velocity = newVelocity;
            }
        }
        else
        {
            _isGrabbingWall = false;
            _limitVelocityOnWallGrabbing = true;
        }
    }

    private void MoveTowardsPortal()
    {
        // Check if we're finished entering the portal
        if (_portalEnterTimer.Elapsed.TotalSeconds > _timeToEnterPortal)
        {
            Debug.Log("Player has entered the portal");
            _movingTowardsPortal = false;
            return;
        }

        float enterT = (float)_portalEnterTimer.Elapsed.TotalSeconds / _timeToEnterPortal;

        // Move players position towards the portal
        Vector2 newPos = Vector2.Lerp(_portalEnteringStartingPosition, _portalPosition, enterT);
        transform.position = newPos;

        // Shrink player
        float newScaleValue = 1 - enterT;
        transform.localScale = new Vector2(newScaleValue, newScaleValue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int otherLayer = collision.gameObject.layer;

        // Check if it's a pick up item
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
        // Check if it's a grabbable wall
        else if ((_grabbableWallMask & 1 << otherLayer) != 0)
        {
            _touchedGrabbableWalls.Add(collision.gameObject);
            _isGrabbingWall = true;
            CreateDust();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int otherLayer = collision.gameObject.layer;

        if ((_grabbableWallMask & 1 << otherLayer) != 0)
        {
            _touchedGrabbableWalls.Remove(collision.gameObject);

            if (_touchedGrabbableWalls.Count <= 0)
            {
                _isGrabbingWall = false;
            }
        }
    }
}
