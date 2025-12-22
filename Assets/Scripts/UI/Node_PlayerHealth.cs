using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node_PlayerHealth : MonoBehaviour
{
    [Header("Health UI")]
    public List<Image> healthImages; // Assign 5 images in inspector

    private int healthPerImage;
    private AttributeBase playerAttribute;

    private void Start()
    {
        playerAttribute = PlayerManager.Instance.GetPlayerAttribute();

        healthPerImage = Mathf.CeilToInt(playerAttribute.MaxHP / (float)healthImages.Count);

        // Subscribe to events
        playerAttribute.OnDamageTaken += OnDamageTaken;
        playerAttribute.OnHeal += OnHealed;

        UpdateHealthUI();
    }

    private void OnDamageTaken(float dmg)
    {
        UpdateHealthUI();
    }

    private void OnHealed(float amount)
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        float currentHP = playerAttribute.HP;

        for (int i = 0; i < healthImages.Count; i++)
        {
            if (currentHP >= (i + 1) * healthPerImage)
            {
                healthImages[i].fillAmount = 1f;
            }
            else if (currentHP > i * healthPerImage)
            {
                float partial = (currentHP - i * healthPerImage) / healthPerImage;
                healthImages[i].fillAmount = partial;
            }
            else
            {
                healthImages[i].fillAmount = 0f;
            }
        }
    }
}
