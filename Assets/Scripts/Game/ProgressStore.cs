using DingDing.Core;
using UnityEngine;

// The one number that goes to disk: the highest level the player opened.
public static class ProgressStore
{
    private const string HighestUnlockedKey = "highestUnlockedLevel";

    public static int HighestUnlockedLevel
    {
        get
        {
            // A tampered save would otherwise reach past the level table.
            int stored = PlayerPrefs.GetInt(HighestUnlockedKey, 1);
            return Mathf.Clamp(stored, 1, LevelTable.LevelCount);
        }
    }

    // A level opens only by knocking out the one at the front of the
    // progress; replaying an old level changes nothing.
    public static void OnMatchFinished(Match match, int levelNumber)
    {
        if (match.Outcome != MatchOutcome.Knockout) return;
        if (levelNumber != HighestUnlockedLevel) return;

        int nextLevel = Mathf.Min(levelNumber + 1, LevelTable.LevelCount);

        PlayerPrefs.SetInt(HighestUnlockedKey, nextLevel);
        PlayerPrefs.Save();

        GameSession.LevelJustUnlocked = true;
    }
}