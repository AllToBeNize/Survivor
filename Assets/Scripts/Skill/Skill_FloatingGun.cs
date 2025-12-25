using System.Collections;
using UnityEngine;

public class Skill_FloatingGun : SkillBase
{
    [Header("Gun Settings")]
    public Transform gunMuzzle;
    public float fireRate = 0.2f;
    public float bulletSpeed = 10f;
    public float bulletDamage = 10f;

    [Header("Targeting")]
    public float rotationSpeed = 180f;
    public float detectionRadius = 15f;
    public LayerMask enemyLayer;

    [Header("Idle Rotation")]
    public float idleRotationSpeed = 30f; // Degrees per second
    public float idleRotationRange = 60f; // Max angle from initial forward
    private float idleAngle = 0f;
    private int idleDirection = 1;

    [Header("Debug")]
    public bool debugMode = false;

    private Coroutine fireRoutine;
    private EnemyBase currentTarget;
    private Vector3 followOffset;
    private Quaternion initialRotation;

    private AttributeBase attribute;

    protected override void OnEquip()
    {
        if (owner == null) return;

        attribute = owner.GetComponent<AttributeBase>();
        attribute.OnDead += OnDead;

        // 记录偏移
        followOffset = transform.position - owner.position;

        // 取消 parent
        transform.SetParent(null);

        if (gunMuzzle == null)
            gunMuzzle = transform;

        // 保存初始朝向
        initialRotation = transform.rotation;

        fireRoutine = StartCoroutine(FireRoutine());
    }

    private void OnDead()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isEquipped || owner == null) return;

        // 跟随玩家位置
        transform.position = owner.position + followOffset;

        currentTarget = FindNearestEnemy();
        if (currentTarget != null)
            RotateTowardsTarget(currentTarget);
        else
            RotateIdle();
    }

    private void RotateIdle()
    {
        // Idle 旋转来回摆动
        float delta = idleRotationSpeed * Time.deltaTime * idleDirection;
        idleAngle += delta;

        if (Mathf.Abs(idleAngle) > idleRotationRange)
        {
            idleAngle = Mathf.Clamp(idleAngle, -idleRotationRange, idleRotationRange);
            idleDirection *= -1; // 反向
        }

        transform.rotation = initialRotation * Quaternion.Euler(0f, idleAngle, 0f);
    }

    private IEnumerator FireRoutine()
    {
        while (isEquipped)
        {
            if (currentTarget != null && PlayerManager.Instance.IsPlayerAlive())
                Shoot();

            yield return new WaitForSeconds(fireRate);
        }
    }

    private EnemyBase FindNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        EnemyBase nearestEnemy = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null && enemy.GetAttribute().IsAlive)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestEnemy = enemy;
                }
            }
        }

        return nearestEnemy;
    }

    private void RotateTowardsTarget(EnemyBase enemy)
    {
        if (enemy == null) return;

        Vector3 headPos = enemy.GetHeadLocation();
        Vector3 dir = (headPos - transform.position).normalized;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        if (debugMode)
            Debug.Log($"[FloatingGun] Target: {enemy.name}, Dir: {dir}");
    }

    private void Shoot()
    {
        if (BulletManager.Instance == null) return;

        DamageInfo dmg = new DamageInfo(bulletDamage, owner.gameObject);
        Vector3 spawnPos = gunMuzzle != null ? gunMuzzle.position : transform.position;
        Vector3 forwardDir = gunMuzzle != null ? gunMuzzle.forward : transform.forward;

        BulletManager.Instance.SpawnBullet(spawnPos, forwardDir, dmg, bulletSpeed);
    }

    public override void Unequip()
    {
        base.Unequip();
        if (fireRoutine != null)
            StopCoroutine(fireRoutine);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
