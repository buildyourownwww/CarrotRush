
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LevelFlow : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private LevelData[] levels;

    [Header("Starting Level")]
    [SerializeField] private int startingLevel = 1;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Level UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text resultText;

    private LevelData currentLevel;

    private List<GameObject> spawnedEnemies =
        new List<GameObject>();

    private float timeRemaining;
    private int carrotsCollected;
    private bool levelFinished;

    
    void Start()
    {
        // Read the level selected in the menu
        int selectedLevel = LevelSelectMenu.SelectedLevelToLoad;

        Debug.Log("LevelFlow received level: " + selectedLevel);

        // Reset the selection for future scene loads
        LevelSelectMenu.SelectedLevelToLoad = 0;

        // If no level was selected, use the default
        if (selectedLevel <= 0)
        {
            selectedLevel = startingLevel;
        }

        LoadLevel(selectedLevel);
    }


    void Update()
    {
        if (currentLevel == null || levelFinished)
            return;

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(0f, timeRemaining);

        UpdateTimerUI();

        if (timeRemaining <= 0f)
        {
            FinishLevel(false);
        }
    }

    public void LoadLevel(int levelNumber)
    {
        if (levels == null ||
            levelNumber < 1 ||
            levelNumber > levels.Length)
        {
            Debug.LogError("Level not found!");
            return;
        }

        LevelData selectedLevel = levels[levelNumber - 1];

        if (selectedLevel == null)
        {
            Debug.LogError("LevelData is missing!");
            return;
        }

        // Remove previous enemies
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        spawnedEnemies.Clear();

        currentLevel = selectedLevel;

        // Reset level progress
        timeRemaining = currentLevel.timeLimit;
        carrotsCollected = 0;
        levelFinished = false;
        Time.timeScale = 1f;

        if (resultText != null)
            resultText.text = "";

        UpdateTimerUI();

        Debug.Log(
            "Loaded level: " + currentLevel.levelName
        );

        // Spawn enemies from LevelData
        if (currentLevel.enemies == null)
            return;

        foreach (EnemySpawnData enemyData in currentLevel.enemies)
        {
            if (enemyData == null ||
                enemyData.enemyPrefab == null)
                continue;

            GameObject enemy = Instantiate(
                enemyData.enemyPrefab,
                enemyData.spawnPosition,
                Quaternion.identity
            );

            spawnedEnemies.Add(enemy);

            Charger charger =
                enemy.GetComponent<Charger>();

            if (charger != null)
                charger.SetPlayer(player);

            Patroller patroller =
                enemy.GetComponent<Patroller>();

            if (patroller != null)
                patroller.SetPlayer(player);

            Shooter shooter =
                enemy.GetComponent<Shooter>();

            if (shooter != null)
                shooter.SetPlayer(player);
        }
    }

    public void RegisterCarrotCollected()
    {
        if (levelFinished || currentLevel == null)
            return;

        carrotsCollected++;

        Debug.Log(
            "Carrots: " + carrotsCollected +
            "/" + currentLevel.requiredCarrots
        );

        if (currentLevel.goal == LevelGoal.CollectCarrots &&
            carrotsCollected >= currentLevel.requiredCarrots)
        {
            FinishLevel(true);
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text =
                "Time: " + Mathf.CeilToInt(timeRemaining);
        }
    }

    void FinishLevel(bool won)
    {
        if (levelFinished)
            return;

        levelFinished = true;

        if (won)
        {
            Debug.Log("LEVEL COMPLETE!");

            if (resultText != null)
                resultText.text = "LEVEL COMPLETE!";

            // Save completed level progress
            SaveLevelProgress();
        }
        else
        {
            Debug.Log("TIME'S UP!");

            if (resultText != null)
                resultText.text = "TIME'S UP!";
        }

        // Pause gameplay
        Time.timeScale = 0f;
    }

    void SaveLevelProgress()
    {
        if (SaveManager.Instance == null ||
            SaveManager.Instance.Data == null)
        {
            Debug.LogWarning(
                "SaveManager not found! Progress not saved."
            );
            return;
        }

        SaveData data = SaveManager.Instance.Data;

        int completedLevel = currentLevel.levelNumber;

        // Update highest completed level
        data.highestCompletedLevel = Mathf.Max(
            data.highestCompletedLevel,
            completedLevel
        );

        // Update best score
        data.bestScore = Mathf.Max(
            data.bestScore,
            carrotsCollected
        );

        // Unlock the next level if it exists
        int nextLevel = completedLevel + 1;

        if (nextLevel <= levels.Length &&
            !data.unlockedLevels.Contains(nextLevel))
        {
            data.unlockedLevels.Add(nextLevel);
            data.unlockedLevels.Sort();

            Debug.Log(
                "Level " + nextLevel + " unlocked!"
            );
        }

        // Write updated data to save.json
        SaveManager.Instance.SaveNow();

        Debug.Log("Level progress saved!");
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
