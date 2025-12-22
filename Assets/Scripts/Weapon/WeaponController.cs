using UnityEngine;

[RequireComponent(typeof(PlayerRotationController))]
public class WeaponController : MonoBehaviour
{
    [Header("Weapon")]
    public WeaponBase weaponData;

    [Header("Target")]
    public LayerMask enemyLayer;

    private PlayerRotationController rotationController;

    private float fireTimer;

    private void Start()
    {
        rotationController = GetComponent<PlayerRotationController>();
    }

    private void Update()
    {
        if (weaponData == null)
            return;

        fireTimer -= Time.deltaTime;

        Transform target = FindTarget();
        if (target == null)
            return;

        Vector3 shootDir = (target.position - transform.position);
        shootDir.y = 0f;

        rotationController.SetDirection(
            shootDir,
            weaponData.rotationPriority
        );

        if (fireTimer <= 0f)
        {
            Fire(shootDir);
            fireTimer = weaponData.fireInterval;
        }
    }
    private Transform FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            weaponData.range,
            enemyLayer
        );

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector3.SqrMagnitude(
                hit.transform.position - transform.position
            );

            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    private void Fire(Vector3 dir)
    {
        Vector3 spawnPos = transform.position +
                           transform.TransformDirection(weaponData.shootOffset);

        DamageInfo dmg = new DamageInfo(weaponData.damage, gameObject);

        BulletManager.Instance.SpawnBullet(spawnPos, dir.normalized, dmg, weaponData.bulletSpeed);
    }
}
