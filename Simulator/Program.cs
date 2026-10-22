using System;
using System.Collections.Generic;
using DingDing.Core;
using DingDing.Core.Cards;

namespace DingDing.Simulator
{
    // Plays one headless match with random legal moves and prints the result.
    public static class Program
    {
        public static void Main(string[] args)
        {
            int levelNumber = 1;
            int seed = 42;

            if (args.Length > 0) int.TryParse(args[0], out levelNumber);
            if (args.Length > 1) int.TryParse(args[1], out seed);

            LevelInfo level = LevelTable.GetLevel(levelNumber);
            Match match = MatchFlow.StartMatch(level, seed).Match;

            Random chooser = new Random(seed);

            for (int guard = 0; guard < 500; guard++)
            {
                if (match.Phase == MatchPhase.Finished) break;

                if (match.Phase == MatchPhase.Corner)
                {
                    List<CardOffer> offers = CardRules.OfferCards(match);
                    CardOffer picked = offers[chooser.Next(0, offers.Count)];

                    Console.WriteLine("Corner: " + picked.Card.Id);
                    MatchFlow.SelectCornerCard(match, picked);
                    continue;
                }

                List<(Cell, Cell)> swaps = SwapRules.ListValidSwaps(match.Board);
                if (swaps.Count == 0)
                {
                    Console.WriteLine("No legal move and no shuffle helped.");
                    break;
                }

                var swap = swaps[chooser.Next(0, swaps.Count)];
                PlayerAction action = new PlayerAction(ActionType.Swap,
                                                        swap.Item1, swap.Item2);

                if (MatchFlow.PlayAction(match, action) == null)
                    Console.WriteLine("Action refused: " + swap.Item1
                                      + " " + swap.Item2);
            }

            Console.WriteLine();
            Console.WriteLine("Level " + levelNumber + ", seed " + seed);
            Console.WriteLine("Outcome        " + match.Outcome);
            Console.WriteLine("Health left    " + match.OpponentHealth
                              + " of " + level.StartingHealth);
            Console.WriteLine("Score          " + match.Score);
            Console.WriteLine("Knockout move  " + match.KnockoutMove);

            for (int round = 1; round <= GameRules.RoundCount; round++)
            {
                Console.WriteLine("Round " + round + "        "
                                  + match.RoundResults[round]
                                  + ", damage " + match.RoundDamage[round]);
            }
        }
    }
}