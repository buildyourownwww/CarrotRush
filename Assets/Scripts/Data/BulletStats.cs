
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewBulletStats",
    menuName = "CarrotRush/Bullet Stats"
)]
public class BulletStats : ScriptableObject
{
    [Header("Bullet Settings")]
    public float speed;
    public float lifetime;
    public int damage;
}
