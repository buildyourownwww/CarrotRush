
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;

    public void OpenSettings()
    {
        PlayClickSound();
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound();
        settingsPanel.SetActive(false);
    }

    void PlayClickSound()
    {
        if (UIAudioManager.Instance != null)
        {
            UIAudioManager.Instance.PlayClick();
        }
    }
}
