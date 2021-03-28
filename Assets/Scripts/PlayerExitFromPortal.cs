using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerExitFromPortal : MonoBehaviour
{
    [SerializeField] private Vector2 _exitForce;
    void Start()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.AddForce(_exitForce, ForceMode2D.Impulse);
    }
}
