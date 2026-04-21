using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 3f;

    [SerializeField] private Sprite healthBarSprite;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0f, 2.2f, 0f);

    private float currentHealth;
    private Transform healthBar;

    private void Awake()
    {
        currentHealth = Mathf.Max(1f, maxHealth);

        if (healthBarSprite != null)
        {
            CreateBar();
        }

        UpdateBar();
    }

    private void LateUpdate()
    {
        if (healthBar == null || Camera.main == null)
        {
            return;
        }

        healthBar.rotation = Camera.main.transform.rotation;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damageAmount);
        UpdateBar();

        if (currentHealth <= 0f)
        {
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null)
            {
                player.numOfEnDest++;
            }

            Destroy(gameObject);
        }
    }

    private void CreateBar()
    {
        GameObject barObject = new GameObject("EnemyHealthBar");
        barObject.transform.SetParent(transform, false);
        barObject.transform.localPosition = healthBarOffset;

        SpriteRenderer barRenderer = barObject.AddComponent<SpriteRenderer>();
        barRenderer.sprite = healthBarSprite;
        barRenderer.sortingOrder = 200;

        healthBar = barObject.transform;
    }

    private void UpdateBar()
    {
        if (healthBar == null)
        {
            return;
        }

        float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
        healthBar.localScale = new Vector3(healthPercent, 0.2f, 1f);
    }
}