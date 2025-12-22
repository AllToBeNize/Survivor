using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage;
    public Image foregroundImage;

    [Header("Target")]
    private AttributeBase target;

    private Camera mainCam;
    private float originalWidth;

    [Header("Animation Settings")]
    public float updateSpeed = 0.5f; // seconds to smoothly update

    private Coroutine updateCoroutine;

    private void Awake()
    {
        mainCam = Camera.main;

        target = GetComponentInParent<AttributeBase>();

        if (foregroundImage != null)
            originalWidth = foregroundImage.rectTransform.sizeDelta.x;
    }

    private void OnEnable()
    {
        if (target != null)
        {
            target.OnDamageTaken += OnHealthChanged;
            target.OnHeal += OnHealthChanged;
            target.OnDead += OnDead;

            // Refresh health immediately when enabled
            UpdateHealthInstant();
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0;
        if (camForward.sqrMagnitude > 0.001f)
            transform.forward = camForward.normalized;
    }

    private void OnHealthChanged(float _)
    {
        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);

        updateCoroutine = StartCoroutine(UpdateHealthSmooth());
    }

    private IEnumerator UpdateHealthSmooth()
    {
        if (foregroundImage == null || target == null) yield break;

        float startValue = 0f;
        float targetValue = target.HP / target.MaxHP;

        if (foregroundImage.type == Image.Type.Filled)
            startValue = foregroundImage.fillAmount;
        else
            startValue = foregroundImage.rectTransform.sizeDelta.x / originalWidth;

        float elapsed = 0f;

        while (elapsed < updateSpeed)
        {
            elapsed += Time.deltaTime;
            float ratio = Mathf.Lerp(startValue, targetValue, elapsed / updateSpeed);

            if (foregroundImage.type == Image.Type.Filled)
                foregroundImage.fillAmount = ratio;
            else
            {
                Vector2 size = foregroundImage.rectTransform.sizeDelta;
                size.x = originalWidth * ratio;
                foregroundImage.rectTransform.sizeDelta = size;
            }

            yield return null;
        }

        // Ensure final value
        if (foregroundImage.type == Image.Type.Filled)
            foregroundImage.fillAmount = targetValue;
        else
        {
            Vector2 size = foregroundImage.rectTransform.sizeDelta;
            size.x = originalWidth * targetValue;
            foregroundImage.rectTransform.sizeDelta = size;
        }
    }

    private void UpdateHealthInstant()
    {
        if (foregroundImage == null || target == null) return;

        float ratio = target.HP / target.MaxHP;

        if (foregroundImage.type == Image.Type.Filled)
            foregroundImage.fillAmount = ratio;
        else
        {
            Vector2 size = foregroundImage.rectTransform.sizeDelta;
            size.x = originalWidth * ratio;
            foregroundImage.rectTransform.sizeDelta = size;
        }
    }

    private void OnDead()
    {
        target.HP = 0f;
        OnHealthChanged(0);
    }

    private void OnDisable()
    {
        if (target != null)
        {
            target.OnDamageTaken -= OnHealthChanged;
            target.OnHeal -= OnHealthChanged;
            target.OnDead -= OnDead;
        }
    }
}
