using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[
    RequireComponent(typeof(Collider2D)),
    RequireComponent(typeof(Rigidbody2D)),
    RequireComponent(typeof(PlayerController))
]
/* If the player about to jump on top of the platform,
 * but is missing just a little bit of height, help him
 * by bringing him on top of the platform
 */
public class PlatformJumpingHelper : MonoBehaviour
{
    [Tooltip("The lowest point that must be above the platform for the player to be brought up on the platform")]
    [SerializeField] private Transform _helpingCutOffPoint;
    [SerializeField] private LayerMask _platformLayer;
    [SerializeField] private float _distToForgetHelpedPlatform = 1f;

    private PlayerController _playerController;
    private Rigidbody2D _rb;
    private List<Collider2D> _helpedPlatforms;

    void Start()
    {
        _helpedPlatforms = new List<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        ForgetDistantHelpedPlatforms();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Potential platform collider
        var collider = collision.collider;

        if ((_platformLayer & 1 << collider.gameObject.layer) != 0 // Check if the other collider is part of platform layer
            && !_helpedPlatforms.Contains(collider) && !_playerController._isGrabbingWall && _playerController.isAlive)
        {
            Debug.Log("Bounce");
            var topOfPlatformYCoord = collider.bounds.max.y;
            var playerColliderCenterPos = collision.otherCollider.bounds.center;

            if (_helpingCutOffPoint.position.y > topOfPlatformYCoord && _rb.velocity.y < 0
                // check that player colider center is within platforms x boundaries
                && playerColliderCenterPos.x > collider.bounds.min.x && playerColliderCenterPos.x < collider.bounds.max.x)
            {
                var bottomOfPlayerYCoord = collision.otherCollider.bounds.min.y;
                var bottomOfPlayerToTopOfPlatformDistance = topOfPlatformYCoord - bottomOfPlayerYCoord;

                var newPos = transform.position;
                newPos.y = newPos.y + bottomOfPlayerToTopOfPlatformDistance + 0.05f;
                transform.position = newPos;

                _rb.velocity = Vector2.zero;

                _helpedPlatforms.Add(collider);
            }
        }
    }

    private void ForgetDistantHelpedPlatforms()
    {
        var newHelpedPlatforms = new List<Collider2D>();

        // if player moved away far enough, forget that he was helped on a platform
        // so that he can be helped on it again
        foreach (var collider in _helpedPlatforms)
        {
            Vector2 closestPlatformPointToPlayer = collider.ClosestPoint(_rb.position);
            if (Vector2.Distance(_rb.position, closestPlatformPointToPlayer) < _distToForgetHelpedPlatform)
            {
                newHelpedPlatforms.Add(collider);
            }
        }
        _helpedPlatforms = newHelpedPlatforms;
    }
}
