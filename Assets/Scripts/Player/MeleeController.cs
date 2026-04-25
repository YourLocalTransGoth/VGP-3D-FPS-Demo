using System.Collections;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [Header("Weapon")]
    public GameObject Wrench;
    [SerializeField] private string attackTriggerName = "attack";

    [Header("Timing")]
    public bool CanAttack = true;
    public float AttackCooldown = 1f;
    [SerializeField] private float hitDelay = 0.12f;

    [Header("Damage")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private LayerMask enemyLayers = ~0;
    [SerializeField] private Vector3 attackOffset = new Vector3(0f, 0f, 0.75f);

    private Animator weaponAnimator;

    private void Awake()
    {
        CacheAnimator();
    }

    private void Update()
    {
        if (GameStateController.IsGamePaused)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && CanAttack)
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (GameStateController.IsGamePaused)
        {
            return;
        }

        if (!CanAttack)
        {
            return;
        }

        CanAttack = false;

        if (!CacheAnimator())
        {
            CanAttack = true;
            return;
        }

        weaponAnimator.ResetTrigger(attackTriggerName);
        weaponAnimator.SetTrigger(attackTriggerName);

        StartCoroutine(AttackRoutine());
    }

    private bool CacheAnimator()
    {
        if (weaponAnimator != null)
        {
            return true;
        }

        if (Wrench == null)
        {
            Debug.LogWarning("MeleeController: Wrench is not assigned.");
            return false;
        }

        weaponAnimator = Wrench.GetComponent<Animator>();
        if (weaponAnimator == null)
        {
            weaponAnimator = Wrench.GetComponentInChildren<Animator>();
        }

        if (weaponAnimator == null)
        {
            Debug.LogWarning("MeleeController: No Animator found on Wrench or its children.");
            return false;
        }

        return true;
    }

    private IEnumerator AttackRoutine()
    {
        if (hitDelay > 0f)
        {
            yield return new WaitForSeconds(hitDelay);
        }

        DealDamage();

        if (AttackCooldown > 0f)
        {
            yield return new WaitForSeconds(AttackCooldown);
        }

        CanAttack = true;
    }

    private void DealDamage()
    {
        if (Wrench == null)
        {
            return;
        }

        Transform origin = Wrench.transform;
        Vector3 center = origin.position + origin.forward * attackOffset.z + origin.right * attackOffset.x + origin.up * attackOffset.y;
        int layers = enemyLayers.value == 0 ? Physics.DefaultRaycastLayers : enemyLayers.value;

        Collider[] hits = Physics.OverlapSphere(center, attackRange, layers, QueryTriggerInteraction.Ignore);
        foreach (Collider hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth == null)
            {
                continue;
            }

            enemyHealth.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Wrench == null)
        {
            return;
        }

        Transform origin = Wrench.transform;
        Vector3 center = origin.position + origin.forward * attackOffset.z + origin.right * attackOffset.x + origin.up * attackOffset.y;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);
    }
}
