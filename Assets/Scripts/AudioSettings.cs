
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MusicParameter = "MusicSliderVolume";
    private const string SFXParameter = "SFXVolume";

    void Start()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager not found!");
            return;
        }

        // Load saved volume values
        float savedMusic = SaveManager.Instance.Data.musicVolume;
        float savedSFX = SaveManager.Instance.Data.soundVolume;

        // Set slider positions without triggering events
        musicSlider.SetValueWithoutNotify(savedMusic);
        sfxSlider.SetValueWithoutNotify(savedSFX);

        // Apply saved volumes to Audio Mixer
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    public void SetMusicVolume(float value)
    {
        float decibels = Mathf.Log10(
            Mathf.Max(value, 0.0001f)
        ) * 20f;

        audioMixer.SetFloat(MusicParameter, decibels);

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.Data.musicVolume = value;
            SaveManager.Instance.SaveNow();
        }
    }

    public void SetSFXVolume(float value)
    {
        float decibels = Mathf.Log10(
            Mathf.Max(value, 0.0001f)
        ) * 20f;

        audioMixer.SetFloat(SFXParameter, decibels);

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.Data.soundVolume = value;
            SaveManager.Instance.SaveNow();
        }
    }
}
