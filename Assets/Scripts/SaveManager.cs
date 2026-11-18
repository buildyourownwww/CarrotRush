
using UnityEngine;
using System;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public SaveData Data { get; private set; }

    private string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save found. Using defaults.");
            Data = new SaveData();
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            SaveData loaded =
                JsonUtility.FromJson<SaveData>(json);

            if (loaded == null ||
                !json.Contains("\"version\"") ||
                loaded.version != 1 ||
                loaded.unlockedLevels == null ||
                !loaded.unlockedLevels.Contains(1) ||
                loaded.bestScore < 0 ||
                loaded.highestCompletedLevel < 0 ||
                float.IsNaN(loaded.musicVolume) ||
                float.IsNaN(loaded.soundVolume) ||
                loaded.musicVolume < 0f ||
                loaded.musicVolume > 1f ||
                loaded.soundVolume < 0f ||
                loaded.soundVolume > 1f)
            {
                throw new Exception("Invalid save data");
            }

            Data = loaded;
            Debug.Log("Save loaded successfully!");
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                "Save file is invalid. Using defaults. " +
                e.Message
            );

            Data = new SaveData();
        }
    }

    [ContextMenu("Save JSON Now")]
    public void SaveNow()
    {
        if (Data == null)
            Data = new SaveData();

        try
        {
            string json = JsonUtility.ToJson(Data, true);

            Directory.CreateDirectory(
                Application.persistentDataPath
            );

            File.WriteAllText(SavePath, json);

            Debug.Log("Game saved to: " + SavePath);
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Could not save game: " + e.Message
            );
        }
    }

    // NEW: Reset all saved progress
    public void ResetProgress()
    {
        Data = new SaveData();

        SaveNow();

        Debug.Log("Progress reset! Only Level 1 unlocked.");
    }

    void OnApplicationPause(bool paused)
    {
        if (paused && Instance == this)
        {
            SaveNow();
        }
    }

    void OnApplicationQuit()
    {
        if (Instance == this)
        {
            SaveNow();
        }
    }
}
