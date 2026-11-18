
using UnityEngine;

public class CarrotCollector : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private ItemStats carrotData;

    [Header("UI")]
    [SerializeField] private HUD hud;

    [Header("Level System")]
    [SerializeField] private LevelFlow levelFlow;

    private int carrotCount = 0;

    void Start()
    {
        if (carrotData == null)
        {
            Debug.LogError("CarrotData is missing!", this);
            enabled = false;
            return;
        }

        if (hud == null)
            hud = FindFirstObjectByType<HUD>();

        if (levelFlow == null)
            levelFlow = FindFirstObjectByType<LevelFlow>();

        if (hud != null)
            hud.SetCarrots(carrotCount);
        else
            Debug.LogError("HUD not found!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Carrot"))
            return;

        if (hud == null || carrotData == null)
            return;

        // Add points from ItemStats
        carrotCount += carrotData.points;

        // Update HUD
        hud.SetCarrots(carrotCount);

        // Tell LevelFlow one carrot was collected
        if (levelFlow != null)
            levelFlow.RegisterCarrotCollected();

        // Remove carrot
        Destroy(other.gameObject);
    }
}
