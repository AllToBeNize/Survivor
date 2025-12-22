using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AttributeBase))]
[RequireComponent(typeof(WeaponController))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerController player;
    private CharacterController cc;
    private AttributeBase attribute;
    private WeaponController weaponController;

    [Header("Game Over Settings")]
    public float deathDelay = 1f; // 默认延迟，可通过动画长度调整

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        player = GetComponent<PlayerController>();
        cc = GetComponent<CharacterController>();
        attribute = GetComponent<AttributeBase>();
        weaponController = GetComponent<WeaponController>();
    }

    private void OnEnable()
    {
        if (attribute != null)
        {
            attribute.OnDamageTaken += OnDamageTaken;
            attribute.OnDead += OnDead;
        }

        if (weaponController != null)
        {
            weaponController.OnWeaponFired += OnWeaponFired;
        }
    }

    private void OnDisable()
    {
        if (attribute != null)
        {
            attribute.OnDamageTaken -= OnDamageTaken;
            attribute.OnDead -= OnDead;
        }

        if (weaponController != null)
        {
            weaponController.OnWeaponFired -= OnWeaponFired;
        }
    }

    private void Update()
    {
        if (animator == null || player == null || cc == null || !attribute.IsAlive)
            return;

        float speed = cc.velocity.magnitude;
        animator.SetFloat("MoveSpeed", speed);
    }

    private void OnWeaponFired()
    {
        animator?.SetTrigger("Shoot");
    }

    private void OnDamageTaken(float dmg)
    {
        animator?.SetTrigger("Hit");
    }

    private void OnDead()
    {
        animator?.SetTrigger("Die");

        StartCoroutine(DelayedGameOver(deathDelay));
    }

    private IEnumerator DelayedGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 调用游戏管理器的游戏结束逻辑
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameOver(GameOverReason.Dead);
        }
    }
}
