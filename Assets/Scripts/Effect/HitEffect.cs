using UnityEngine;

public class HitEffect : PooledObject
{
    public ParticleSystem ps;

    private float lifeTime;

    private void Awake()
    {
        if (ps == null)
            ps = GetComponentInChildren<ParticleSystem>();

        var main = ps.main;
        lifeTime = main.duration + main.startLifetime.constantMax;
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        ps.Play();
        Invoke(nameof(Despawn), lifeTime);
    }

    public override void OnDespawn()
    {
        CancelInvoke();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
