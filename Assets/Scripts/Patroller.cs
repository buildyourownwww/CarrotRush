
using UnityEngine;
using System.Collections;

public class Patroller : MonoBehaviour
{
    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float stoppingDistance = 0.1f;

    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private float losePlayerRange = 4f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float warningDuration = 0.5f;
    [SerializeField] private float attackCooldown = 1.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Transform currentTarget;
    private EnemyState currentState = EnemyState.Patrol;

    private bool playerDetected = false;
    private bool attackInProgress = false;
    private float nextAttackTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Start()
    {
        currentTarget = pointA;
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            Patrol();
            return;
        }

        float distance = Mathf.Abs(
            player.position.x - rb.position.x
        );

        // Update player detection
        if (playerDetected)
        {
            if (distance > losePlayerRange)
            {
                playerDetected = false;
            }
        }
        else if (distance <= detectionRange)
        {
            playerDetected = true;
        }

        // Finish the current attack warning first
        if (attackInProgress)
        {
            StopHorizontalMovement();
            return;
        }

        // Choose the state
        if (!playerDetected)
        {
            currentState = EnemyState.Patrol;
        }
        else if (distance <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else
        {
            currentState = EnemyState.Chase;
        }

        // Run the current state
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Attack:
                StopHorizontalMovement();

                if (Time.time >= nextAttackTime)
                {
                    StartCoroutine(AttackSequence());
                }
                break;
        }
    }

    void Patrol()
    {
        if (pointA == null || pointB == null)
        {
            StopHorizontalMovement();
            return;
        }

        float distance = Mathf.Abs(
            currentTarget.position.x - rb.position.x
        );

        if (distance <= stoppingDistance)
        {
            currentTarget =
                currentTarget == pointA ? pointB : pointA;
        }

        float direction = Mathf.Sign(
            currentTarget.position.x - rb.position.x
        );

        rb.linearVelocity = new Vector2(
            direction * patrolSpeed,
            rb.linearVelocity.y
        );
    }

    
    void Chase()
    {
        float distance = Mathf.Abs(
            player.position.x - rb.position.x
        );

        float direction = Mathf.Sign(
            player.position.x - rb.position.x
        );

        // Move slightly inside attack range
        float targetDistance = attackRange * 0.8f;

        float remainingDistance = Mathf.Max(
            0f,
            distance - targetDistance
        );

        float speed = Mathf.Min(
            chaseSpeed,
            remainingDistance / Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(
            direction * speed,
            rb.linearVelocity.y
        );
    }


    void StopHorizontalMovement()
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    
    
    IEnumerator AttackSequence()
    {
        attackInProgress = true;

        // Yellow warning
        spriteRenderer.color = Color.yellow;

        yield return new WaitForSeconds(warningDuration);

        spriteRenderer.color = originalColor;

        if (player != null)
        {
            float horizontalDistance = Mathf.Abs(
                player.position.x - rb.position.x
            );

            float verticalDistance = Mathf.Abs(
                player.position.y - rb.position.y
            );

            // Attack only if Player is close horizontally
            // AND at approximately the same height
            if (horizontalDistance <= attackRange &&
                verticalDistance <= 0.8f)
            {
                PlayerHealth playerHealth =
                    player.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                    Debug.Log("Patroller hit the Player!");
                }
            }
        }

        nextAttackTime = Time.time + attackCooldown;
        attackInProgress = false;
    }
}