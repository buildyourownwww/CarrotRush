
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string firstLevel = "Level1";

    public void Play()
    {
        StartCoroutine(PlayAndLoad());
    }

    IEnumerator PlayAndLoad()
    {
        Time.timeScale = 1f;

        UIAudioManager audioManager = FindFirstObjectByType<UIAudioManager>();

        if (audioManager != null)
        {
            audioManager.PlayClick();
        }

        yield return new WaitForSecondsRealtime(0.2f);

        SceneManager.LoadScene(firstLevel);
    }

    public void Quit()
    {
        StartCoroutine(QuitAndClose());
    }

    IEnumerator QuitAndClose()
    {
        if (UIAudioManager.Instance != null)
        {
            UIAudioManager.Instance.PlayClick();
        }

        yield return new WaitForSecondsRealtime(0.2f);

        Debug.Log("Quit pressed");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
