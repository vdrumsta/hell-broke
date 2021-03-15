using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[
    RequireComponent(typeof(Collider2D)),
    RequireComponent(typeof(Rigidbody2D))
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

    private Rigidbody2D _rb;
    private List<Collider2D> _helpedPlatforms;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _helpedPlatforms = new List<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var collider = collision.collider;

        if ((_platformLayer & 1 << collider.gameObject.layer) != 0 // Check if the other collider is part of platform layer
            && !_helpedPlatforms.Contains(collider))
        {
            var topOfPlatformYCoord = collider.bounds.max.y;

            if (_helpingCutOffPoint.position.y > topOfPlatformYCoord && _rb.velocity.y < 0)
            {
                var bottomOfPlayerYCoord = collision.otherCollider.bounds.min.y;
                var bottomOfPlayerToTopOfPlatformDistance = topOfPlatformYCoord - bottomOfPlayerYCoord;

                var newPos = transform.position;
                newPos.y = newPos.y + bottomOfPlayerToTopOfPlatformDistance + 0.1f;
                transform.position = newPos;

                _helpedPlatforms.Add(collider);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var collider = collision.collider;

        if (_helpedPlatforms.Contains(collider))
        {
            _helpedPlatforms.Remove(collider);
        }
    }
}
