using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class EnemyController : Damageable
{
    enum EnemyState
    {
        Searching,
        Pursuing,
        Attacking,
        Dead
    }

    enum SearchState
    {
        Walking,
        Waiting,
        ReadyToWalk
    }

    [System.Serializable]
    public struct Sight
    {
        public float range;
        [Range(0, 180)]
        public float angle;
        public Vector3 offset;
        public bool showGizmo;
    }


    public GameObject bloodParticle;
    public float searchDistance;
    public Animator animator;
    public Weapon weapon;
    public float attackBufferRange = 10f;
    public Sight sight;
    public LayerMask sightBlockMask;
    public LayerMask targetMask;
    public float walkSpeed = 3.5f;
    public float runSpeed = 7f;
    public GameObject weaponProjectile;
    public float alertRange = 20f;

    public bool IsDead
    {
        get
        {
            return state == EnemyState.Dead;
        }
    }

    Health health;

    Vector3 startPosition;
    NavMeshAgent agent;
    EnemyState state;

    SearchState searchState;
    Coroutine searchWait;

    Vector3 lastSeenLocation;
    bool playerInView;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        searchState = SearchState.ReadyToWalk;
        playerInView = false;
        agent.speed = walkSpeed;
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsDead)
        {
            return;
        }

        playerInView = PlayerInView();

        switch (state)
        {
            case EnemyState.Searching:
                Search();
                break;
            case EnemyState.Pursuing:
                Pursue();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }
    }

    void OnDeath()
    {
        state = EnemyState.Dead;
        animator.SetTrigger("die");
        agent.SetDestination(transform.position);
        Destroy(gameObject, 3);
    }

    void Search()
    {
        if(searchState == SearchState.ReadyToWalk)
        {
            Vector3 ranDir = Random.insideUnitSphere * searchDistance;
            NavMeshHit hit;
            NavMesh.SamplePosition(ranDir + startPosition, out hit, searchDistance, 1);
            agent.speed = walkSpeed;

            agent.SetDestination(hit.position);
            searchState = SearchState.Walking;
            animator.SetBool("walk", true);
        }
        else if (searchState == SearchState.Walking)
        {
            if (agent.remainingDistance < Mathf.Epsilon)
            {
                searchState = SearchState.Waiting;
                animator.SetBool("walk", false);
                searchWait = StartCoroutine(SearchWait());
            }
        }

        if(playerInView)
        {
            AlertEnemiesInRange();
            StartPursuing();
        }
        
    }

    public void Alert(Vector3 position)
    {
        if(IsDead)
        {
            return;
        }

        lastSeenLocation = position;
        agent.SetDestination(lastSeenLocation);
        StartPursuing();
    }

    void AlertEnemiesInRange()
    {
        EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();

        Vector3 dstVec;
        float sqrRange = alertRange * alertRange;

        foreach(EnemyController enemy in allEnemies)
        {
            dstVec = transform.position - enemy.transform.position;

            if(dstVec.sqrMagnitude < sqrRange)
            {
                enemy.Alert(lastSeenLocation);
            }
        }
    }

    void StartPursuing()
    {
        if (searchWait != null)
        {
            StopCoroutine(searchWait);
        }

        searchState = SearchState.ReadyToWalk;
        animator.SetBool("walk", false);
        state = EnemyState.Pursuing;
    }

    bool PlayerInView()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 sightStart = transform.TransformPoint(sight.offset);
        Vector3 dstVector = player.transform.position - sightStart;

        if(dstVector.sqrMagnitude <= sight.range * sight.range)
        {
            float angle = Vector3.Angle(transform.forward, player.transform.position - transform.position);

            if(angle <= sight.angle)
            {
                Vector3 direction = player.transform.position - sightStart;
                direction.y = 0;
                float distance = direction.magnitude;
                direction.Normalize();

                if(Physics.Raycast(sightStart, direction, distance, sightBlockMask))
                {
                    return false;
                }

                lastSeenLocation = player.transform.position;
                return true;
            }
        }

        return false;
    }

    IEnumerator SearchWait()
    {
        float waitTime = Random.Range(2f, 4f);
        yield return new WaitForSeconds(waitTime);
        searchState = SearchState.ReadyToWalk;
    }

    void Pursue()
    {
        Vector3 dstVec = lastSeenLocation - transform.position;
        dstVec.y = 0;
        if(playerInView)
        {
            if(dstVec.sqrMagnitude > attackBufferRange * attackBufferRange)
            {
                agent.SetDestination(lastSeenLocation);
                agent.speed = runSpeed;
                animator.SetBool("run", true);
            }
            else
            {
                animator.SetBool("run", false);
                state = EnemyState.Attacking;
            }
        }
        else if (agent.remainingDistance < Mathf.Epsilon)
        {
            animator.SetBool("run", false);
            state = EnemyState.Searching;
        }
        else
        {
            animator.SetBool("run", true);
            agent.SetDestination(lastSeenLocation);
        }
    }

    void Attack()
    {
        if(!playerInView)
        {
            animator.SetBool("attack", false);
            state = EnemyState.Pursuing;
            agent.SetDestination(lastSeenLocation);
            return;
        }

        agent.SetDestination(transform.position);

        Vector3 dstVec = lastSeenLocation - transform.position;
        dstVec.y = 0;

        transform.forward = dstVec.normalized;

        animator.SetBool("attack", true);

        if(dstVec.sqrMagnitude > weapon.range * weapon.range)
        {
            animator.SetBool("attack", false);
            state = EnemyState.Pursuing;
        }
    }

    public void FireWeapon()
    {
        Vector3 playerCenter = lastSeenLocation + Vector3.up * 1.5f;
        Vector3 direction = playerCenter - weapon.barrelLocation.position;
        direction.y = 0;
        GameObject newObj = Instantiate(weaponProjectile, weapon.barrelLocation.position, Quaternion.LookRotation(direction, Vector3.up));

        Projectile newProjectile = newObj.GetComponent<Projectile>();
        newProjectile.damage = weapon.damage;
        newProjectile.range = weapon.range;
        newProjectile.targetMask = targetMask;
    }

    public override void TakeDamage(float damage)
    {
        health.TakeDamage(damage);
    }

    public override void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitDirection)
    {
        TakeDamage(damage);

        if(bloodParticle)
        {
            GameObject newObj = Instantiate(bloodParticle, hitPosition, Quaternion.LookRotation(-hitDirection, Vector3.up));
            Destroy(newObj, 2);
        }

        if(state == EnemyState.Searching)
        {
            lastSeenLocation = transform.position - hitDirection.normalized * 5;
            agent.SetDestination(lastSeenLocation);
            AlertEnemiesInRange();
            StartPursuing();
        }
        else if(state == EnemyState.Pursuing && !playerInView)
        {
            lastSeenLocation = transform.position - hitDirection.normalized * 5;
            agent.SetDestination(lastSeenLocation);
            AlertEnemiesInRange();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        if(!Application.isPlaying)
        {
            Gizmos.DrawWireSphere(transform.position, searchDistance);
        }
        else
        {
            Gizmos.DrawWireSphere(startPosition, searchDistance);
        }

        Gizmos.color = Color.blue;

        if(sight.showGizmo)
        {
            DrawSight();
        }
    }

    void DrawSight()
    {
        Vector3 startPos = sight.offset;
        
        int circleSteps = 30;

        float angleStep = 2 * Mathf.PI / circleSteps;

        Vector3 worldStart = transform.TransformPoint(startPos);

        float radAngle = Mathf.Deg2Rad * sight.angle;
        float radius = sight.range * Mathf.Sin(radAngle);
        float z = sight.range * Mathf.Cos(radAngle);

        Vector3 lastPos = startPos + new Vector3(radius, 0, z);

        for (int i = 1; i <= circleSteps; i++)
        {
            float angle = angleStep * i;
            Vector3 newPos = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), z) + startPos;

            Gizmos.DrawLine(transform.TransformPoint(lastPos), transform.TransformPoint(newPos));
            lastPos = newPos;
        }

        int semiCircSteps = (int)(sight.angle * 2 / 360 * circleSteps);
        semiCircSteps = Mathf.Max(semiCircSteps, 4);

        if(semiCircSteps % 2 == 1)
        {
            semiCircSteps++;
        }

        lastPos = startPos + new Vector3(-radius, 0, z);
        angleStep = radAngle * 2 / semiCircSteps;

        for(int i = 1; i <= semiCircSteps; i++)
        {
            float angle = -radAngle + angleStep * i;
            float curZ = sight.range * Mathf.Cos(angle);
            float curX = sight.range * Mathf.Sin(angle);
            Vector3 newPos = new Vector3(curX, 0, curZ) + startPos;

            Gizmos.DrawLine(transform.TransformPoint(lastPos), transform.TransformPoint(newPos));
            lastPos = newPos;
        }

        lastPos = startPos + new Vector3(0, -radius, z);
        angleStep = radAngle * 2 / semiCircSteps;

        for (int i = 1; i <= semiCircSteps; i++)
        {
            float angle = -radAngle + angleStep * i;
            float curZ = sight.range * Mathf.Cos(angle);
            float curY = sight.range * Mathf.Sin(angle);
            Vector3 newPos = new Vector3(0, curY, curZ) + startPos;

            Gizmos.DrawLine(transform.TransformPoint(lastPos), transform.TransformPoint(newPos));
            lastPos = newPos;
        }

        Gizmos.DrawLine(worldStart, transform.TransformPoint(startPos + new Vector3(radius, 0, z)));
        Gizmos.DrawLine(worldStart, transform.TransformPoint(startPos + new Vector3(0, radius, z)));
        Gizmos.DrawLine(worldStart, transform.TransformPoint(startPos + new Vector3(-radius, 0, z)));
        Gizmos.DrawLine(worldStart, transform.TransformPoint(startPos + new Vector3(0, -radius, z)));
    }
}
