
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

    [Header("Enemy Data")]
    [SerializeField] private EnemyStats stats;

    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Player")]
    [SerializeField] private Transform player;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Vector2 patrolA;
    private Vector2 patrolB;
    private bool goingToA = true;

    private EnemyState currentState = EnemyState.Patrol;

    private bool playerDetected = false;
    private bool attackInProgress = false;
    private float nextAttackTime = 0f;

    // Connect Player when enemy is spawned
    public void SetPlayer(Transform target)
    {
        player = target;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (stats == null)
        {
            Debug.LogError("PatrollerStats is missing!", this);
            enabled = false;
        }
    }

    void Start()
    {
        // Save patrol positions
        if (pointA != null && pointB != null)
        {
            patrolA = pointA.position;
            patrolB = pointB.position;
        }
    }

    void FixedUpdate()
    {
        if (attackInProgress)
        {
            StopHorizontalMovement();
            return;
        }

        if (player == null)
        {
            currentState = EnemyState.Patrol;
            Patrol();
            return;
        }

        float distance = Mathf.Abs(
            player.position.x - rb.position.x
        );

        // Player detection
        if (playerDetected)
        {
            if (distance > stats.losePlayerRange)
            {
                playerDetected = false;
            }
        }
        else if (distance <= stats.detectionRange)
        {
            playerDetected = true;
        }

        // Choose state
        if (!playerDetected)
        {
            currentState = EnemyState.Patrol;
        }
        else if (distance <= stats.attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else
        {
            currentState = EnemyState.Chase;
        }

        // Run current state
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

        Vector2 target = goingToA ? patrolA : patrolB;

        float distance = Mathf.Abs(
            target.x - rb.position.x
        );

        if (distance <= stats.stoppingDistance)
        {
            goingToA = !goingToA;
            target = goingToA ? patrolA : patrolB;
        }

        float direction = Mathf.Sign(
            target.x - rb.position.x
        );

        rb.linearVelocity = new Vector2(
            direction * stats.patrolSpeed,
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
        float targetDistance =
            stats.attackRange * stats.chaseTargetMultiplier;

        float remainingDistance = Mathf.Max(
            0f,
            distance - targetDistance
        );

        float speed = Mathf.Min(
            stats.chaseSpeed,
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

        // Yellow warning before attack
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }

        yield return new WaitForSeconds(
            stats.warningDuration
        );

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Damage Player if still close
        if (player != null)
        {
            float horizontalDistance = Mathf.Abs(
                player.position.x - rb.position.x
            );

            float verticalDistance = Mathf.Abs(
                player.position.y - rb.position.y
            );

            if (horizontalDistance <= stats.attackRange &&
                verticalDistance <= stats.attackHeightTolerance)
            {
                PlayerHealth playerHealth =
                    player.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(stats.damage);

                    Debug.Log("Patroller hit the Player!");
                }
            }
        }

        nextAttackTime = Time.time + stats.attackCooldown;
        attackInProgress = false;
    }
}
