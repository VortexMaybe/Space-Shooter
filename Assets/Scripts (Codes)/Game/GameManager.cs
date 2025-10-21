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

    private void Awake()
    {
       instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = "Score:" + score;
    }
    public void InitiateGameOver() 
    {
        isGameOver = true;
        gameOverText.SetActive(true);
    }
}
