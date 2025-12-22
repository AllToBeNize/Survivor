using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node_PlayerHealth : MonoBehaviour
{
    [Header("Health UI")]
    public List<Image> healthImages; // Assign 5 images in inspector
    public float updateSpeed = 0.5f; // seconds for smooth transition

    private int healthPerImage;
    private AttributeBase playerAttribute;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();

    private void Start()
    {
        playerAttribute = PlayerManager.Instance.GetPlayerAttribute();
        healthPerImage = Mathf.CeilToInt(playerAttribute.MaxHP / (float)healthImages.Count);

        // Subscribe to events
        playerAttribute.OnDamageTaken += OnHealthChanged;
        playerAttribute.OnHeal += OnHealthChanged;

        UpdateHealthUIInstant();
    }

    private void OnHealthChanged(float _)
    {
        // Stop any ongoing animations
        foreach (var c in activeCoroutines)
        {
            if (c != null) StopCoroutine(c);
        }
        activeCoroutines.Clear();

        for (int i = 0; i < healthImages.Count; i++)
        {
            float currentFill = healthImages[i].fillAmount;
            float targetFill = CalculateFillForIndex(i, playerAttribute.HP);
            Coroutine c = StartCoroutine(AnimateFill(healthImages[i], currentFill, targetFill));
            activeCoroutines.Add(c);
        }
    }

    private float CalculateFillForIndex(int index, float currentHP)
    {
        if (currentHP >= (index + 1) * healthPerImage)
            return 1f;
        else if (currentHP > index * healthPerImage)
            return (currentHP - index * healthPerImage) / healthPerImage;
        else
            return 0f;
    }

    private IEnumerator AnimateFill(Image img, float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < updateSpeed)
        {
            elapsed += Time.deltaTime;
            img.fillAmount = Mathf.Lerp(from, to, elapsed / updateSpeed);
            yield return null;
        }
        img.fillAmount = to;
    }

    private void UpdateHealthUIInstant()
    {
        float currentHP = playerAttribute.HP;
        for (int i = 0; i < healthImages.Count; i++)
        {
            healthImages[i].fillAmount = CalculateFillForIndex(i, currentHP);
        }
    }
}
