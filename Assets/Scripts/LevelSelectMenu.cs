
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelSelectMenu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject menuPanel;

    [Header("Level Buttons")]
    [SerializeField] private Button[] levelButtons;

    [Header("Back Button")]
    [SerializeField] private Button backButton;

    public static int SelectedLevelToLoad = 0;

    private float previousTimeScale = 1f;
    private bool isLoading = false;

    void Awake()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
    }

    void Start()
    {
        // Connect level buttons automatically
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null)
                continue;

            int levelNumber = i + 1;

            levelButtons[i].onClick.AddListener(
                () => SelectLevel(levelNumber)
            );
        }

        // Connect BACK automatically
        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseMenu);
            Debug.Log("Back button connected!");
        }
        else
        {
            Debug.LogError("Back button is not assigned!");
        }
    }

    public void OpenMenu()
    {
        if (menuPanel == null)
            return;

        PlayClickSound();

        previousTimeScale = Time.timeScale;

        RefreshButtons();
        menuPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        Debug.Log("BACK BUTTON CLICKED!");

        PlayClickSound();

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        Time.timeScale = previousTimeScale;
    }

    void RefreshButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button button = levelButtons[i];

            if (button == null)
                continue;

            int levelNumber = i + 1;
            bool unlocked = IsUnlocked(levelNumber);

            button.interactable = unlocked;

            TMP_Text label =
                button.GetComponentInChildren<TMP_Text>(true);

            if (label != null)
            {
                label.text = "Level " + levelNumber +
                    (unlocked ? " - Unlocked" : " - Locked");
            }
        }
    }

    bool IsUnlocked(int levelNumber)
    {
        if (levelNumber == 1)
            return true;

        if (SaveManager.Instance == null ||
            SaveManager.Instance.Data == null)
            return false;

        SaveData data = SaveManager.Instance.Data;

        return data.unlockedLevels != null &&
               data.unlockedLevels.Contains(levelNumber);
    }

    void SelectLevel(int levelNumber)
    {
        if (isLoading || !IsUnlocked(levelNumber))
            return;

        PlayClickSound();

        isLoading = true;

        SelectedLevelToLoad = levelNumber;

        Debug.Log("Selected Level " + levelNumber);

        StartCoroutine(ReloadSelectedLevel());
    }

    IEnumerator ReloadSelectedLevel()
    {
        yield return null;

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    void PlayClickSound()
    {
        if (UIAudioManager.Instance != null)
        {
            UIAudioManager.Instance.PlayClick();
        }
    }
}
