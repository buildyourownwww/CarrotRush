
using UnityEngine;

public class CarrotCollector : MonoBehaviour
{
    [Header("Carrot Settings")]
    [SerializeField] private int pointsPerCarrot = 1;

    [Header("UI")]
    [SerializeField] private HUD hud;

    private int carrotCount = 0;

    void Start()
    {
        // Find the existing HUD
        if (hud == null)
        {
            hud = FindFirstObjectByType<HUD>();
        }

        // Start the counter at zero
        if (hud != null)
        {
            hud.SetCarrots(carrotCount);
        }
        else
        {
            Debug.LogError("HUD not found!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only collect objects tagged Carrot
        if (!other.CompareTag("Carrot"))
            return;

        if (hud == null)
            return;

        // Add points
        carrotCount += pointsPerCarrot;

        // Update the existing counter immediately
        hud.SetCarrots(carrotCount);

        // Remove the collected carrot
        Destroy(other.gameObject);
    }
}
