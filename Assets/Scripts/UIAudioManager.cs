
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    void Awake()
    {
        // Prevent duplicate managers
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this object when changing scenes
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClick()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogError("AudioSource or ClickSound is missing!");
        }
    }
}
