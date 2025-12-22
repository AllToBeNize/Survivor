using UnityEngine;

/// <summary>
/// 旋转优先级（数值越大，优先级越高）
/// </summary>
public enum RotationPriority
{
    None = -1,

    Move = 0,       // 移动朝向
    Shoot = 10,     // 主武器射击
    SubWeapon = 20, // 悬浮机关枪
    Skill = 30,     // 技能 / 大招
    Force = 50      // 剧情 / 强制朝向
}

/// <summary>
/// 玩家旋转控制器（带优先级，按帧生效）
/// </summary>
public class PlayerRotationController : MonoBehaviour
{
    [Header("Rotate")]
    public float rotateSpeed = 15f;

    private Vector3 desiredDir;
    private RotationPriority currentPriority = RotationPriority.None;

    private int lastRequestFrame = -1;

    public void SetDirection(Vector3 dir, RotationPriority priority)
    {
        if (dir.sqrMagnitude < 0.0001f)
            return;

        if (Time.frameCount != lastRequestFrame)
        {
            currentPriority = RotationPriority.None;
        }

        if (priority < currentPriority)
            return;

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

        Quaternion targetRot = Quaternion.LookRotation(desiredDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}
