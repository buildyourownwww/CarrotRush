using UnityEngine;

// Shows a pixel-art ! when an enemy's original SpriteRenderer turns yellow.
// Works even when the old square SpriteRenderer is disabled for visuals.
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyWarningIcon : MonoBehaviour
{
    [Header("Warning Position")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 1.1f);
    [SerializeField] private float iconScale = 1f;

    private SpriteRenderer oldRenderer;
    private GameObject warningObject;
    private Texture2D iconTexture;
    private Sprite iconSprite;

    private void Awake()
    {
        oldRenderer = GetComponent<SpriteRenderer>();

        string[] pixels =
        {
            "..###..",
            "..#Y#..",
            "..#Y#..",
            "..#Y#..",
            "..#Y#..",
            "..###..",
            ".......",
            "..###..",
            "..#Y#..",
            "..###.."
        };

        iconTexture = new Texture2D(7, pixels.Length, TextureFormat.RGBA32, false);
        iconTexture.filterMode = FilterMode.Point;
        iconTexture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < pixels.Length; y++)
        {
            string row = pixels[pixels.Length - y - 1];
            for (int x = 0; x < row.Length; x++)
            {
                Color color = row[x] == 'Y' ? new Color32(255, 220, 65, 255)
                    : row[x] == '#' ? new Color32(68, 45, 65, 255)
                    : Color.clear;
                iconTexture.SetPixel(x, y, color);
            }
        }
        iconTexture.Apply();

        iconSprite = Sprite.Create(
            iconTexture,
            new Rect(0, 0, iconTexture.width, iconTexture.height),
            new Vector2(0.5f, 0f),
            16f
        );

        warningObject = new GameObject("AttackWarning_!");
        warningObject.transform.SetParent(transform, false);
        warningObject.transform.localPosition = offset;
        warningObject.transform.localScale = Vector3.one * iconScale;

        SpriteRenderer warningRenderer = warningObject.AddComponent<SpriteRenderer>();
        warningRenderer.sprite = iconSprite;
        warningRenderer.sortingLayerName = "Default";
        warningRenderer.sortingOrder = 50;
        warningObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (oldRenderer == null || warningObject == null) return;

        Color c = oldRenderer.color;
        // Existing Patroller, Shooter, and Charger scripts use Color.yellow
        // during their warning phase.
        bool isWarning = c.r > 0.85f && c.g > 0.7f && c.b < 0.3f;
        warningObject.SetActive(isWarning);
    }

    private void OnDestroy()
    {
        if (iconSprite != null) Destroy(iconSprite);
        if (iconTexture != null) Destroy(iconTexture);
    }
}
