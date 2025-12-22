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

        Debug.Log($"Dir:{shootDir}");

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

    void Fire(Vector3 dir)
    {
        Vector3 spawnPos = transform.position +
                           transform.TransformDirection(weaponData.shootOffset);

        //BulletBase bullet = BulletManager.Instance.Spawn(
        //    spawnPos,
        //    Quaternion.LookRotation(dir)
        //);

        //bullet.transform.forward = dir;
    }
}
