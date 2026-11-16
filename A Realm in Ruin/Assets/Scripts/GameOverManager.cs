using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over Settings")]
    public Transform player;
    public float deathY = -10f;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public Button playAgainButton;

    private bool gameEnded = false;

    void Start()
    {
        Time.timeScale = 1f;

        gameOverPanel.SetActive(false);

        playAgainButton.onClick.RemoveListener(RestartGame);
        playAgainButton.onClick.AddListener(RestartGame);
    }

    void Update()
    {
        if (gameEnded || player == null)
            return;

        if (player.position.y < deathY)
        {
            PlayerDied();
        }
    }

    void PlayerDied()
    {
        gameEnded = true;
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Debug.Log("PLAY AGAIN BUTTON CLICKED!");

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
   
    public void ReturnToMainMenu()
    {
        // Allow the game to run again.
        Time.timeScale = 1f;

        // Load the main menu.
        SceneManager.LoadScene("MainMenu");
    }
}
