namespace DingDing.Core
{
    public static class LevelTable
    {
        // Placeholder numbers
        // simulator report.
        private static readonly LevelInfo[] levels = new LevelInfo[]
        {
            new LevelInfo(1,  Difficulty.Normal,     60,  12, 14, 16),
            new LevelInfo(2,  Difficulty.Normal,     75,  12, 14, 16),
            new LevelInfo(3,  Difficulty.Normal,     90,  11, 13, 15),
            new LevelInfo(4,  Difficulty.Normal,    105,  11, 13, 15),
            new LevelInfo(5,  Difficulty.Hard,      130,  10, 12, 14),
            new LevelInfo(6,  Difficulty.Normal,    140,  11, 13, 15),
            new LevelInfo(7,  Difficulty.Normal,    155,  10, 12, 14),
            new LevelInfo(8,  Difficulty.Hard,      180,  10, 12, 14),
            new LevelInfo(9,  Difficulty.Normal,    190,  10, 12, 14),
            new LevelInfo(10, Difficulty.SuperHard, 220,   9, 11, 13)
        };
        public static int LevelCount{ get { return levels.Length; } }

        public static LevelInfo GetLevel(int number)
        {
            return levels[number - 1];
        }
    } 
}