using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool isPaused = false;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }

    }

    
    public static void CursorVisible(bool state)
    {
        Cursor.visible = state;

        if (state)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    
    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(sceneName);
    }

  
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

   
    public void GamePause(bool pause)
    {
        isPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }

    
    public void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false;

        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
}
