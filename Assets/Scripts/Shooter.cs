
using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Aim,
        Shoot
    }

    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float fieldOfView = 120f;
    [SerializeField] private float facingAngle = 225f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Attack Settings")]
    [SerializeField] private float warningDuration = 0.5f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Bullet Settings")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private EnemyState currentState = EnemyState.Idle;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Check distance, viewing angle, and walls
    bool CanSeePlayer()
    {
        if (player == null || firePoint == null)
            return false;

            // Check whether Shooter is visible on screen
            Camera cam = Camera.main;

            if (cam != null)
            {
                Vector3 screenPos =
                    cam.WorldToViewportPoint(transform.position);

                if (screenPos.z <= 0f ||
                    screenPos.x < 0f || screenPos.x > 1f ||
                    screenPos.y < 0f || screenPos.y > 1f)
                {
                    return false;
                }
            }

        Vector2 start = firePoint.position;
        Vector2 target = player.position;

        Vector2 direction = target - start;
        float distance = direction.magnitude;

        // 1. Distance check
        if (distance > detectionRange)
            return false;

        // 2. Field of View angle check
        Vector2 facingDirection = new Vector2(
            Mathf.Cos(facingAngle * Mathf.Deg2Rad),
            Mathf.Sin(facingAngle * Mathf.Deg2Rad)
        );

        float angleToPlayer = Vector2.Angle(
            facingDirection,
            direction
        );

        if (angleToPlayer > fieldOfView / 2f)
            return false;

        // 3. Wall raycast check
        RaycastHit2D hit = Physics2D.Raycast(
            start,
            direction.normalized,
            distance,
            wallLayer
        );

        return hit.collider == null;
    }

    void Update()
    {
        if (player == null || isAttacking)
            return;

        if (CanSeePlayer() &&
            Time.time >= nextAttackTime)
        {
            currentState = EnemyState.Aim;
            StartCoroutine(AttackSequence());
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;
        currentState = EnemyState.Aim;

        // Yellow warning before shooting
        spriteRenderer.color = Color.yellow;

        yield return new WaitForSeconds(warningDuration);

        spriteRenderer.color = originalColor;
        currentState = EnemyState.Shoot;

        // Check again before firing
        if (CanSeePlayer() && bulletPrefab != null)
        {
            Vector2 direction = (
                player.position - firePoint.position
            ).normalized;

            Bullet bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity
            );

            bullet.SetDirection(direction);

            Debug.Log("Shooter fired a bullet!");
        }

        nextAttackTime = Time.time + attackCooldown;
        isAttacking = false;
        currentState = EnemyState.Idle;
    }

    // Show the viewing direction in Scene view
    void OnDrawGizmosSelected()
    {
        Vector2 facingDirection = new Vector2(
            Mathf.Cos(facingAngle * Mathf.Deg2Rad),
            Mathf.Sin(facingAngle * Mathf.Deg2Rad)
        );

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            transform.position,
            facingDirection * detectionRange
        );
    }
}
