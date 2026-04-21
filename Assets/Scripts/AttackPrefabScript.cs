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
        GameObject hitObject = collision.gameObject;
        GameObject hitRoot = hitObject.transform.root.gameObject;
        bool isPlayerHit = hitObject.CompareTag("Player")
            || hitRoot.CompareTag("Player")
            || hitRoot.GetComponentInChildren<PlayerMovement>() != null;

        if (isPlayerHit)
        {
            GunShoot.enemyDestroyed++;
            if (healthBar != null)
            {
                healthBar.TakeDamage(0.2f);
            }

            Destroy(gameObject);
        }
    }

}
