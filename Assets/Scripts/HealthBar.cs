using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
   
    public Image healthBarSprite;
    public GameObject Player;

    void Awake()
    {
        if (healthBarSprite == null)
        {
            healthBarSprite = GetComponent<Image>();
        }

        if (healthBarSprite != null)
        {
            healthBarSprite.type = Image.Type.Filled;
        }

        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        UpdateHealthBar(1f);
    }
   


    public void UpdateHealthBar(float updatedHealth)
    {
        if (healthBarSprite == null)
        {
            return;
        }
    
        float clampedHealth = Mathf.Clamp01(updatedHealth);
        healthBarSprite.fillAmount = clampedHealth;

        if (clampedHealth <= 0f && Player != null)
        {
            Destroy(Player);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (healthBarSprite == null)
        {
            return;
        }

        UpdateHealthBar(healthBarSprite.fillAmount - damageAmount);
    }
}
