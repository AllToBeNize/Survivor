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

    [Header("Shoot Settings")]
    public float fireInterval = 0.3f;
    public float range = 10f;
    public int damage = 25;

    [Header("Bullet Settings")]
    public float bulletSpeed = 20f;

    [Header("Shoot Point Offset")]
    public Vector3 shootOffset = Vector3.forward;

    [Header("Model")]
    public GameObject modelPrefab;         // Weapon model prefab
    public string mountPointName = "";     // Name of the transform on player to attach
    public string muzzlePointName = "";    // Name of the muzzle transform on the model
}
