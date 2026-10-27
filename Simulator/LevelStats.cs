using DingDing.Core;

namespace DingDing.Simulator
{
    // What one level's run of matches came out to. Counters while it runs,
    // ratios when it is read.
    public class LevelStats
    {
        public int LevelNumber;
        public Difficulty Difficulty;
        public int Health;

        public int Matches;
        public int Knockouts;
        public int[] KnockoutsByRound = new int[GameRules.RoundCount + 1];
        public int Losses;

        public int TotalRounds;
        public int TotalMoves;
        public int TotalDamage;
        public int TotalScore;
        public int TotalShuffles;

        public void Add(Match match, int movesPlayed, int shuffles)
        {
            Matches++;

            if (match.Outcome == MatchOutcome.Knockout)
            {
                Knockouts++;
                KnockoutsByRound[match.CurrentRound]++;
            }
            else
            {
                Losses++;
            }

            TotalRounds += match.CurrentRound;
            TotalMoves += movesPlayed;
            TotalScore += match.Score;
            TotalShuffles += shuffles;

            for (int round = 1; round <= GameRules.RoundCount; round++)
            {
                TotalDamage += match.RoundDamage[round];
            }
        }

        public double WinRate { get { return Matches == 0 ? 0 : (double)Knockouts / Matches; } }

        // How many tries a player needs on average to get past this level.
        public double Attempts { get { return WinRate <= 0 ? 0 : 1 / WinRate; } }

        public double LossRate { get { return Matches == 0 ? 0 : (double)Losses / Matches; } }

        public double KnockoutRateInRound(int round) { return Matches == 0 ? 0 : (double)KnockoutsByRound[round] / Matches; }

        public double AverageRounds { get { return Matches == 0 ? 0 : (double)TotalRounds / Matches; } }

        public double AverageScore { get { return Matches == 0 ? 0 : (double)TotalScore / Matches; } }

        public double DamagePerMove { get { return TotalMoves == 0 ? 0 : (double)TotalDamage / TotalMoves; } }

        public double ShufflesPerMatch { get { return Matches == 0 ? 0 : (double)TotalShuffles / Matches; } }
    }
}