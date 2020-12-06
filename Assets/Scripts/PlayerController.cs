using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movSpeed;                //The movement speed when grounded
    [SerializeField] float movAccel;                //The maximum change in velocity the player can do on the ground. This determines how responsive the character will be when on the ground.

    [Header("Jump")]
    [SerializeField] KeyCode jumpButton;
    [SerializeField] float initialJumpForce;        //The force applied to the player when starting to jump

    [Header("Misc")]
    [SerializeField] float gravityMultiplier = 2.7f;
    [SerializeField] Collider2D playerSpriteCollider;

    //Rigidbody cache
    private Rigidbody2D rigidbody;
    private bool isGrounded;

    private Vector2 originalTouchPos;
    private bool isJump;
    private bool isTouching;

    void Start()
    {
        //Setup our rigidbody cache variable
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Increase the speed of the player falling so he doesn't look floaty
        rigidbody.AddForce(gravityMultiplier * Physics2D.gravity * rigidbody.mass, ForceMode2D.Force);

        Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        if (Input.GetKeyDown(jumpButton))
        {
            Jump();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchPhase touchPhase = touch.phase;

            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Debug.DrawLine(transform.position, touchPos);

            switch (touchPhase)
            {
                case TouchPhase.Began:
                    originalTouchPos = touchPos;
                    break;
                case TouchPhase.Stationary:
                    if (!isJump)
                    {
                        isJump = IsJumpSwipe(touchPos);
                    }

                    break;
                case TouchPhase.Moved:
                    if (!isJump)
                    {
                        isJump = IsJumpSwipe(touchPos);
                    }

                    // Update trajectory

                    break;
                case TouchPhase.Ended:
                    if (isJump)
                    {
                        Debug.Log("It's a jump!");
                    }
                    else
                    {
                        Debug.Log("It's a shot!");
                    }

                    ResetTouchVars();
                    break;
            }
        }
    }

    private void ResetTouchVars()
    {
        isJump = false;
    }

    private bool IsJumpSwipe(Vector2 currentTouchPos)
    {
        return (playerSpriteCollider.OverlapPoint(originalTouchPos) && !playerSpriteCollider.OverlapPoint(currentTouchPos));
    }

    private void Move(Vector2 _dir)
    {
        Vector2 velocity = rigidbody.velocity;

        //The velocity we want our character to have. We get the movement direction, the ground direction and the speed we want (ground speed or air speed)
        Vector2 targetVelocity = _dir * movSpeed;

        //The change in velocity we need to perform to achieve our target velocity
        Vector2 velocityDelta = targetVelocity - velocity;

        //The maximum change in velocity we can do
        float maxDelta = movAccel;

        //Clamp the velocity delta to our maximum velocity change
        velocityDelta.x = Mathf.Clamp(velocityDelta.x, -maxDelta, maxDelta);

        //We don't want to move the character vertically
        velocityDelta.y = 0;

        //Apply the velocity change to the character
        rigidbody.AddForce(velocityDelta * rigidbody.mass, ForceMode2D.Impulse);
    }

    void Jump()
    {
        rigidbody.AddForce(Vector3.up * initialJumpForce * rigidbody.mass, ForceMode2D.Impulse);
    }
}
