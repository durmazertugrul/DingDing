using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScreen : MonoBehaviour
{
    public void OnPlay()
    {
        AudioManager.Instance.Play(AudioManager.Instance.sounds.uiTap);

        GameSession.SelectedLevelNumber = ProgressStore.HighestUnlockedLevel;
        SceneManager.LoadScene(SceneNames.Game);
    }

    public void OnRankings()
    {
        AudioManager.Instance.Play(AudioManager.Instance.sounds.uiTap);
        SceneManager.LoadScene(SceneNames.Rankings);
    }

    public void OnQuit()
    {
        AudioManager.Instance.Play(AudioManager.Instance.sounds.uiBack);
        Application.Quit();
    }
}