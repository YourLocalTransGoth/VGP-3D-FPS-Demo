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
       UpdateHealthBar(1.00);
    }
   


    public void UpdateHealthBar( double updatedHealth)
    {
    
        healthBarSprite.fillAmount = (float)updatedHealth;

        if (updatedHealth <= 0)
        {
            Destroy(Player);
        }
    }
}
