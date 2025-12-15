using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Прогрес на Света")]
    public int currentWorldLevel = 1;
    [SerializeField] private int bossLevelIndex = 10;

    [SerializeField] private int xpLevelsPerWorldPhase = 3;

    [Header("Лимит на бонуси")]
    [SerializeField] private int maxPickupsPerPhase = 3;
    private int currentPickupsUsed = 0;

    private int playerXPLevelAtStartOfPhase;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (ExperienceManager.instance != null)
        {
            playerXPLevelAtStartOfPhase = ExperienceManager.instance.GetCurrentLevel();
        }
    }

    void Start()
    {

        ExperienceManager.OnLevelUp += CheckWorldProgress;
    }

    void OnDestroy()
    {
        ExperienceManager.OnLevelUp -= CheckWorldProgress;
    }

    private void CheckWorldProgress()
    {
        if (ExperienceManager.instance == null) return;

        int currentPlayerXPLevel = ExperienceManager.instance.GetCurrentLevel();

        if (currentPlayerXPLevel >= playerXPLevelAtStartOfPhase + xpLevelsPerWorldPhase)
        {
            AdvanceWorldLevel();
        }

        if (currentPlayerXPLevel == bossLevelIndex)
        {
            StartBossBattle();
        }
    }

    private void AdvanceWorldLevel()
    {
        currentWorldLevel++;
        playerXPLevelAtStartOfPhase = ExperienceManager.instance.currentLevel;

        Debug.Log("--- НОВО НИВО: " + currentWorldLevel + " ---");

    }

    private void StartBossBattle()
    {
        Debug.Log("--- BOSS BATTLE STARTING! ---");
       
    }

    public void GoToNextLevel()
    {
        AdvanceWorldLevel();
    }
}