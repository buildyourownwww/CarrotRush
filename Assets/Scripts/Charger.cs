
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

    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 4f;

    [Header("Charge Settings")]
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float chargeDuration = 0.5f;
    [SerializeField] private float warningDuration = 0.5f;
    [SerializeField] private float recoveryDuration = 1.5f;

    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitRangeX = 0.8f;
    [SerializeField] private float hitRangeY = 0.8f;

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
    }

    void Update()
    {
        if (player == null || isBusy)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= detectionRange)
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

        if (distanceX <= hitRangeX &&
            distanceY <= hitRangeY)
        {
            PlayerHealth playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
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
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        spriteRenderer.color = Color.yellow;

        yield return new WaitForSeconds(warningDuration);

        // Charge toward the Player
        currentState = EnemyState.Charge;
        spriteRenderer.color = originalColor;

        float direction = Mathf.Sign(
            player.position.x - transform.position.x
        );

        rb.linearVelocity = new Vector2(
            direction * chargeSpeed,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(chargeDuration);

        // Stop
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // Recover: gray
        currentState = EnemyState.Recover;
        spriteRenderer.color = Color.gray;

        yield return new WaitForSeconds(recoveryDuration);

        spriteRenderer.color = originalColor;
        currentState = EnemyState.Idle;
        isBusy = false;
    }
}
