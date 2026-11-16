using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Game Settings")]
    public string gameSceneName = "SampleScene";

    public void PlayGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");

        Application.Quit();
    }
}
