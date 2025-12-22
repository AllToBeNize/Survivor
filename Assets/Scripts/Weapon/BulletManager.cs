using UnityEngine;

public class BulletManager : MonoSingleton<BulletManager>
{
    public BulletBase bulletPrefab;
    private ObjectPool bulletPool;
    public int preloadCount = 50;

    protected override void Awake()
    {
        base.Awake();
        bulletPool = new ObjectPool(bulletPrefab, preloadCount, transform);
    }

    public void SpawnBullet(Vector3 position, Vector3 direction, DamageInfo damage, float speed = -1f)
    {
        var bullet = bulletPool.Spawn<BulletBase>(position, Quaternion.LookRotation(direction));
        bullet.Fire(position, direction, damage, speed);
    }
}
