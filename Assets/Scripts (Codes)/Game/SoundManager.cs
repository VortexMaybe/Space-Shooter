using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource gameSource;

    [Header("Music Clips")]
    public AudioClip[] mainMenuMusic;

    [Header("Game Music")]
    public AudioClip[] gameMusic;

    [Header("Game Over")]
    public AudioClip gameOverMusic;

    [Header("Sound Effects")]
    public AudioClip buttonClickSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            PlayRandomMusic(mainMenuMusic);
        }
        else if (scene.name == "GameScene")
        {
            PlayRandomMusic(gameMusic);
        }
    }

    public void PlayRandomMusic(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        // Избира случайна песен
        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (musicSource.clip == clip) return; // ако вече свири същата песен, не я пуска отново

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameOverMusic()
    {
        if (gameOverMusic == null || musicSource == null)
        {
            Debug.LogWarning("❌ Не е зададен GameOverMusic или липсва MusicSource!");
            return;
        }

        musicSource.Stop();
        musicSource.clip = gameOverMusic;
        musicSource.loop = false; // по-добре да не се повтаря
        musicSource.Play();

        Debug.Log("▶️ Пускам Game Over музика: " + gameOverMusic.name);
    }


    public void PlayOneShot(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayButtonClick()
    {
        PlayOneShot(buttonClickSound);
    }
}