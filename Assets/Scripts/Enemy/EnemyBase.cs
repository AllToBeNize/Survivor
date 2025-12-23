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
    public Transform HeadTransform;
    public float AttackRadius = 1f;
    public float AttackDelay = 0.3f;
    public float AttackDamage = 10f;
    public LayerMask targetLayer;

    [Header("Layer Settings")]
    public string DeadLayerName = "Dead";

    [Header("Debug")]
    public bool debugMode = false;

    private NavMeshAgent agent;
    private AttributeBase attr;
    private Animator animator;
    private Transform target;

    private bool canAttack = true;
    private bool initialized = false;
    private bool deadSubscribed = false;

    private Collider[] cachedColliders;
    private Transform[] cachedTransforms;
    private int[] cachedLayers;
    private int deadLayer;

    private void Awake()
    {
        if (initialized) return;
        initialized = true;

        agent = GetComponent<NavMeshAgent>();
        attr = GetComponent<AttributeBase>();
        animator = GetComponentInChildren<Animator>();

        cachedColliders = GetComponentsInChildren<Collider>(true);
        cachedTransforms = GetComponentsInChildren<Transform>(true);

        cachedLayers = new int[cachedTransforms.Length];
        for (int i = 0; i < cachedTransforms.Length; i++)
        {
            cachedLayers[i] = cachedTransforms[i].gameObject.layer;
        }

        deadLayer = LayerMask.NameToLayer(DeadLayerName);
        if (deadLayer == -1)
        {
            Debug.LogError($"Layer '{DeadLayerName}' does not exist!");
        }

        if (!deadSubscribed)
        {
            attr.OnDead += Die;
            deadSubscribed = true;
        }
    }

    private void OnEnable()
    {
        if (attr != null)
            attr.ResetAttribute();

        canAttack = true;

        SetAliveState();

        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = MoveSpeed;
            agent.isStopped = false;
        }

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
            else if (debugMode)
                Debug.LogWarning($"{name} cannot find Player!");
        }
    }

    private void OnDisable()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
    }

    private void Update()
    {
        if (target == null || attr == null || !attr.IsAlive)
            return;

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

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        if (animator != null)
            animator.SetTrigger("Attack");

        yield return new WaitForSeconds(AttackDelay);

        if (AttackPoint != null)
        {
            Collider[] hits = Physics.OverlapSphere(
                AttackPoint.position,
                AttackRadius,
                targetLayer
            );

            foreach (var hit in hits)
            {
                var targetAttr = hit.GetComponent<AttributeBase>();
                if (targetAttr != null && targetAttr.IsAlive)
                {
                    DamageInfo dmg = new DamageInfo(AttackDamage, gameObject);
                    targetAttr.TakeDamage(dmg);
                }
            }
        }

        yield return new WaitForSeconds(Mathf.Max(0f, AttackCooldown - AttackDelay));
        canAttack = true;
    }

    private void Die()
    {
        SetDeadState();

        if (animator != null)
            animator.SetTrigger("Die");

        StartCoroutine(DespawnAfterDelay());
    }

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        Despawn();
    }

    private void SetDeadState()
    {
        canAttack = false;

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;

        for (int i = 0; i < cachedColliders.Length; i++)
            cachedColliders[i].enabled = false;

        if (deadLayer != -1)
        {
            for (int i = 0; i < cachedTransforms.Length; i++)
                cachedTransforms[i].gameObject.layer = deadLayer;
        }
    }

    private void SetAliveState()
    {
        for (int i = 0; i < cachedColliders.Length; i++)
            cachedColliders[i].enabled = true;

        for (int i = 0; i < cachedTransforms.Length; i++)
        {
            if (cachedTransforms[i] != null)
                cachedTransforms[i].gameObject.layer = cachedLayers[i];
        }

        if (agent != null)
            agent.enabled = true;
    }

    public AttributeBase GetAttribute()
    {
        return attr;
    }

    public Vector3 GetHeadLocation()
    {
        if (HeadTransform != null)
            return HeadTransform.position;

        return transform.position + Vector3.up * 1.5f;
    }

    public void SetTarget(Transform t)
    {
        target = t;
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
