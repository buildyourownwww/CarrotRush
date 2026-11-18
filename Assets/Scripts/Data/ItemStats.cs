
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewItemStats",
    menuName = "CarrotRush/Item Stats"
)]
public class ItemStats : ScriptableObject
{
    [Header("Collectible Settings")]
    public int points = 1;
}
