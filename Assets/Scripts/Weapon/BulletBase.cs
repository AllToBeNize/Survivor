using UnityEngine;

public class BulletBase : PooledObject
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public DamageInfo damageInfo;
    public TrailRenderer trail;

    private Vector3 direction;
    private float timer;
    private bool isActive = false;

    // New: whether to ignore the damage source
    private bool ignoreSource = true;

    public void Fire(Vector3 startPosition, Vector3 direction, DamageInfo damage, float speedOverride = -1f, bool ignoreSource = true)
    {
        transform.position = startPosition;
        this.direction = direction.normalized;
        this.damageInfo = damage;
        this.ignoreSource = ignoreSource;

        if (speedOverride > 0)
            speed = speedOverride;

        timer = 0f;
        isActive = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isActive) return;

        transform.position += direction * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Despawn();
        }
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        if (trail != null)
            trail.Clear();
    }

    public override void OnDespawn()
    {
        isActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        // Ignore the source if requested
        if (ignoreSource && damageInfo.Source != null && other.gameObject == damageInfo.Source)
            return;

        var attr = other.GetComponent<AttributeBase>();
        if (attr != null && attr.IsAlive)
        {
            attr.TakeDamage(damageInfo);
        }

        EffectManager.Instance.SpawnHitEffect(
            transform.position,
            Quaternion.LookRotation(-direction)
        );

        Despawn(); // Recycle on any collision
    }
}
