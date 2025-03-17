using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;  // Assign in Inspector

    [Header("Audio Sources")]
    public AudioSource musicSource; // For background music
    public AudioSource sfxSource;   // For SFX (task complete, collisions, etc.)

    [Header("Music Clips by Scene")]
    public AudioClip splashMusic;
    public AudioClip atticMusic;
    public AudioClip livingRoomMusic;
    public AudioClip basementMusic;
    public AudioClip levelCompleteMusic;


    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persists across scene loads
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }
    }

    private void OnEnable()
    {
        // Subscribe to scene load event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent errors when object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check the scene name or build index to decide which music to play
        switch (scene.name)
        {
            case "SplashScreen":
                PlayMusic(splashMusic);
                break;
            case "AtticLevel":
                PlayMusic(atticMusic);
                break;
            case "RohanLivingRoom":
                PlayLevelIntroThenMusic(levelCompleteMusic, livingRoomMusic);
                break;
            case "BasementLevel":
                PlayLevelIntroThenMusic(levelCompleteMusic, basementMusic);
                break;
            default:
                PlayMusic(null);
                break;
        }
    }

    // Simple method to set a new clip on the musicSource
    public void PlayMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip)
            return; // Avoid replaying the same track if it's already playing

        musicSource.Stop();
        musicSource.clip = newClip;

        if (newClip != null)
        {
            musicSource.Play();
        }
    }

    public void PlayLevelIntroThenMusic(AudioClip introClip, AudioClip mainLevelClip)
    {
        // Use a coroutine so we can wait for the intro to finish
        StartCoroutine(PlayIntroThenMain(introClip, mainLevelClip));
    }

    private IEnumerator PlayIntroThenMain(AudioClip introClip, AudioClip mainClip)
    {
        // 1. Stop existing music, if any
        musicSource.Stop();
        
        // 2. Play the intro track
        musicSource.loop = false;           
        musicSource.clip = introClip;
        musicSource.Play();

        // 3. Wait until the intro track finishes
        yield return new WaitForSeconds(introClip.length - 1f);
        
        // 4. Play the main level track
        musicSource.loop = true;            
        musicSource.clip = mainClip;
        musicSource.Play();
    }

    // Simple "play a one-shot SFX" method
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // If you want the music to fade out during a task SFX
    public void PlayTaskCompletionSound(AudioClip taskClip, float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutMusicAndPlayTask(taskClip, fadeDuration));
    }

    private IEnumerator FadeOutMusicAndPlayTask(AudioClip taskClip, float fadeDuration)
    {
        float originalVolume = musicSource.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(originalVolume, 0, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = 0;

        // Play task SFX
        sfxSource.PlayOneShot(taskClip);
        // Wait until done
        yield return new WaitForSeconds(taskClip.length);

        // Fade back in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, originalVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = originalVolume;
    }
}
