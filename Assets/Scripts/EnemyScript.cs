using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyScript : MonoBehaviour
{
    public abstract void Die(Vector2 knockbackDirection, bool breakApart);
}
