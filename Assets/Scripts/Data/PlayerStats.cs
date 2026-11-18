
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewPlayerStats",
    menuName = "CarrotRush/Player Stats"
)]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Health")]
    public int maxHealth = 3;

    [Header("Damage Protection")]
    public float invulnerabilityDuration = 1f;
    public float damageFlashDuration = 0.2f;
}
