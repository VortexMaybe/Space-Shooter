using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Настройки на Света (World Progression)")]
    public int currentWorldLevel = 1; // Текущо ниво на трудност
    [SerializeField] private int bossLevelIndex = 10; // XP ниво, при което започва Бос битка
    [SerializeField] private int xpLevelsPerWorldPhase = 2; // Колко XP нива трябват за увеличаване на World Level

    [Header("Настройки за Power-Ups (Pickups)")]
    [SerializeField] public int maxPickupsPerPhase = 5;
    public int currentPickupsUsed = 0; // Брой използвани Power-Ups в текущата фаза

    private int playerXPLevelAtStartOfPhase; // XP нивото на играча, когато е започнала текущата фаза



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Запазваме обекта, за да може да съществува през всички сцени
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Инициализираме началното XP ниво
        if (ExperienceManager.instance != null)
        {
            // Уверете се, че GetCurrentLevel() съществува в ExperienceManager
            playerXPLevelAtStartOfPhase = ExperienceManager.instance.GetCurrentLevel();
        }
    }

    void Start()
    {
        // Прикачваме метода към събитието, което се извиква при вдигане на XP ниво
        ExperienceManager.OnLevelUp += CheckWorldProgress;

        // ВАЖНО: При стартиране, увери се, че Spawner-ът е актуален
        if (Spawner.instance != null)
        {
            Spawner.instance.UpdateDifficulty(currentWorldLevel);
        }
    }

    void OnDestroy()
    {
        // Отписваме събитието, за да избегнем грешки (NullReferenceException)
        ExperienceManager.OnLevelUp -= CheckWorldProgress;
    }

    private void CheckWorldProgress()
    {
        if (ExperienceManager.instance == null) return;

        int currentPlayerXPLevel = ExperienceManager.instance.GetCurrentLevel();

        // 1. Проверка за напредък в World Level
        if (currentPlayerXPLevel >= playerXPLevelAtStartOfPhase + xpLevelsPerWorldPhase)
        {
            AdvanceWorldLevel();
        }

        // 2. Проверка за Бос Битка
        if (currentPlayerXPLevel == bossLevelIndex)
        {
            StartBossBattle();
        }
    }

    private void AdvanceWorldLevel()
    {
        currentWorldLevel++;
        playerXPLevelAtStartOfPhase = ExperienceManager.instance.currentLevel;

        Debug.Log("--- НОВА ФАЗА: " + currentWorldLevel + " ---");

        if (Spawner.instance != null)
        {
            Spawner.instance.UpdateDifficulty(currentWorldLevel);
        }

        currentPickupsUsed = 0;

        if (currentWorldLevel >= 5)
        {
            maxPickupsPerPhase = 3;
        }
        else
        {
            maxPickupsPerPhase = 2;
        }
    }

    private void StartBossBattle()
    {
        Debug.Log("--- BOSS BATTLE STARTING! ---");
        // Тук трябва да добавиш логика за зареждане на сцената с боса или спаунване на Боса.
    }

    // Метод за ръчно преминаване към следващата фаза (ако е необходимо, напр. бутон)
    public void GoToNextLevel()
    {
        AdvanceWorldLevel();
    }
}