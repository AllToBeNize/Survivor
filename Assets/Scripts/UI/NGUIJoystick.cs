using UnityEngine;

public class NGUIJoystick : MonoBehaviour
{
    public float Horizontal => snapX ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x;
    public float Vertical => snapY ? SnapFloat(input.y, AxisOptions.Vertical) : input.y;
    public Vector2 Direction => new Vector2(Horizontal, Vertical);

    [Header("Settings")]
    public float handleRange = 1f;
    public float deadZone = 0.1f;
    public AxisOptions axisOptions = AxisOptions.Both;
    public bool snapX = false;
    public bool snapY = false;

    [Header("NGUI References")]
    public UIWidget background;
    public UIWidget handle;

    private Vector2 input = Vector2.zero;
    private Camera uiCamera;
    private Vector3 bgWorldPos;
    private float radius;

    private void Start()
    {
        if (background == null || handle == null)
        {
            Debug.LogError("NGUIJoystick: Background or Handle is null");
            enabled = false;
            return;
        }

        uiCamera = UICamera.mainCamera;
        bgWorldPos = background.transform.position;

        radius = Mathf.Min(background.width, background.height) * 0.5f;

        handle.transform.localPosition = Vector3.zero;
    }

    private void OnPress(bool pressed)
    {
        Debug.Log("OnPress: " + pressed);
        if (!pressed)
        {
            ResetJoystick();
        }
    }

    private void OnDrag(Vector2 delta)
    {
        Debug.Log("OnDrag: " + delta);
        if (uiCamera == null) return;

        Vector3 worldPos = UICamera.lastWorldPosition;
        Vector3 local = background.transform.InverseTransformPoint(worldPos);

        Vector2 pos = new Vector2(local.x, local.y);
        Vector2 normalized = pos / radius;

        input = normalized;
        FormatInput();

        float magnitude = input.magnitude;
        if (magnitude < deadZone)
        {
            input = Vector2.zero;
        }
        else if (magnitude > 1f)
        {
            input = input.normalized;
        }

        handle.transform.localPosition = new Vector3(
            input.x * radius * handleRange,
            input.y * radius * handleRange,
            0f
        );
    }

    private void ResetJoystick()
    {
        input = Vector2.zero;
        handle.transform.localPosition = Vector3.zero;
    }

    private void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return 0;

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);

            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                return value > 0 ? 1 : -1;
            }
            else
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                return value > 0 ? 1 : -1;
            }
        }
        else
        {
            return value > 0 ? 1 : -1;
        }
    }
}
