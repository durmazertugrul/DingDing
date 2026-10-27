namespace DingDing.Core
{
    public static class LevelTable
    {
        // Placeholder numbers
        // simulator report.
        private static readonly LevelInfo[] levels = new LevelInfo[]
        {
            new LevelInfo(1,  Difficulty.Normal,    120,  12, 14, 16),
            new LevelInfo(2,  Difficulty.Normal,    247,  12, 14, 16),
            new LevelInfo(3,  Difficulty.Normal,    231,  11, 13, 15),
            new LevelInfo(4,  Difficulty.Normal,    235,  11, 13, 15),
            new LevelInfo(5,  Difficulty.Hard,      237,  10, 12, 14),
            new LevelInfo(6,  Difficulty.Normal,    239,  11, 13, 15),
            new LevelInfo(7,  Difficulty.Normal,    221,  10, 12, 14),
            new LevelInfo(8,  Difficulty.Hard,      240,  10, 12, 14),
            new LevelInfo(9,  Difficulty.Normal,    224,  10, 12, 14),
            new LevelInfo(10, Difficulty.SuperHard, 244,   9, 11, 13)
        };
        public static int LevelCount{ get { return levels.Length; } }

        public static LevelInfo GetLevel(int number)
        {
            return levels[number - 1];
        }
    } 
}