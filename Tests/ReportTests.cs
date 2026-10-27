using System;
using System.Collections.Generic;
using System.IO;
using DingDing.Core;
using DingDing.Simulator;
using DingDing.Simulator.Bots;
using NUnit.Framework;

namespace DingDing.Tests
{
    public class ReportTests
    {
        private static LevelStats Run(int health, int matchCount)
        {
            LevelInfo level = LevelTable.GetLevel(1);

            return LevelRunner.RunLevel(level, health, matchCount, 500, new GreedyBot(new Random(4)), new HeuristicChooser());
        }

        [Test]
        public void SameCommandSameReport()
        {
            LevelStats firstRun = Run(120, 20);
            LevelStats secondRun = Run(120, 20);

            Assert.That(secondRun.Knockouts, Is.EqualTo(firstRun.Knockouts));
            Assert.That(secondRun.TotalScore, Is.EqualTo(firstRun.TotalScore));
            Assert.That(secondRun.TotalMoves, Is.EqualTo(firstRun.TotalMoves));
        }

        [Test]
        public void MoreHealthFewerWins()
        {
            // The only thing that changed is the health pool, so the win
            // rate cannot climb.
            LevelStats lowHealth = Run(100, 30);
            LevelStats highHealth = Run(200, 30);

            Assert.That(highHealth.WinRate, Is.LessThanOrEqualTo(lowHealth.WinRate));
        }

        [Test]
        public void ReportFileWritten()
        {
            List<LevelStats> heuristicStats = new List<LevelStats>();
            List<LevelStats> randomStats = new List<LevelStats>();

            for (int levelNumber = 1; levelNumber <= 2; levelNumber++)
            {
                LevelStats stats = new LevelStats();
                stats.LevelNumber = levelNumber;
                stats.Difficulty = Difficulty.Normal;
                stats.Health = 100;
                stats.Matches = 1;
                stats.Knockouts = 1;
                stats.KnockoutsByRound[3] = 1;

                heuristicStats.Add(stats);
                randomStats.Add(stats);
            }

            string reportPath = Path.Combine(Path.GetTempPath(), "dingding-report-test", "difficulty-report.md");

            ReportWriter.WriteMarkdown(heuristicStats, randomStats, randomStats, 1, 500, reportPath);

            Assert.That(File.Exists(reportPath), Is.True);

            string[] lines = File.ReadAllLines(reportPath);
            int tableRows = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("| 1 |") || line.StartsWith("| 2 |")) tableRows++;
            }

            Assert.That(tableRows, Is.EqualTo(2));
            Assert.That(Array.Exists(lines, line => line.StartsWith("|---|")), Is.True);
        }
    }
}