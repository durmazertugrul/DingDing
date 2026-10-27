using System.Collections.Generic;
using DingDing.Core;

namespace DingDing.Simulator.Bots
{
    // Every action the engine would accept right now: the legal swaps plus
    // a tap on each special piece.
    public static class BotMoves
    {
        public static List<PlayerAction> ListCandidates(Match match)
        {
            List<PlayerAction> candidates = new List<PlayerAction>();

            foreach (var swap in SwapRules.ListValidSwaps(match.Board))
            {
                candidates.Add(new PlayerAction(ActionType.Swap, swap.Item1, swap.Item2));
            }

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Piece piece = match.Board[row, column];
                    if (piece == null || !piece.IsSpecial()) continue;

                    Cell cell = new Cell(row, column);
                    candidates.Add(new PlayerAction(ActionType.Tap, cell, cell));
                }
            }

            return candidates;
        }
    }
}