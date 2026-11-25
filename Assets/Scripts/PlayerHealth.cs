
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private PlayerStats stats;

    [Header("Health UI")]
    [SerializeField] private GameObject heart1;
    [SerializeField] private GameObject heart2;
    [SerializeField] private GameObject heart3;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;

    private int currentHealth;
    private bool isInvulnerable = false;
    private bool isDead = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (stats == null)
        {
            Debug.LogError("PlayerStats is missing!", this);
            enabled = false;
            return;
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        currentHealth = stats.maxHealth;
    }

    void Start()
    {
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isDead || !enabled)
            return;

        // Play hit sound with random pitch
        if (hitAudioSource != null && hitSound != null)
        {
            hitAudioSource.pitch = Random.Range(0.65f, 1.35f);
            Debug.Log("Hit sound pitch: " + hitAudioSource.pitch);
            hitAudioSource.PlayOneShot(hitSound);
        }

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

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        float flashDuration = Mathf.Clamp(
            stats.damageFlashDuration,
            0f,
            stats.invulnerabilityDuration
        );

        yield return new WaitForSeconds(flashDuration);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        yield return new WaitForSeconds(
            Mathf.Max(
                0f,
                stats.invulnerabilityDuration - flashDuration
            )
        );

        isInvulnerable = false;
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player died!");

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.gray;
        }

        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.enabled = false;
        }

        LevelFlow levelFlow = FindFirstObjectByType<LevelFlow>();

        if (levelFlow != null)
        {
            levelFlow.FinishLevel(
                false,
                true,
                deathSound != null ? deathSound.length : 0f
            );
        }
    }
}
