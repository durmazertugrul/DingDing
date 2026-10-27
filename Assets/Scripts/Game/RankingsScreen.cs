using DingDing.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// The level select and the progress display at once: ten opponents, and
// the player's own place among them.
public class RankingsScreen : MonoBehaviour
{
    public RectTransform rowsParent;        // Level_Rows
    public LevelRow rowPrefab;
    public LevelRow titleFightRow;          // level 10, fixed in the scene
    public TMP_Text rankLabel;

    private void Start()
    {
        Build(ProgressStore.HighestUnlockedLevel);
    }

    public void Build(int highestUnlockedLevel)
    {
        int levelCount = LevelTable.LevelCount;

        // Beating level 10 puts the player at number one.
        rankLabel.text = "YOUR RANK #" + (levelCount + 1 - highestUnlockedLevel);

        titleFightRow.Bind(this, levelCount, LevelTable.GetLevel(levelCount).Difficulty, StateOf(levelCount, highestUnlockedLevel));

        for (int index = rowsParent.childCount - 1; index >= 0; index--)
        {
            Destroy(rowsParent.GetChild(index).gameObject);
        }

        // Top to bottom, the strongest opponent first.
        for (int levelNumber = levelCount - 1; levelNumber >= 1; levelNumber--)
        {
            LevelRow row = Instantiate(rowPrefab, rowsParent, false);

            row.Bind(this, levelNumber, LevelTable.GetLevel(levelNumber).Difficulty, StateOf(levelNumber, highestUnlockedLevel));
        }

        // The match that opened a level has just handed the player here.
        if (GameSession.LevelJustUnlocked)
        {
            GameSession.LevelJustUnlocked = false;
            AudioManager.Instance.Play(AudioManager.Instance.sounds.levelUnlock);
        }
    }

    private static RowState StateOf(int levelNumber, int highestUnlockedLevel)
    {
        if (levelNumber < highestUnlockedLevel) return RowState.Completed;
        if (levelNumber == highestUnlockedLevel) return RowState.Current;

        return RowState.Locked;
    }

    public void StartLevel(int levelNumber)
    {
        AudioManager.Instance.Play(AudioManager.Instance.sounds.uiTap);

        GameSession.SelectedLevelNumber = levelNumber;
        SceneManager.LoadScene(SceneNames.Game);
    }

    // Wired to Btn_Back.
    public void OnBack()
    {
        AudioManager.Instance.Play(AudioManager.Instance.sounds.uiBack);
        SceneManager.LoadScene(SceneNames.MainMenu);
    }
}