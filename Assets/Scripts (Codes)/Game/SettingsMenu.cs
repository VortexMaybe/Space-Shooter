using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // Зареждаме предишните настройки
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // При промяна – актуализираме в SoundManager
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMusicVolume(float value)
    {
        if (SoundManager.instance != null && SoundManager.instance.musicSource != null)
            SoundManager.instance.musicSource.volume = value;

        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private void SetSFXVolume(float value)
    {
        if (SoundManager.instance != null && SoundManager.instance.sfxSource != null)
            SoundManager.instance.sfxSource.volume = value;

        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
