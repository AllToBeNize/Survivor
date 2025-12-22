using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage;
    public Image foregroundImage;

    [Header("Target")]
    private AttributeBase target;

    private Camera mainCam;
    private float originalWidth;

    private void Start()
    {
        mainCam = Camera.main;

        target = GetComponentInParent<AttributeBase>();

        if (target != null)
        {
            target.OnDamageTaken += UpdateHealth;
            target.OnHeal += UpdateHealth;
            target.OnDead += OnDead;

            UpdateHealth(0);
        }

        if (foregroundImage != null)
            originalWidth = foregroundImage.rectTransform.sizeDelta.x;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 camForward = mainCam.transform.forward;
         camForward.y = 0;
        if (camForward.sqrMagnitude > 0.001f)
            transform.forward = camForward.normalized;
    }

    private void UpdateHealth(float _)
    {
        if (target == null || foregroundImage == null) return;

        if (foregroundImage.type == Image.Type.Filled)
        {
            foregroundImage.fillAmount = target.HP / target.MaxHP;
        }
        else
        {
            float ratio = Mathf.Clamp01(target.HP / target.MaxHP);
            Vector2 size = foregroundImage.rectTransform.sizeDelta;
            size.x = originalWidth * ratio;
            foregroundImage.rectTransform.sizeDelta = size;
        }
    }

    private void OnDead()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (target != null)
        {
            target.OnDamageTaken -= UpdateHealth;
            target.OnHeal -= UpdateHealth;
            target.OnDead -= OnDead;
        }
    }
}
