
using UnityEngine;
using System.Collections;

public class Charger : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Prepare,
        Charge,
        Recover
    }

    [Header("Enemy Data")]
    [SerializeField] private EnemyStats stats;

    [Header("Player")]
    [SerializeField] private Transform player;
    public void SetPlayer(Transform target)
    {
        player = target;
    }

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private EnemyState currentState = EnemyState.Idle;
    private bool isBusy = false;
    private bool hitThisCharge = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (stats == null)
        {
            Debug.LogError("ChargerStats is missing!", this);
            enabled = false;
        }
    }

    void Update()
    {
        if (player == null || isBusy)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= stats.detectionRange)
        {
            StartCoroutine(ChargeSequence());
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    void FixedUpdate()
    {
        if (currentState != EnemyState.Charge ||
            player == null || hitThisCharge)
            return;

        float distanceX = Mathf.Abs(
            player.position.x - transform.position.x
        );

        float distanceY = Mathf.Abs(
            player.position.y - transform.position.y
        );

        if (distanceX <= stats.hitRangeX &&
            distanceY <= stats.hitRangeY)
        {
            PlayerHealth playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(stats.damage);
                hitThisCharge = true;

                Debug.Log("Charger hit the Player!");
            }
        }
    }

    IEnumerator ChargeSequence()
    {
        isBusy = true;
        hitThisCharge = false;

        // Prepare: yellow warning
        currentState = EnemyState.Prepare;
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        spriteRenderer.color = Color.yellow;

        yield return new WaitForSeconds(stats.windUpTime);

        // Stop safely if the player disappeared
        if (player == null)
        {
            spriteRenderer.color = originalColor;
            currentState = EnemyState.Idle;
            isBusy = false;
            yield break;
        }

        // Charge toward the Player
        currentState = EnemyState.Charge;
        spriteRenderer.color = originalColor;

        float direction = Mathf.Sign(
            player.position.x - transform.position.x
        );

        rb.linearVelocity = new Vector2(
            direction * stats.chargeSpeed,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(stats.chargeDuration);

        // Stop
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        // Recover: gray
        currentState = EnemyState.Recover;
        spriteRenderer.color = Color.gray;

        yield return new WaitForSeconds(stats.recoveryDuration);

        spriteRenderer.color = originalColor;
        currentState = EnemyState.Idle;
        isBusy = false;
    }
}
