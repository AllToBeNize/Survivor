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

    /// <summary>
    /// Spawn a bullet
    /// </summary>
    /// <param name="position">Spawn position</param>
    /// <param name="direction">Forward direction</param>
    /// <param name="damage">Damage info</param>
    /// <param name="speed">Bullet speed override</param>
    /// <param name="ignoreSource">Whether to ignore the source</param>
    public void SpawnBullet(Vector3 position, Vector3 direction, DamageInfo damage, float speed = -1f, bool ignoreSource = true)
    {
        var bullet = bulletPool.Spawn<BulletBase>(position, Quaternion.LookRotation(direction));
        bullet.Fire(position, direction, damage, speed, ignoreSource);
    }
}
