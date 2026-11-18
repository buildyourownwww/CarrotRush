
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Data")]
    [SerializeField] private BulletStats stats;

    private Vector2 direction = Vector2.right;

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Start()
    {
        if (stats == null)
        {
            Debug.LogError(
                "BulletStats is missing!", this
            );

            Destroy(gameObject);
            return;
        }

        // Destroy bullet after its lifetime
        Destroy(gameObject, stats.lifetime);
    }

    void FixedUpdate()
    {
        if (stats == null)
            return;

        // Move bullet using data asset speed
        transform.position +=
            (Vector3)(
                direction *
                stats.speed *
                Time.fixedDeltaTime
            );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (stats == null)
            return;

        PlayerHealth playerHealth =
            other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // Damage comes from BulletStats
            playerHealth.TakeDamage(stats.damage);

            Debug.Log("Bullet hit the Player!");

            Destroy(gameObject);
        }
    }
}
