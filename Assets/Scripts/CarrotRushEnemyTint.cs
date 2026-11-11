using UnityEngine;

// Keeps existing enemy attack telegraphs visible even when the original rectangle renderer is hidden.
public class CarrotRushEnemyTint : MonoBehaviour
{
    public SpriteRenderer source;
    public SpriteRenderer target;

    void LateUpdate()
    {
        if (source != null && target != null)
            target.color = Color.white;
    }
}
