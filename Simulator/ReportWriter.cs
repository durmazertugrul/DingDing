using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using DingDing.Core;

namespace DingDing.Simulator
{
    // Prints the run to the console and writes the same table to a markdown
    // file the repository keeps. Three win columns bracket a human: the
    // weakest play, ordinary play and the strongest play the bots manage.
    // The bands are targets for a human player, and ordinary play is the
    // closest stand in for one, so the checks judge that column.
    public static class ReportWriter
    {
        private const string HeaderRow = "| Level | Tier | Health | Win% best | Win% random | Win% weak | Dmg/move best | Dmg/move random | Dmg/move weak | KO R1 | KO R2 | KO R3 | Rounds | Score |";
        private const string SeparatorRow = "|---|---|---|---|---|---|---|---|---|---|---|---|---|---|";
        private const double LevelOneKnockoutTarget = 0.60;

        public static void WriteConsole(List<LevelStats> bestPlay, List<LevelStats> averagePlay, List<LevelStats> weakPlay)
        {
            Console.WriteLine(HeaderRow);

            for (int index = 0; index < bestPlay.Count; index++)
            {
                Console.WriteLine(ToTableRow(bestPlay[index], averagePlay[index], weakPlay[index]));
            }

            Console.WriteLine();
            Console.WriteLine("Checks");

            foreach (var message in Checks(bestPlay, averagePlay, weakPlay))
            {
                Console.WriteLine("- " + message);
            }
        }

        public static void WriteMarkdown(List<LevelStats> bestPlay, List<LevelStats> averagePlay, List<LevelStats> weakPlay, int matchCount, int baseSeed, string reportPath)
        {
            StringBuilder reportText = new StringBuilder();

            reportText.AppendLine("# Difficulty Report");
            reportText.AppendLine();
            reportText.AppendLine("Matches per level: " + matchCount + ", base seed: " + baseSeed);
            reportText.AppendLine();
            reportText.AppendLine("Best play tries every legal move and keeps the one that takes the most health. Random play takes any legal move. Weak play takes the move that does the least damage. Random play is not a floor in a match-3 game, since any clear deals damage, so the weak column is the floor. The remaining columns come from the best play run.");
            reportText.AppendLine();
            reportText.AppendLine("Ordinary play matches the damage per move a human reaches, so the bands are checked against the random column. The weak and best columns show how wide the outcome can swing around it.");
            reportText.AppendLine();

            // The separator row has to sit right under the header, or the
            // table is not drawn at all.
            reportText.AppendLine(HeaderRow);
            reportText.AppendLine(SeparatorRow);

            for (int index = 0; index < bestPlay.Count; index++)
            {
                reportText.AppendLine(ToTableRow(bestPlay[index], averagePlay[index], weakPlay[index]));
            }

            reportText.AppendLine();
            reportText.AppendLine("## Checks");
            reportText.AppendLine();

            foreach (var message in Checks(bestPlay, averagePlay, weakPlay))
            {
                reportText.AppendLine("- " + message);
            }

            string folder = Path.GetDirectoryName(reportPath);
            if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);

            File.WriteAllText(reportPath, reportText.ToString());
        }

        private static string ToTableRow(LevelStats bestPlay, LevelStats averagePlay, LevelStats weakPlay)
        {
            return "| " + bestPlay.LevelNumber
                 + " | " + bestPlay.Difficulty
                 + " | " + bestPlay.Health
                 + " | " + Percent(bestPlay.WinRate)
                 + " | " + Percent(averagePlay.WinRate)
                 + " | " + Percent(weakPlay.WinRate)
                 + " | " + Number(bestPlay.DamagePerMove, "0.00")
                 + " | " + Number(averagePlay.DamagePerMove, "0.00")
                 + " | " + Number(weakPlay.DamagePerMove, "0.00")
                 + " | " + Percent(bestPlay.KnockoutRateInRound(1))
                 + " | " + Percent(bestPlay.KnockoutRateInRound(2))
                 + " | " + Percent(bestPlay.KnockoutRateInRound(3))
                 + " | " + Number(bestPlay.AverageRounds, "0.00")
                 + " | " + Number(bestPlay.AverageScore, "0")
                 + " |";
        }

        // A Turkish decimal comma would make these unreadable in a table, so
        // the culture is pinned.
        private static string Percent(double ratio)
        {
            return (ratio * 100).ToString("0.0", CultureInfo.InvariantCulture);
        }

        private static string Number(double value, string format)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        private static List<string> Checks(List<LevelStats> ceiling, List<LevelStats> ordinary, List<LevelStats> floor)
        {
            List<string> messages = new List<string>();

            for (int index = 0; index < ordinary.Count; index++)
            {
                LevelStats top = ceiling[index];
                LevelStats middle = ordinary[index];
                LevelStats bottom = floor[index];

                // Level one is not judged by a win band at all. It is the
                // opening match and its only requirement is that a strong
                // round one can finish it.
                if (middle.LevelNumber == 1)
                {
                    double knockoutRate = top.KnockoutRateInRound(1);

                    if (knockoutRate < LevelOneKnockoutTarget) messages.Add("Level 1 does not allow a round one knockout often enough: " + Percent(knockoutRate) + "% of best-play matches, below " + Percent(LevelOneKnockoutTarget) + "%.");
                    else messages.Add("Level 1 allows a round one knockout in " + Percent(knockoutRate) + "% of best play, as intended.");

                    continue;
                }

                double bandLow = LowEdge(middle.Difficulty);
                double bandHigh = HighEdge(middle.Difficulty);
                string band = BandOf(middle.Difficulty);
                string bracket = " Floor " + Percent(bottom.WinRate) + "%, ceiling " + Percent(top.WinRate) + "%.";

                if (middle.WinRate > bandHigh) messages.Add("Level " + middle.LevelNumber + " is too easy: ordinary play wins " + Percent(middle.WinRate) + "%, above the band (" + band + ")." + bracket);
                else if (middle.WinRate < bandLow) messages.Add("Level " + middle.LevelNumber + " is too hard: ordinary play wins only " + Percent(middle.WinRate) + "%, below the band (" + band + ")." + bracket);
                else messages.Add("Level " + middle.LevelNumber + " sits in its band (" + band + "): ordinary play wins " + Percent(middle.WinRate) + "%." + bracket);
            }

            // A knockout in round one means the corner cards never entered
            // the match at all. Level one is meant to allow it.
            foreach (var stats in ceiling)
            {
                if (stats.LevelNumber == 1) continue;

                if (stats.KnockoutRateInRound(1) > 0.05) messages.Add("Level " + stats.LevelNumber + " ends in round one in " + Percent(stats.KnockoutRateInRound(1)) + "% of best-play matches; the cards barely come into play.");
            }

            // A hard level has to leave a round two knockout open, or a strong
            // performance has no score upside left in it.
            foreach (var stats in ceiling)
            {
                if (stats.Difficulty != Difficulty.Hard) continue;

                if (stats.KnockoutRateInRound(2) < 0.20) messages.Add("Level " + stats.LevelNumber + " knocks out in round two in only " + Percent(stats.KnockoutRateInRound(2)) + "% of best-play matches; strong play has no reward over a points win.");
            }

            return messages;
        }

        private static double LowEdge(Difficulty difficulty)
        {
            if (difficulty == Difficulty.Hard) return 0.50;
            if (difficulty == Difficulty.SuperHard) return 0.35;

            return 0.70;
        }

        private static double HighEdge(Difficulty difficulty)
        {
            if (difficulty == Difficulty.Hard) return 0.65;
            if (difficulty == Difficulty.SuperHard) return 0.45;

            return 0.85;
        }

        private static string BandOf(Difficulty difficulty)
        {
            if (difficulty == Difficulty.Hard) return "50 to 65";
            if (difficulty == Difficulty.SuperHard) return "35 to 45";

            return "70 to 85";
        }
    }
}