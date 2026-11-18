
using UnityEngine;

public enum LevelGoal
{
    CollectCarrots,
    DefeatAllEnemies,
    ReachExit
}

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public Vector2 spawnPosition;
}

[CreateAssetMenu(
    fileName = "NewLevelData",
    menuName = "CarrotRush/Level Data"
)]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    public int levelNumber;
    public string levelName;

    [Header("Level Goal")]
    public LevelGoal goal;
    public int requiredCarrots;

    [Header("Time")]
    public float timeLimit;

    [Header("Enemies")]
    public EnemySpawnData[] enemies;
}
