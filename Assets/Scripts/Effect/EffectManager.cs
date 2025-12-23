using UnityEngine;

public class EffectManager : MonoSingleton<EffectManager>
{
    public HitEffect hitEffectPrefab;
    public int preloadCount = 20;

    private ObjectPool hitEffectPool;

    protected override void Awake()
    {
        base.Awake();

        hitEffectPool = new ObjectPool(
            hitEffectPrefab,
            preloadCount,
            transform
        );
    }

    public void SpawnHitEffect(Vector3 position, Quaternion rotation)
    {
        hitEffectPool.Spawn<HitEffect>(position, rotation);
    }
}
