
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
