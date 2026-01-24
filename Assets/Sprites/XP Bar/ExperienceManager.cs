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

    [Header("Experience Settings")]
    public int currentLevel = 1;
    public int totalExperience = 0;

    [Tooltip("Колко XP ни трябва за 1-во ниво")]
    public int baseLevelXP = 30;
    [Tooltip("С колко XP повече да иска всяко следващо ниво (1.2 = 20% ; 1.3 = 30%)")]
    public float levelMultiplier = 1.2f;

    private int nextLevelsExperience;
    private int previousLevelsExperience;

    [Header("Audio")]
    [SerializeField] private AudioClip levelupSound;

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;

    void Awake()
    {
        if (instance == null) instance = this;
    }
    void Start()
    {
        UpdateLevelThresholds();
        UpdateInterface();
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }
    void CheckForLevelUp()
    {
        while (totalExperience >= nextLevelsExperience)
        {
            currentLevel++;
            UpdateLevelThresholds();

            PlayLevelUpSound();
            if (OnLevelUp != null)
            {
                OnLevelUp.Invoke();
            }
        }
    }

    void UpdateLevelThresholds()
    {
        if (currentLevel <= 1)
        {
            previousLevelsExperience = 0;
            nextLevelsExperience = baseLevelXP;
        }
        else
        {
            previousLevelsExperience = (int)(baseLevelXP * Mathf.Pow(levelMultiplier, currentLevel - 1));

            nextLevelsExperience = (int)(baseLevelXP * Mathf.Pow(levelMultiplier, currentLevel));
        }
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

        if (start < 0) start = 0;

        levelText.text = currentLevel.ToString();
        experienceText.text = start + " xp / " + end + " xp ";
        experienceFill.fillAmount = (float)start / (float)end;
    }
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}
