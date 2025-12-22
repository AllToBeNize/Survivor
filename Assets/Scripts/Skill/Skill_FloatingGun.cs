using System.Collections;
using UnityEngine;

public class Skill_FloatingGun : SkillBase
{
    [Header("Gun Settings")]
    public Transform gunMuzzle;          // Bullet spawn point (fixed on model)
    public float fireRate = 0.2f;        // Seconds between shots
    public float bulletSpeed = 10f;
    public float bulletDamage = 10f;

    [Header("Targeting")]
    public float rotationSpeed = 180f;   // Degrees per second
    public float detectionRadius = 15f;  // Enemy detection radius
    public LayerMask enemyLayer;         // Layers that are considered enemies

    [Header("Debug")]
    public bool debugMode = false;

    private Coroutine fireRoutine;

    private EnemyBase currentTarget;

    protected override void OnEquip()
    {
        if (gunMuzzle == null)
            gunMuzzle = transform; // fallback

        fireRoutine = StartCoroutine(FireRoutine());
    }

    private void Update()
    {
        if (!isEquipped) return;

        currentTarget = FindNearestEnemy();
        if (currentTarget != null)
            RotateTowardsTarget(currentTarget);
    }

    private IEnumerator FireRoutine()
    {
        while (isEquipped)
        {
            if (currentTarget != null)
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
        Quaternion finalRot = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        transform.rotation = finalRot;

        if (debugMode)
        {
            Debug.Log($"[FloatingGun] Target: {enemy.name}, HeadPos: {headPos}, TargetRot: {targetRot.eulerAngles}, FinalRot: {finalRot.eulerAngles}");
        }
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
