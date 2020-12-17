using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType
{
    Weapon
}

public class PickUpScript : MonoBehaviour
{
    public PickUpType Type;
    public GameObject WeaponSpawnPrefab;
}
