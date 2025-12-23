using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage;
    public Image foregroundImage;

    [Header("Animation Settings")]
    public float updateSpeed = 0.5f; // seconds to smoothly update

    private AttributeBase target;
    private Camera mainCam;
    private CanvasGroup canvasGroup;

    private float originalWidth;
    private Coroutine updateCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        mainCam = Camera.main != null ? Camera.main : FindObjectOfType<Camera>();
        target = GetComponentInParent<AttributeBase>();

        if (foregroundImage != null)
            originalWidth = foregroundImage.rectTransform.sizeDelta.x;
    }

    private void OnEnable()
    {
        if (target == null) return;

        target.OnDamageTaken += OnHealthChanged;
        target.OnHeal += OnHealthChanged;
        target.OnDead += OnDead;

        // 对象池复用：立刻刷新
        UpdateHealthInstant();
        RefreshVisible();
    }

    private void OnDisable()
    {
        if (target == null) return;

        target.OnDamageTaken -= OnHealthChanged;
        target.OnHeal -= OnHealthChanged;
        target.OnDead -= OnDead;

        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }

    private void LateUpdate()
    {
        if (mainCam == null) return;

        Vector3 dir = transform.position - mainCam.transform.position;
        dir.y = 0f; // 锁死垂直方向（不仰不俯）

        if (dir.sqrMagnitude < 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(dir);
    }


    private void OnHealthChanged(float _)
    {
        RefreshVisible();

        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);

        updateCoroutine = StartCoroutine(UpdateHealthSmooth());
    }

    private void OnDead()
    {
        RefreshVisible();
    }

    /// <summary>
    /// 控制显示 / 隐藏（不 SetActive）
    /// </summary>
    private void RefreshVisible()
    {
        if (target == null) return;

        bool shouldShow = target.IsAlive && target.HP > 0f;

        canvasGroup.alpha = shouldShow ? 1f : 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator UpdateHealthSmooth()
    {
        if (foregroundImage == null || target == null)
            yield break;

        float startValue;
        float targetValue = Mathf.Clamp01(target.HP / target.MaxHP);

        if (foregroundImage.type == Image.Type.Filled)
            startValue = foregroundImage.fillAmount;
        else
            startValue = foregroundImage.rectTransform.sizeDelta.x / originalWidth;

        float elapsed = 0f;

        while (elapsed < updateSpeed)
        {
            elapsed += Time.deltaTime;
            float ratio = Mathf.Lerp(startValue, targetValue, elapsed / updateSpeed);

            ApplyValue(ratio);
            yield return null;
        }

        ApplyValue(targetValue);
        updateCoroutine = null;
    }

    private void UpdateHealthInstant()
    {
        if (foregroundImage == null || target == null)
            return;

        float ratio = Mathf.Clamp01(target.HP / target.MaxHP);
        ApplyValue(ratio);
    }

    private void ApplyValue(float ratio)
    {
        if (foregroundImage.type == Image.Type.Filled)
        {
            foregroundImage.fillAmount = ratio;
        }
        else
        {
            Vector2 size = foregroundImage.rectTransform.sizeDelta;
            size.x = originalWidth * ratio;
            foregroundImage.rectTransform.sizeDelta = size;
        }
    }
}
