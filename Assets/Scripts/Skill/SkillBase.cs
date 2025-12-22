using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    [Header("Mount Point Settings")]
    public string mountPointName = "MountPoint"; // Set in Inspector

    protected Transform owner;       // Player
    protected Transform mountPoint;  // Found by name
    protected bool isEquipped = false;

    /// <summary>
    /// Equip skill to player, find mount point by name
    /// </summary>
    public virtual void Equip(Transform ownerTransform)
    {
        owner = ownerTransform;
        if (owner == null)
        {
            Debug.LogWarning($"{name} Equip failed: owner is null");
            return;
        }

        // Find mount point by name
        mountPoint = owner.Find(mountPointName);
        if (mountPoint == null)
        {
            Debug.LogWarning($"{name} cannot find mount point '{mountPointName}' on {owner.name}");
            return;
        }

        isEquipped = true;
        gameObject.SetActive(true);

        // Parent to mount point
        transform.SetParent(mountPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        OnEquip();
    }

    protected abstract void OnEquip();

    public virtual void Unequip()
    {
        isEquipped = false;
        gameObject.SetActive(false);
    }
}
