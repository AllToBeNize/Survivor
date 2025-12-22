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
    private int lastRequestFrame = -1;

    public void SetDirection(Vector3 dir, RotationPriority priority)
    {
        if (dir.sqrMagnitude < 0.0001f)
            return;

        Debug.Log(
            $"[Rotation] SetDirection frame={Time.frameCount} " +
            $"priority={priority} dir={dir}"
        );

        if (Time.frameCount != lastRequestFrame)
        {
            currentPriority = RotationPriority.None;
        }

        if (priority < currentPriority)
        {
            Debug.Log(
                $"[Rotation] Rejected by priority. " +
                $"current={currentPriority}, incoming={priority}"
            );
            return;
        }

        dir.y = 0f;
        desiredDir = dir.normalized;
        currentPriority = priority;
        lastRequestFrame = Time.frameCount;
    }

    void Update()
    {
        if (lastRequestFrame != Time.frameCount)
            return;

        if (desiredDir == Vector3.zero)
            return;

        Debug.Log(
            $"[Rotation] Apply rotation frame={Time.frameCount} " +
            $"dir={desiredDir} forward(before)={transform.forward}"
        );

        Quaternion targetRot = Quaternion.LookRotation(desiredDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );

        Debug.Log(
            $"[Rotation] forward(after)={transform.forward}"
        );
    }
}
