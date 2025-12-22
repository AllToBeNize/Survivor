using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AttributeBase))]
public class EnemyBase : PooledObject
{
    [Header("Movement & Attack")]
    public float MoveSpeed = 3.5f;
    public float AttackRange = 2f;
    public float AttackCooldown = 1.5f;
    public Transform AttackPoint;
    public float AttackRadius = 1f;
    public float AttackDelay = 0.3f;
    public float AttackDamage = 10f;
    public LayerMask targetLayer;

    [Header("Debug")]
    public bool debugMode = false;

    private NavMeshAgent agent;
    private AttributeBase attr;
    private Animator animator;
    private Transform target;
    private bool canAttack = true;
    private bool initialized = false;
    private bool deadSubscribed = false;

    private void Awake()
    {
        if (!initialized)
        {
            initialized = true;
            agent = GetComponent<NavMeshAgent>();
            attr = GetComponent<AttributeBase>();
            animator = GetComponentInChildren<Animator>();

            // Subscribe OnDead once
            if (!deadSubscribed)
            {
                attr.OnDead += Die;
                deadSubscribed = true;
            }
        }
    }

    private void OnEnable()
    {
        // Reset health and state
        if (attr != null)
        {
            attr.ResetAttribute();
        }

        canAttack = true;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = MoveSpeed;
            agent.isStopped = false;
        }

        // Find player automatically if not assigned
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
            else if (debugMode)
                Debug.LogWarning($"{name} cannot find Player in scene!");
        }
    }

    public AttributeBase GetAttribute()
    {
        return attr;
    }

    private void OnDisable()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
    }

    private void Update()
    {
        if (target == null || attr == null || !attr.IsAlive) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > AttackRange)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }

            if (animator != null)
                animator.SetBool("IsMoving", true);

            if (debugMode)
                Debug.Log($"{name} moving towards {target.name}");
        }
        else
        {
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;

            if (animator != null)
                animator.SetBool("IsMoving", false);

            if (canAttack)
                StartCoroutine(AttackRoutine());
        }
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        if (animator != null)
            animator.SetTrigger("Attack");

        if (debugMode)
            Debug.Log($"{name} starts attack on {target?.name}");

        yield return new WaitForSeconds(AttackDelay);

        if (AttackPoint != null)
        {
            Collider[] hits = Physics.OverlapSphere(AttackPoint.position, AttackRadius, targetLayer);
            if (hits.Length == 0 && debugMode)
                Debug.Log($"{name} attack hit nothing.");

            foreach (var hit in hits)
            {
                var targetAttr = hit.GetComponent<AttributeBase>();
                if (targetAttr != null && targetAttr.IsAlive)
                {
                    DamageInfo dmg = new DamageInfo(AttackDamage, gameObject);
                    float applied = targetAttr.TakeDamage(dmg);

                    if (debugMode)
                        Debug.Log($"{name} hit {targetAttr.name} for {applied} damage. Target HP: {targetAttr.HP}");
                }
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning($"{name} has no AttackPoint assigned.");
        }

        yield return new WaitForSeconds(AttackCooldown - AttackDelay);
        canAttack = true;
    }

    private void Die()
    {
        if (animator != null)
            animator.SetTrigger("Die");

        // Delay despawn by one frame to avoid NavMeshAgent errors
        StartCoroutine(DespawnNextFrame());
    }

    private IEnumerator DespawnNextFrame()
    {
        yield return null;
        Despawn();
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(AttackPoint.position, AttackRadius);
        }
    }
}
