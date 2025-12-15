using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager instance;
    public delegate void LevelUpAction();
    public static event LevelUpAction OnLevelUp;

    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    public int currentLevel, totalExperience;
    int previousLevelsExperience, nextLevelsExperience;

    [Header("Audio")]
    [SerializeField] private AudioClip levelupSound;

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        UpdateLevel();
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    void CheckForLevelUp()
    {
        while (totalExperience >= nextLevelsExperience)
        {
            currentLevel++;
            UpdateLevel();

            PlayLevelUpSound();
            if (OnLevelUp != null)
            {
                OnLevelUp.Invoke();
            }
        }
    }

    void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
    }

    private void PlayLevelUpSound()
    {
        if (SoundManager.instance != null && SoundManager.instance.sfxSource != null && levelupSound != null)
        {
            SoundManager.instance.sfxSource.PlayOneShot(levelupSound);
        }
    }

    void UpdateInterface()
    {
        int start = totalExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        levelText.text = currentLevel.ToString();
        experienceText.text = start + " exp / " + end + " exp";
        experienceFill.fillAmount = (float)start / (float)end;
    }
}
