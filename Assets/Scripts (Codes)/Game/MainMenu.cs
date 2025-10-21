using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Зарежда основната сцена на играта
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        // По желание можеш да покажеш отделен панел с настройки
        Debug.Log("Settings clicked!");
        // Пример: settingsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Exit clicked!");
        Application.Quit();

        // За тестове в редактора:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
