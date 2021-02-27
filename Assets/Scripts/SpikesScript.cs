using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesScript : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayerMask;

    private Animator _anim;
    private Collider2D _spikesCollider;
    private PlayerController _playerScript;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _spikesCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int otherLayer = collision.gameObject.layer;

        // If player touches the spikes, then activate the spikes
        if ((_playerLayerMask & 1 << otherLayer) != 0)
        {
            _playerScript = collision.GetComponent<PlayerController>();
            // Animation triggers
            _anim.SetTrigger("activateSpikes");
        }
    }

    /// <summary>
    /// Checks whether the player is still on the spikes and if he is, kill him
    /// </summary>
    public void SpikeKillPlayer()
    {
        if (_spikesCollider.IsTouchingLayers(_playerLayerMask))
        {
            if (_playerScript)
            {
                _playerScript.KillPlayer(true);
            }
            else
            {
                Debug.LogError("Couldn't retrieve player script to kill him with spikes");
            }
        }
    }
}