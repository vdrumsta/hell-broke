using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float SpeedRange = 3;
    public float SpeedMultiplier = 2f;
    private float _speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        _speed = Random.Range(- SpeedRange, SpeedRange);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, _speed * SpeedMultiplier * Time.deltaTime));
    }
}
