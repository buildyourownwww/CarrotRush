
using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;

    [Header("Music Tracks")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;

    [Header("Settings")]
    public float crossfadeDuration = 2f;

    private AudioSource activeSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        activeSource = musicSourceA;
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        CrossfadeTo(menuMusic);
    }

    public void PlayLevelMusic()
    {
        CrossfadeTo(levelMusic);
    }

    public void StopMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        musicSourceA.Stop();
        musicSourceB.Stop();

        musicSourceA.volume = 1f;
        musicSourceB.volume = 1f;
    }

    private void CrossfadeTo(AudioClip newClip)
    {
        if (newClip == null ||
            (activeSource.clip == newClip && activeSource.isPlaying))
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeMusic(newClip));
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        AudioSource nextSource =
            activeSource == musicSourceA ? musicSourceB : musicSourceA;

        nextSource.clip = newClip;
        nextSource.volume = 0f;
        nextSource.Play();

        float timer = 0f;
        AudioSource previousSource = activeSource;

        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / crossfadeDuration;

            nextSource.volume = progress;
            previousSource.volume = 1f - progress;

            yield return null;
        }

        previousSource.Stop();
        previousSource.volume = 1f;
        nextSource.volume = 1f;

        activeSource = nextSource;
        fadeCoroutine = null;
    }
}
