using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderController : Damageable
{
    public float damageMultiplier = 1;
    EnemyController enemyController;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    public override void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitDirection)
    {
        enemyController.TakeDamage(damage * damageMultiplier, hitPosition, hitDirection);
    }

    public override void TakeDamage(float damage)
    {
        enemyController.TakeDamage(damage * damageMultiplier);
    }
}
