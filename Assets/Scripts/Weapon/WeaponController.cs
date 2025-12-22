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

    private Transform mountPoint;
    private GameObject weaponModelInstance;
    private Transform muzzlePoint;

    private void Start()
    {
        rotationController = GetComponent<PlayerRotationController>();

        if (weaponData == null)
            return;

        // Find mount point on player by name
        mountPoint = transform.Find(weaponData.mountPointName);
        if (mountPoint == null)
        {
            Debug.LogWarning($"Mount point '{weaponData.mountPointName}' not found on player. Using player root.");
            mountPoint = transform;
        }

        // Instantiate weapon model
        if (weaponData.modelPrefab != null)
        {
            weaponModelInstance = Instantiate(
                weaponData.modelPrefab,
                mountPoint.position,
                mountPoint.rotation,
                mountPoint
            );

            // Find muzzle point on model by name
            if (!string.IsNullOrEmpty(weaponData.muzzlePointName))
            {
                muzzlePoint = weaponModelInstance.transform.Find(weaponData.muzzlePointName);
                if (muzzlePoint == null)
                {
                    Debug.LogWarning($"Muzzle point '{weaponData.muzzlePointName}' not found on weapon model. Using model root.");
                    muzzlePoint = weaponModelInstance.transform;
                }
            }
            else
            {
                muzzlePoint = weaponModelInstance.transform;
            }
        }
        else
        {
            // Fallback if no model prefab
            muzzlePoint = transform;
        }
    }

    private void Update()
    {
        if (weaponData == null || rotationController == null)
            return;

        fireTimer -= Time.deltaTime;

        Transform target = FindTarget();
        if (target == null)
            return;

        // Direction in XZ plane
        Vector3 shootDir = target.position - transform.position;
        shootDir.y = 0f;

        // Rotate player/weapon
        rotationController.SetDirection(shootDir, weaponData.rotationPriority);

        // Fire
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
            float dist = Vector3.SqrMagnitude(hit.transform.position - transform.position);
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
        if (BulletManager.Instance == null)
            return;

        Vector3 spawnPos = (muzzlePoint != null ? muzzlePoint.position : transform.position) +
                           (muzzlePoint != null ? muzzlePoint.forward : transform.forward) * 0f +
                           weaponData.shootOffset;

        Vector3 forwardDir = muzzlePoint != null ? muzzlePoint.forward : transform.forward;

        DamageInfo dmg = new DamageInfo(weaponData.damage, gameObject);

        BulletManager.Instance.SpawnBullet(spawnPos, forwardDir.normalized, dmg, weaponData.bulletSpeed);
    }
}
