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

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetButtonDown("Submit") && isGameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
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
