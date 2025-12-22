using UnityEngine;
using System;

public struct DamageInfo
{
    public float BaseDamage;        // Base damage
    public float ArmorPenetration;  // 0~1
    public bool IsCritical;
    public float CriticalMultiplier;
    public GameObject Source;       // Damage source

    public DamageInfo(float dmg, GameObject source = null)
    {
        BaseDamage = dmg;
        ArmorPenetration = 0f;
        IsCritical = false;
        CriticalMultiplier = 1f;
        Source = source;
    }
}

public class AttributeBase : MonoBehaviour
{
    [Header("Attributes")]
    public float MaxHP = 100f;
    public float HP = 100f;
    public float Defense = 0f;

    public bool IsInvincible = false;
    private bool isDead = false;

    public event Action OnDead;
    public event Action<float> OnDamageTaken;
    public event Action<float> OnHeal;

    /// <summary>
    /// Handles damage with DamageInfo
    /// </summary>
    public float TakeDamage(DamageInfo info)
    {
        if (IsInvincible || isDead) return 0f;

        float effectiveDefense = Defense * (1f - info.ArmorPenetration);

        float damage = Mathf.Max(0, info.BaseDamage - effectiveDefense);

        if (info.IsCritical)
            damage *= info.CriticalMultiplier;

        HP = Mathf.Max(0, HP - damage);
        OnDamageTaken?.Invoke(damage);

        if (HP <= 0 && !isDead)
        {
            isDead = true;
            OnDead?.Invoke();
        }

        return damage;
    }

    /// <summary>
    /// Simple version: only numeric damage
    /// </summary>
    public float TakeDamage(float dmg)
    {
        return TakeDamage(new DamageInfo(dmg));
    }

    public void Heal(float value)
    {
        if (isDead) return;

        float before = HP;
        HP = Mathf.Min(MaxHP, HP + value);

        float healed = HP - before;
        if (healed > 0)
            OnHeal?.Invoke(healed);
    }

    public bool IsAlive => !isDead && HP > 0;

    /// <summary>
    /// Add temporary invincibility
    /// </summary>
    public void AddInvincibility(float time)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(InvincibleRoutine(time));
    }

    private System.Collections.IEnumerator InvincibleRoutine(float time)
    {
        IsInvincible = true;
        yield return new WaitForSeconds(time);
        IsInvincible = false;
    }

    /// <summary>
    /// Reset HP and death status
    /// </summary>
    public void ResetAttribute()
    {
        isDead = false;
        HP = MaxHP;
        // Optionally reset invincibility
        IsInvincible = false;
        // Notify UI or other listeners that HP is fully restored
        OnHeal?.Invoke(MaxHP);
    }
}
