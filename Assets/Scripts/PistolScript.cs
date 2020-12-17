using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolScript : WeaponScript
{
    [SerializeField] GameObject _bulletInstance;
    [SerializeField] Transform _bulletSpawnPoint;
    [SerializeField] float _bulletFireForce;

    public override void Fire()
    {
        GameObject bullet = Instantiate(_bulletInstance, _bulletSpawnPoint.position, transform.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(-(transform.up) * _bulletFireForce, ForceMode2D.Impulse);
    }
}
