using UnityEngine;

public enum WeaponType
{
    Main,
    Sub,
    Skill
}

[CreateAssetMenu(menuName = "Weapon/WeaponBase")]
public class WeaponBase : ScriptableObject
{
    [Header("Basic")]
    public WeaponType weaponType;
    public RotationPriority rotationPriority = RotationPriority.Shoot;

    [Header("Shoot")]
    public float fireInterval = 0.3f;
    public float range = 10f;
    public int damage = 25;

    [Header("Bullet")]
    //public BulletBase bulletPrefab;
    public float bulletSpeed = 10f;

    [Header("Shoot Point Offset")]
    public Vector3 shootOffset = Vector3.forward;
}
