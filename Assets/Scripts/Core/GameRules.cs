namespace DingDing.Core
{
    public static class GameRules
    {
        public const int RoundCount = 3;

        // Damage and score; tuned by hand against the simulator report.
        public const int DamagePerPiece = 1;
        public const int PointsPerPiece = 10;
        public const int PointsPerCreatedSpecialPiece = 25;
        public const int PointsPerLeftoverMove = 50;
        public const int SpecialBonusDamage = 5;      // per special piece blasted
        public const int PointsPerActivation = 50;
        public const int EarlyKnockoutBonusPerRound = 500;
    }
}