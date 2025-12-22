using UnityEngine;

public enum RotationPriority
{
    None = -1,
    Move = 0,
    Shoot = 10,
    SubWeapon = 20,
    Skill = 30,
    Force = 50
}

public class PlayerRotationController : MonoBehaviour
{
    public float rotateSpeed = 15f;

    private Vector3 desiredDir;
    private RotationPriority currentPriority = RotationPriority.None;
    private bool hasRequest;

    public void SetDirection(Vector3 dir, RotationPriority priority)
    {
        if (dir.sqrMagnitude < 0.0001f)
            return;

        if (priority < currentPriority)
            return;

        dir.y = 0f;
        desiredDir = dir.normalized;
        currentPriority = priority;
        hasRequest = true;

        //Debug.Log($"[Rotation] Accept {priority} dir={desiredDir}");
    }

    void LateUpdate()
    {
        if (!hasRequest)
            return;

        Quaternion targetRot = Quaternion.LookRotation(desiredDir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotateSpeed * 360f * Time.deltaTime
        );

        hasRequest = false;
        currentPriority = RotationPriority.None;
    }
}
