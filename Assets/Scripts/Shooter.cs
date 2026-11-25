
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

    [Header("Enemy Data")]
    [SerializeField] private EnemyStats stats;

    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallLayer;

    [Header("Bullet Settings")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootingSound;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private EnemyState currentState = EnemyState.Idle;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (stats == null)
        {
            Debug.LogError("ShooterStats is missing!", this);
            enabled = false;
        }
    }

    bool CanSeePlayer()
    {
        if (player == null || firePoint == null)
            return false;

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

        if (distance > stats.detectionRange)
            return false;

        Vector2 facingDirection = new Vector2(
            Mathf.Cos(stats.facingAngle * Mathf.Deg2Rad),
            Mathf.Sin(stats.facingAngle * Mathf.Deg2Rad)
        );

        float angleToPlayer = Vector2.Angle(
            facingDirection,
            direction
        );

        if (angleToPlayer > stats.fieldOfView / 2f)
            return false;

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

        spriteRenderer.color = Color.yellow;

        yield return new WaitForSeconds(stats.warningDuration);

        spriteRenderer.color = originalColor;
        currentState = EnemyState.Shoot;

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

            // Play shooting sound
            if (audioSource != null && shootingSound != null)
            {
                audioSource.PlayOneShot(shootingSound);
            }

            Debug.Log("Shooter fired a bullet!");
        }

        nextAttackTime = Time.time + stats.attackCooldown;
        isAttacking = false;
        currentState = EnemyState.Idle;
    }

    void OnDrawGizmosSelected()
    {
        if (stats == null)
            return;

        Vector2 facingDirection = new Vector2(
            Mathf.Cos(stats.facingAngle * Mathf.Deg2Rad),
            Mathf.Sin(stats.facingAngle * Mathf.Deg2Rad)
        );

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            transform.position,
            facingDirection * stats.detectionRange
        );
    }
}
    