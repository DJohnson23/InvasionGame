using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    public abstract void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitDirection);
    public abstract void TakeDamage(float damage);
}
