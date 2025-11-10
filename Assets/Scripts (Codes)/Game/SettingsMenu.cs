using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    private void Start()
    {

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        UpdateVolumeText(musicVolumeText, musicSlider.value);
        UpdateVolumeText(sfxVolumeText, sfxSlider.value);
    }

    private void SetMusicVolume(float value)
    {
        if (SoundManager.instance != null && SoundManager.instance.musicSource != null)
            SoundManager.instance.musicSource.volume = value;

        PlayerPrefs.SetFloat("MusicVolume", value);

        UpdateVolumeText(musicVolumeText, value);
    }

    private void SetSFXVolume(float value)
    {
        if (SoundManager.instance != null && SoundManager.instance.sfxSource != null)
            SoundManager.instance.sfxSource.volume = value;

        PlayerPrefs.SetFloat("SFXVolume", value);

        UpdateVolumeText(sfxVolumeText, value);
    }

    private void UpdateVolumeText(TextMeshProUGUI tmpText, float volume)
    {
        if (tmpText != null)
        {
            int percent = Mathf.RoundToInt(volume * 100f);

            tmpText.text = percent.ToString() + "%";
        }
    }
}
