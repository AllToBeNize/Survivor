using System.Collections.Generic;
using UnityEngine;

public class Skill_SpinningBlade : SkillBase
{
    [Header("Blade Settings")]
    public float rotationSpeed = 180f;   // degrees per second
    public float attackRadius = 2f;      // Damage detection radius
    public float damagePerTick = 5f;     // DOT damage per tick
    public float damageInterval = 0.5f;  // Time between applying damage to same enemy
    public LayerMask damageLayer;

    [Header("Rotation Settings")]
    public float orbitRadius = 2f;       // Distance from mount point
    public float heightOffset = 0f;      // Y-axis offset relative to mount point

    private Vector3 rotationCenter;      // World-space rotation center
    private float currentAngle = 0f;     // 自身角度

    private Dictionary<AttributeBase, float> enemyLastDamageTime = new Dictionary<AttributeBase, float>();

    private AttributeBase attribute;

    protected override void OnEquip()
    {
        if (owner != null)
        {
            attribute = owner.GetComponent<AttributeBase>();
            attribute.OnDead += OnDead;
        }

        if (mountPoint != null)
        {
            rotationCenter = mountPoint.position + Vector3.up * heightOffset;
            transform.position = rotationCenter + mountPoint.forward * orbitRadius;

            transform.SetParent(null);
            transform.LookAt(rotationCenter, Vector3.up);
        }
    }

    private void OnDead()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isEquipped || mountPoint == null) return;

        // 更新旋转中心随玩家移动
        rotationCenter = mountPoint.position + Vector3.up * heightOffset;

        // 更新自身角度
        currentAngle += rotationSpeed * Time.deltaTime;
        if (currentAngle > 360f) currentAngle -= 360f;

        // 计算刀子当前位置
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * orbitRadius;
        transform.position = rotationCenter + offset;

        // 朝向旋转中心
        Vector3 lookDir = rotationCenter - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

        if (!PlayerManager.Instance.IsPlayerAlive()) { return; }

        // DOT伤害检测
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, damageLayer);
        float currentTime = Time.time;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) continue;

            var attr = hit.GetComponent<AttributeBase>();
            if (attr != null && attr.IsAlive)
            {
                // 检查是否到达DOT间隔
                if (!enemyLastDamageTime.ContainsKey(attr) || currentTime - enemyLastDamageTime[attr] >= damageInterval)
                {
                    DamageInfo dmg = new DamageInfo(damagePerTick, owner.gameObject);
                    attr.TakeDamage(dmg);

                    enemyLastDamageTime[attr] = currentTime;
                }
            }
        }

        // 清理死亡或离开的敌人
        List<AttributeBase> toRemove = new List<AttributeBase>();
        foreach (var kvp in enemyLastDamageTime)
        {
            if (kvp.Key == null || !kvp.Key.IsAlive)
                toRemove.Add(kvp.Key);
        }
        foreach (var r in toRemove)
            enemyLastDamageTime.Remove(r);
    }

    private void OnDrawGizmosSelected()
    {
        if (mountPoint == null) return;

        Vector3 center = mountPoint.position + Vector3.up * heightOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, orbitRadius);
    }
}
