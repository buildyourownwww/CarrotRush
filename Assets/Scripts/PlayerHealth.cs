
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invulnerabilityTime = 1f;

    [Header("Health UI")]
    [SerializeField] private GameObject heart1;
    [SerializeField] private GameObject heart2;
    [SerializeField] private GameObject heart3;

    private int currentHealth;
    private bool isInvulnerable = false;
    private bool isDead = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        currentHealth = maxHealth;
    }

    void Start()
    {
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isDead)
            return;

        currentHealth = Mathf.Max(
            0,
            currentHealth - damage
        );

        Debug.Log("Player HP: " + currentHealth);

        UpdateHearts();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageFlash());
        }
    }

    void UpdateHearts()
    {
        if (heart1 != null)
            heart1.SetActive(currentHealth >= 1);

        if (heart2 != null)
            heart2.SetActive(currentHealth >= 2);

        if (heart3 != null)
            heart3.SetActive(currentHealth >= 3);
    }

    IEnumerator DamageFlash()
    {
        isInvulnerable = true;

        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(
            Mathf.Max(0f, invulnerabilityTime - 0.2f)
        );

        isInvulnerable = false;
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player died!");

        spriteRenderer.color = Color.gray;

        // Disable movement when Player dies
        PlayerMovement movement =
            GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.enabled = false;
        }
    }
}
