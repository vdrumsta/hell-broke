using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponArm : MonoBehaviour
{
    public bool HasWeapon;

    [SerializeField] Transform weaponHoldPoint;
    private GameObject currentWeaponInstance;

    void Update()
    {
        if (!HasWeapon)
        {
            // Point arm straight down
        }
    }

    public void ShootAt(Vector2 targetPoint)
    {
        if (HasWeapon)
        {
            // Point arm at targetPoint
            Vector2 direction = (Vector2) transform.position - targetPoint;
            transform.up = direction;

            // Fire weapon
            WeaponScript weaponScript = currentWeaponInstance.GetComponent<WeaponScript>();
            weaponScript.Fire();
        }
    }

    public void AddWeapon(GameObject prefab)
    {
        if (currentWeaponInstance)
        {
            Destroy(currentWeaponInstance);
        }

        currentWeaponInstance = Instantiate(prefab, weaponHoldPoint.position, weaponHoldPoint.rotation, weaponHoldPoint);
        HasWeapon = true;
    }
}
