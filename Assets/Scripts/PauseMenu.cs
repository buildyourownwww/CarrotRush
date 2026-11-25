
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause UI")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject controls;

    [Header("Scenes")]
    [SerializeField] string menuScene = "MainMenu";

    [Header("Audio")]
    [SerializeField] AudioMixerSnapshot normalSnapshot;
    [SerializeField] AudioMixerSnapshot pausedSnapshot;
    [SerializeField] float transitionTime = 0.2f;

    public static bool IsPaused { get; private set; }

    void Start()
    {
        SetPaused(false);
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetPaused(!IsPaused);
        }
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;

        if (pausePanel != null)
            pausePanel.SetActive(paused);

        if (controls != null)
            controls.SetActive(!paused);

        Time.timeScale = paused ? 0f : 1f;

        if (paused)
        {
            if (pausedSnapshot == null)
                Debug.LogError("PAUSED SNAPSHOT IS MISSING on " + gameObject.name);
            else
                pausedSnapshot.TransitionTo(transitionTime);
        }
        else
        {
            if (normalSnapshot == null)
                Debug.LogError("NORMAL SNAPSHOT IS MISSING on " + gameObject.name);
            else
                normalSnapshot.TransitionTo(transitionTime);
        }
    }

    public void Pause()
    {
        SetPaused(true);
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(menuScene);
    }

    void OnApplicationPause(bool appPaused)
    {
        if (appPaused)
            SetPaused(true);
    }
}
