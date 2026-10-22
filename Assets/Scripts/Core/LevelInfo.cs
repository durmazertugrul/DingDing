namespace DingDing.Core
{
    // One level's fixed definition. Nothing here changes while a match runs.
    public class LevelInfo
    {
        public int Number { get; }
        public Difficulty Difficulty { get; }
        public int StartingHealth { get; }

        // [1], [2] and [3] are used; [0] stays empty so the round number
        // can be used as the index directly.
        public int[] MovesPerRound { get; }

        public LevelInfo(int number, Difficulty difficulty, int startingHealth, int firstRoundMoves, int secondRoundMoves, int thirdRoundMoves)
        {
            Number = number;
            Difficulty = difficulty;
            StartingHealth = startingHealth;
            MovesPerRound = new int[GameRules.RoundCount + 1];
            MovesPerRound[1] = firstRoundMoves;
            MovesPerRound[2] = secondRoundMoves;
            MovesPerRound[3] = thirdRoundMoves;
        }
        

    }
}