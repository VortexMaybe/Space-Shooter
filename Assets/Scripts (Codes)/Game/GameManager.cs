using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int score = 0;
    bool isGameOver = false;

    public static GameManager instance;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject gameOverText;
    [SerializeField] AudioClip gameOverMusic;
    [SerializeField] GameObject gameOverPanel;

    // 🟢 ДОБАВЕНО: настройки за животи
    [Header("Player Lives")]
    [SerializeField] int maxLives = 3;       // максимален брой животи
    int currentLives;                        // текущи животи
    [SerializeField] TextMeshProUGUI livesText; // по желание — UI текст за животи

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 🟢 инициализация на животи
        currentLives = maxLives;
        UpdateLivesUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit") && isGameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // 🟢 Функция за загуба на живот
    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            InitiateGameOver();
        }
        else
        {
            Debug.Log("💔 Lost a life! Lives left: " + currentLives);
            // по желание можеш да добавиш respawn логика тук
        }
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Lives: " + currentLives;
    }

    // Update is called once per frame
    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString("D7");
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
