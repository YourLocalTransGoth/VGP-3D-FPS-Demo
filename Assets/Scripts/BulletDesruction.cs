using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDesctrucion : MonoBehaviour

{
    public float bulletSpan = 5000;
    public float damageAmount = 1f;

    private bool hasHit;

    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, bulletSpan);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamageEnemy(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamageEnemy(other);
    }

    private void TryDamageEnemy(Collider other)
    {
        if (hasHit)
        {
            return;
        }

        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
        if (enemyHealth == null)
        {
            return;
        }

        hasHit = true;
        enemyHealth.TakeDamage(damageAmount);
        Destroy(gameObject);
    }

 


    // Update is called once per frame
    void Update()
    {
        
    }
}