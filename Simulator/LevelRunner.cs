using System.Collections.Generic;
using DingDing.Core;
using DingDing.Core.Cards;
using DingDing.Simulator.Bots;

namespace DingDing.Simulator
{
    // Plays one level many times and counts what happened.
    public static class LevelRunner
    {
        // The same level with a different health pool; LevelInfo cannot be
        // changed once built.
        private static LevelInfo WithHealth(LevelInfo level, int health)
        {
            return new LevelInfo(level.Number, level.Difficulty, health, level.MovesPerRound[1], level.MovesPerRound[2], level.MovesPerRound[3]);
        }

        public static LevelStats RunLevel(LevelInfo level, int health, int matchCount, int baseSeed, IActionBot actionBot, ICardChooser cardChooser)
        {
            LevelInfo testLevel = WithHealth(level, health);

            LevelStats stats = new LevelStats();
            stats.LevelNumber = level.Number;
            stats.Difficulty = level.Difficulty;
            stats.Health = health;

            for (int matchIndex = 0; matchIndex < matchCount; matchIndex++)
            {
                // The seed comes from the index, so the same command always
                // produces the same report.
                Match match = MatchFlow.StartMatch(testLevel, baseSeed + matchIndex).Match;

                int movesPlayed = 0;
                int shuffles = 0;

                for (int guard = 0; guard < 1000; guard++)
                {
                    if (match.Phase == MatchPhase.Finished) break;

                    if (match.Phase == MatchPhase.Corner)
                    {
                        List<CardOffer> offers = CardRules.OfferCards(match);
                        MatchFlow.SelectCornerCard(match, cardChooser.Choose(match, offers));
                        continue;
                    }

                    PlayerAction action = actionBot.ChooseAction(match);
                    if (action == null) break;

                    List<object> events = MatchFlow.PlayAction(match, action);
                    if (events == null) break;

                    movesPlayed++;

                    foreach (var gameEvent in events)
                    {
                        if (gameEvent is BoardShuffled) shuffles++;
                    }
                }

                stats.Add(match, movesPlayed, shuffles);
            }

            return stats;
        }
    }
}