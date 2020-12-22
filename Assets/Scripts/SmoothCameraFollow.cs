using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform player;
    public float cameraSpeed = 0.1f;

    [SerializeField] bool _enableHorizontalLock;
    [SerializeField] bool _enableVerticalLimit;

    [Header("Limits")]
    [SerializeField] Transform _topLimit;
    [SerializeField] Transform _bottomLimit;

    private Vector3 cameraOffset;

    private void Start()
    {
        cameraOffset = transform.position;
        transform.position = player.position + cameraOffset;
    }

    private void Update()
    {
        if (_enableHorizontalLock)
        {
            Vector3 newPos = transform.position;
            newPos.x = cameraOffset.x;
            transform.position = newPos;
        }
    }

    void FixedUpdate()
    {
        Vector3 finalPosition = player.position + cameraOffset;
        Vector3 lerpPosition = Vector3.Lerp(transform.position, finalPosition, cameraSpeed);

        if (_enableVerticalLimit)
        {
            if (lerpPosition.y > _topLimit.position.y)
            {
                lerpPosition.y = _topLimit.position.y;
            }
            else if (lerpPosition.y < _bottomLimit.position.y)
            {
                lerpPosition.y = _bottomLimit.position.y;
            }
        }
        transform.position = lerpPosition;
    }
}
