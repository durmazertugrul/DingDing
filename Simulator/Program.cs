using System;
using System.Collections.Generic;
using DingDing.Core;

namespace DingDing.Simulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int seed = 12345;
            if (args.Length == 1 && int.TryParse(args[0], out int number))
                seed = number;

            Random random = new Random(seed);
            Board board = BoardInitializer.CreateStartingBoard(random);

            Console.WriteLine("Starting board, seed " + seed + ":");
            ConsoleBoardPrinter.Print(board);

            List<(Cell, Cell)> swaps = SwapRules.ListValidSwaps(board);
            Console.WriteLine(swaps.Count + " valid swaps on this board.");

            if (swaps.Count == 0)
            {
                Console.WriteLine("No move to play.");
                return;
            }

            Cell firstCell = swaps[0].Item1;
            Cell secondCell = swaps[0].Item2;
            Console.WriteLine("Playing " + firstCell + " with " + secondCell + ".");

            PlayerAction action = new PlayerAction(ActionType.Swap, firstCell, secondCell);
            List<object> events = new List<object>();
            MoveResult result = MoveResolver.ResolveAction(board, action, random, events);

            Console.WriteLine();
            Console.WriteLine("Events:");

            foreach (object gameEvent in events)
            {
                Console.WriteLine("  " + DescribeEvent(gameEvent));
            }

            Console.WriteLine();
            Console.WriteLine("Steps: " + result.Steps.Count
                + ", cascades: " + result.CascadeCount
                + ", specials created: " + result.CreatedSpecials.Count);

            foreach ((PieceKind Kind, int GroupSize) createdSpecial in result.CreatedSpecials)
            {
                Console.WriteLine("  " + createdSpecial.Kind
                    + " from " + createdSpecial.GroupSize + " pieces");
            }

            Console.WriteLine();
            Console.WriteLine("Final board:");
            ConsoleBoardPrinter.Print(board);
        }

        private static string DescribeEvent(object gameEvent)
        {
            // Turns one event into a readable console line.
            // The simulator only prints them; Unity reads the same list to play animations.
            if (gameEvent is PiecesSwapped swapped)
                return "PiecesSwapped " + swapped.FirstCell + " " + swapped.SecondCell;

            if (gameEvent is SpecialsActivated activated)
                return "SpecialsActivated " + activated.Activations.Count + " blast(s)";

            if (gameEvent is CellsCleared cleared)
                return "CellsCleared " + cleared.Cells.Count + " cell(s)";

            if (gameEvent is SpecialCreated created)
                return "SpecialCreated " + created.Kind + " at " + created.Cell
                    + " from " + created.SourceCells.Count + " piece(s)";

            if (gameEvent is PiecesFell fell)
                return "PiecesFell " + fell.Moves.Count + " move(s)";

            if (gameEvent is PiecesSpawned spawned)
                return "PiecesSpawned " + spawned.Pieces.Count + " piece(s)";

            // An event type with no case above still shows its name.
            return gameEvent.GetType().Name;
        }
    }
}