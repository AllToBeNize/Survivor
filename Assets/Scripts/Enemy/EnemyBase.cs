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
    public Transform AttackPoint;       // Attack origin
    public float AttackRadius = 1f;     // Detection radius
    public float AttackDelay = 0.3f;    // Delay before applying damage
    public float AttackDamage = 10f;
    public LayerMask targetLayer;

    private NavMeshAgent agent;
    private AttributeBase attr;
    private Animator animator;
    private Transform target;
    private bool canAttack = true;
    private bool initialized = false;

    private void Awake()
    {
        // 缓存组件，只执行一次
        if (!initialized)
        {
            initialized = true;
            agent = GetComponent<NavMeshAgent>();
            attr = GetComponent<AttributeBase>();
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void OnEnable()
    {
        // 每次激活都重置状态
        if (attr != null)
        {
            attr.HP = attr.MaxHP;
            attr.OnDead += Die;
        }

        canAttack = true;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = MoveSpeed;
            agent.isStopped = false;
        }

        // 激活时找目标（场景直接放置时自动找 Player）
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }
    }

    private void OnDisable()
    {
        if (attr != null)
            attr.OnDead -= Die;

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

        Debug.Log($"{name} starts attack on {target?.name}");

        yield return new WaitForSeconds(AttackDelay);

        if (AttackPoint != null)
        {
            Collider[] hits = Physics.OverlapSphere(AttackPoint.position, AttackRadius, targetLayer);
            if (hits.Length == 0)
            {
                Debug.Log($"{name} attack hit nothing.");
            }
            foreach (var hit in hits)
            {
                var targetAttr = hit.GetComponent<AttributeBase>();
                if (targetAttr != null && targetAttr.IsAlive)
                {
                    DamageInfo dmg = new DamageInfo(AttackDamage, gameObject);
                    float applied = targetAttr.TakeDamage(dmg);
                    Debug.Log($"{name} hit {targetAttr.name} for {applied} damage. Target HP: {targetAttr.HP}");
                }
            }
        }
        else
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

        // 延迟回收一帧，避免 NavMeshAgent 报错
        StartCoroutine(DespawnNextFrame());
    }

    private IEnumerator DespawnNextFrame()
    {
        yield return null; // 等一帧
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
