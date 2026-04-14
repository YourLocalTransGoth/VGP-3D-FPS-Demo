using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AttackPrefabScript : MonoBehaviour
{
    public float life = 3;
    public HealthBar healthBar; 
    // Start is called before the first frame update
    void Awake()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        Destroy(gameObject, life);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GunShoot.enemyDestroyed++;
                double updatedHealth = healthBar.healthBarSprite.fillAmount - 0.2;
            healthBar.UpdateHealthBar(updatedHealth);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

}
