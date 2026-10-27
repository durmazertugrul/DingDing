// The only things that survive a scene change.
public static class GameSession
{
    public static int SelectedLevelNumber = 1;

    // Set when a match opens a new level, read once by the rankings screen.
    public static bool LevelJustUnlocked;
}