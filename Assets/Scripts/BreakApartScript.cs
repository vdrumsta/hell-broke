using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakApartScript : MonoBehaviour
{
    [SerializeField] GameObject[] _breakablePieces;
    [SerializeField] GameObject[] _objectsToDisable;
    [SerializeField] float _breakApartForce = 1f;
    [SerializeField] float _breakApartDirectionVariance = 2f;

    public void BreakApart(Vector2 direction)
    {
        // Enable breakable pieces and toss them in break direction
        foreach (var piece in _breakablePieces)
        {
            piece.SetActive(true);
            piece.transform.SetParent(null);
            Destroy(piece, 4);

            var rb = piece.GetComponent<Rigidbody2D>();
            if (!rb) continue;

            // Add force to them in the opposite direction with slight variance
            float randomForceVariance = _breakApartForce * Random.Range(0f, 0.2f);

            var directionAngleVariance = Random.Range(-_breakApartDirectionVariance, _breakApartDirectionVariance);
            var newDirection = Quaternion.AngleAxis(directionAngleVariance, Vector3.forward) * direction;

            rb.AddForce(newDirection * (_breakApartForce + randomForceVariance), ForceMode2D.Impulse);
        }

        foreach(var objectToDisable in _objectsToDisable)
        {
            objectToDisable.SetActive(false);
        }
    }
}
