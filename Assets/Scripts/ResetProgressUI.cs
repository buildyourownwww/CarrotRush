
using UnityEngine;
using UnityEngine.UI;

public class ResetProgressUI : MonoBehaviour
{
    [Header("Confirmation Panel")]
    [SerializeField] private GameObject confirmPanel;

    [Header("Buttons")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Level Menu")]
    [SerializeField] private LevelSelectMenu levelSelectMenu;

    void Awake()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
    }

    void Start()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(OpenConfirmation);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmReset);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelReset);
    }

    public void OpenConfirmation()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(true);
    }

    public void CancelReset()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
    }

    public void ConfirmReset()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager not found!");
            return;
        }

        // Reset the saved progress
        SaveManager.Instance.ResetProgress();

        LevelSelectMenu.SelectedLevelToLoad = 0;

        // Hide confirmation window
        if (confirmPanel != null)
            confirmPanel.SetActive(false);

        // Refresh the locked/unlocked buttons
        if (levelSelectMenu != null)
        {
            levelSelectMenu.CloseMenu();
            levelSelectMenu.OpenMenu();
        }

        Debug.Log("Progress reset confirmed!");
    }
}
