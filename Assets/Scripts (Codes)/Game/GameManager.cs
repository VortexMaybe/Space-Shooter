using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    int score = 0;
    bool isGameOver = false;

    public static GameManager instance;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject gameOverText;
    [SerializeField] AudioClip gameOverMusic;
    [SerializeField] GameObject gameOverPanel;

    [Header("Combo Settings")]
    [SerializeField] private float comboMultiplierBase = 1.2f;
    [SerializeField] private float rollDuration = 0.4f;
    [SerializeField] private int comboBonusPerLevel = 4;

    private int comboCount = 0;
    private Coroutine scoreRollCoroutine;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        score = 0;
        isGameOver = false;
        comboCount = 0;

        scoreText.text = score.ToString("D7");
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit") && isGameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void AddScore(int baseScore)
    {
        comboCount++;

        int bonusPoints = comboBonusPerLevel * (comboCount - 1);

        int scoreToAdd = baseScore + bonusPoints;

        int oldScore = score;
        score += scoreToAdd;

        if (scoreRollCoroutine != null)
        {
            StopCoroutine(scoreRollCoroutine);
        }
        scoreRollCoroutine = StartCoroutine(RollScoreUI(oldScore, score));
    }

    public void ResetCombo()
    {
        if (comboCount > 0)
        {
            comboCount = 0;
            Debug.Log("Combo Reset BITCH!");
        }
    }

    private IEnumerator RollScoreUI(int startValue, int endValue)
    {
        float elapsed = 0f;

        while (elapsed < rollDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rollDuration;

            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t));

            if (t < 1f)
            {
                int randomDelta = UnityEngine.Random.Range(1, 1000);
                scoreText.text = (currentValue + randomDelta).ToString("D7");
            }
            else
            {
                scoreText.text = endValue.ToString("D7");
            }

            yield return null;
        }

        scoreText.text = endValue.ToString("D7");
    }

    public void InitiateGameOver()
    {
        isGameOver = true;
        gameOverText.SetActive(true);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        if (SoundManager.instance != null && gameOverMusic != null)
        {
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.PlayGameOverMusic();
        }

        ResetCombo();
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
