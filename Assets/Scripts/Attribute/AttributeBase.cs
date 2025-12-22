using UnityEngine;
using System;

public struct DamageInfo
{
    public float BaseDamage;      // 原始伤害
    public float ArmorPenetration; // 护甲穿透 0~1
    public bool IsCritical;       // 暴击？
    public float CriticalMultiplier; // 暴击倍率（如 2x）

    public DamageInfo(float dmg)
    {
        BaseDamage = dmg;
        ArmorPenetration = 0;
        IsCritical = false;
        CriticalMultiplier = 1f;
    }
}

public class AttributeBase : MonoBehaviour
{
    [Header("Attributes")]
    public float MaxHP = 100f;
    public float HP = 100f;
    public float Defense = 0f;

    public bool IsInvincible = false;   // 无敌状态
    private bool isDead = false;        // 确保死亡只触发一次

    public event Action OnDead;
    public event Action<float> OnDamageTaken;   // 最终伤害
    public event Action<float> OnHeal;

    /// <summary>
    /// 处理伤害（带 DamageInfo 扩展）
    /// </summary>
    public float TakeDamage(DamageInfo info)
    {
        if (IsInvincible || isDead) return 0f;

        // 护甲计算：Defense * (1 - 穿透)
        float effectiveDefense = Defense * (1f - info.ArmorPenetration);

        // 最终伤害
        float damage = Mathf.Max(0, info.BaseDamage - effectiveDefense);

        // 暴击
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
    /// 简单版本：仅数值伤害
    /// </summary>
    public float TakeDamage(float dmg)
    {
        return TakeDamage(new DamageInfo(dmg));
    }

    public void Heal(float value)
    {
        if (isDead) return; // 死了不能加血（如果你想支持复活可删除）

        float before = HP;
        HP = Mathf.Min(MaxHP, HP + value);

        float healed = HP - before;
        if (healed > 0)
            OnHeal?.Invoke(healed);
    }

    public bool IsAlive => !isDead && HP > 0;

    /// <summary>
    /// 给角色添加无敌时间
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
}

