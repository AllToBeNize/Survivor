using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Input")]
    public Joystick joystick;

    [Header("Camera")]
    public Transform cameraTransform;

    private PlayerRotationController rotationController;
    private CharacterController controller;
    private AttributeBase attribute;
    private Vector3 moveDir;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        rotationController = GetComponent<PlayerRotationController>();
        attribute = GetComponent<AttributeBase>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        PlayerManager.Instance.InitPlayer(gameObject);
    }

    void Update()
    {
        if (!attribute.IsAlive)
        {
            return;
        }

        ReadInput();
        Move();
        ApplyRotation();
    }

    void ReadInput()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDir = camForward * v + camRight * h;

        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        // dead zone
        if (moveDir.magnitude < 0.2f)
            moveDir = Vector3.zero;
    }

    void Move()
    {
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }
    void ApplyRotation()
    {
        if (moveDir == Vector3.zero)
            return;

        rotationController.SetDirection(
            moveDir,
            RotationPriority.Move
        );
    }
}
