using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LavaMeltScript : MonoBehaviour
{
    [SerializeField] float _burnSpeed;

    private bool _isBurning;
    private Material[] _materials;
    private PlayerController _playerScript;

    void Start()
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _materials = spriteRenderers.Select(t => t.material).ToArray();
        _playerScript = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (_isBurning && _materials.Length > 0)
        {
            ProcessBurnTick();
        }
    }

    public void BurnObject()
    {
        if (!_isBurning)
        {
            _isBurning = true;

            // if LavaMeltScript is attached on the player, then we want to trigger game over
            if (_playerScript)
            {
                _playerScript.KillPlayer();
            }
        }
    }

    /// <summary>
    /// Every tick increase the dissolve effect from 100 to 0
    /// </summary>
    private void ProcessBurnTick()
    {
        bool burnedUp = false;
        foreach(Material mat in _materials)
        {
            if (!mat) continue;

            float newFadeValue = mat.GetFloat("_Fade");
            newFadeValue -= _burnSpeed * Time.deltaTime;
            newFadeValue = Mathf.Clamp01(newFadeValue);
            mat.SetFloat("_Fade", newFadeValue);

            burnedUp = newFadeValue <= 0 ? true : false;
        }

        if (burnedUp) 
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.simulated = false;
            }
        }
    }
}
