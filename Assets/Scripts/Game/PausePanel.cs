using UnityEngine;
using UnityEngine.SceneManagement;

// Freezes the match and shows the menu. Hiding the game also blocks input,
// since a stopped clock does not stop taps.
public class PausePanel : MonoBehaviour
{
    public GameObject root;
    public GameObject gameRoot;         // the HUD and the board together

    public bool IsPaused { get; private set; }

    public void Open()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;

        gameRoot.SetActive(false);
        root.SetActive(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        root.SetActive(false);
        gameRoot.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}