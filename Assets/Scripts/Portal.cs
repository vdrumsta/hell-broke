using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Portal : MonoBehaviour
{
    [SerializeField] private bool _exitPortal = true;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private GameObject _winUI;

    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();

        if (!_exitPortal)
        {
            _anim.SetTrigger("SwirlEnter");
        }
    }

    public void FinishExit()
    {
        if (_winUI && _exitPortal)
        {
            _winUI.SetActive(true);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int otherLayer = collision.gameObject.layer;

        if (_exitPortal && (_playerLayer & 1 << otherLayer) != 0)
        {
            _anim.SetTrigger("SwirlExit");

            var animState = _anim.GetCurrentAnimatorStateInfo(0);
            var animationLength = animState.length;
            var playerController = collision.GetComponent<PlayerController>();
            playerController?.EnterPortal(transform.position, animationLength / 2);
        }
    }
}