using UnityEngine;
using System;

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

    // Event triggered when weapon fires
    public event Action OnWeaponFired;

    private void Start()
    {
        rotationController = GetComponent<PlayerRotationController>();

        if (weaponData == null)
            return;

        mountPoint = transform.Find(weaponData.mountPointName) ?? transform;

        if (weaponData.modelPrefab != null)
        {
            weaponModelInstance = Instantiate(weaponData.modelPrefab, mountPoint.position, mountPoint.rotation, mountPoint);

            muzzlePoint = string.IsNullOrEmpty(weaponData.muzzlePointName)
                ? weaponModelInstance.transform
                : weaponModelInstance.transform.Find(weaponData.muzzlePointName) ?? weaponModelInstance.transform;
        }
        else
        {
            muzzlePoint = transform;
        }
    }

    private void Update()
    {
        if (PlayerManager.Instance != null && !PlayerManager.Instance.IsPlayerAlive()) 
        {
            return;
        }
        if (weaponData == null || rotationController == null)
            return;

        fireTimer -= Time.deltaTime;

        Transform target = FindTarget();
        if (target == null)
            return;

        Vector3 shootDir = target.position - transform.position;
        shootDir.y = 0f;

        rotationController.SetDirection(shootDir, weaponData.rotationPriority);

        if (fireTimer <= 0f)
        {
            Fire(shootDir);
            fireTimer = weaponData.fireInterval;
        }
    }

    private Transform FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, weaponData.range, enemyLayer);

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

        // Trigger the event
        OnWeaponFired?.Invoke();
    }
}
