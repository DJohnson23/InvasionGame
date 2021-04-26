using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask obstacleMask;
    public LayerMask targetMask;
    public float damage;
    public float range;
    public float speed = 30f;
    public float radius = 0.2f;

    float distanceTravelled = 0;

    private void Update()
    {
        float distance = speed * Time.deltaTime;
        transform.Translate(new Vector3(0, 0, distance));

        distanceTravelled += distance;

        if(distanceTravelled > range)
        {
            Destroy(gameObject);
            return;
        }


        Collider[] obstacleCollisions = Physics.OverlapSphere(transform.position, radius, obstacleMask);


        if(obstacleCollisions.Length > 0)
        {
            Destroy(gameObject);
            return;
        }

        Collider[] targetCollisions = Physics.OverlapSphere(transform.position, radius, targetMask);

        if(targetCollisions.Length > 0)
        {
            foreach(Collider collider in targetCollisions)
            {
                Damageable other = collider.GetComponent<Damageable>();

                if(other != null)
                {
                    other.TakeDamage(damage, transform.position, transform.forward);
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
