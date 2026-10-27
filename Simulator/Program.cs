using System;
using System.Collections.Generic;
using System.Diagnostics;
using DingDing.Core;
using DingDing.Simulator.Bots;

namespace DingDing.Simulator
{
    // Three ways of playing, same card chooser: the weakest and the
    // strongest bracket a human player.
    public static class Program
    {
        public static void Main(string[] args)
        {
            int matchCount = 2000;
            int baseSeed = 1000;

            if (args.Length > 0) int.TryParse(args[0], out matchCount);
            if (args.Length > 1) int.TryParse(args[1], out baseSeed);

            Stopwatch stopwatch = Stopwatch.StartNew();

            List<LevelStats> bestPlay = new List<LevelStats>();
            List<LevelStats> averagePlay = new List<LevelStats>();
            List<LevelStats> weakPlay = new List<LevelStats>();

            for (int levelNumber = 1; levelNumber <= LevelTable.LevelCount; levelNumber++)
            {
                LevelInfo level = LevelTable.GetLevel(levelNumber);
                int health = level.StartingHealth;

                Console.WriteLine("Running level " + levelNumber + "...");

                bestPlay.Add(LevelRunner.RunLevel(level, health, matchCount, baseSeed, new GreedyBot(new Random(levelNumber)), new HeuristicChooser()));
                averagePlay.Add(LevelRunner.RunLevel(level, health, matchCount, baseSeed, new RandomBot(new Random(levelNumber)), new HeuristicChooser()));
                weakPlay.Add(LevelRunner.RunLevel(level, health, matchCount, baseSeed, new WeakBot(new Random(levelNumber)), new HeuristicChooser()));
            }

            Console.WriteLine();
            ReportWriter.WriteConsole(bestPlay, averagePlay, weakPlay);

            ReportWriter.WriteMarkdown(bestPlay, averagePlay, weakPlay, matchCount, baseSeed, "../Reports/difficulty-report.md");

            Console.WriteLine();
            Console.WriteLine("Done in " + stopwatch.Elapsed.TotalSeconds.ToString("0.0") + " seconds.");
            Console.WriteLine("Report written to ../Reports/difficulty-report.md");
        }
    }
}