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
    public bool debugMode = true;

    private Coroutine fireRoutine;

    protected override void OnEquip()
    {
        if (gunMuzzle == null)
            gunMuzzle = transform;

        fireRoutine = StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        while (isEquipped)
        {
            EnemyBase targetEnemy = FindNearestEnemy();
            if (targetEnemy != null)
            {
                RotateTowardsTarget(targetEnemy);
                Shoot();
            }

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

        if (debugMode)
        {
            Debug.Log($"[FloatingGun] Target: {enemy.name}, HeadPos: {headPos}, TargetRot: {targetRot.eulerAngles}, FinalRot: {finalRot.eulerAngles}");
        }

        transform.rotation = finalRot;
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
