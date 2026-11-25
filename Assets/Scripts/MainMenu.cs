
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

        // Play the UI click sound
        UIAudioManager audioManager = FindFirstObjectByType<UIAudioManager>();

        if (audioManager != null)
        {
            audioManager.PlayClick();
        }

        // Wait for the sound to start playing
        yield return new WaitForSecondsRealtime(0.2f);

        SceneManager.LoadScene(firstLevel);
    }

    public void Quit()
    {
        Debug.Log("Quit pressed");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
