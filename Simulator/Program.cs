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
            Random random = new Random(seed);

            Board board = new Board();
            BoardInitializer initializer = new BoardInitializer(random);

            initializer.FillRandom(board);

            Console.WriteLine("Initial board, seed " + seed + ":");
            ConsoleBoardPrinter.Print(board);

            bool matchFound;

            do
            {
                List<MatchGroup> groups = MatchFinder.FindMatchGroups(board);
                matchFound = groups.Count > 0;

                if (matchFound)
                {
                    Console.WriteLine("Found " + groups.Count + " group(s). Clearing...");

                    foreach (MatchGroup group in groups)
                    {
                        Console.WriteLine("  " + group.Color
                            + ", " + group.Cells.Count + " cells"
                            + ", shape " + MatchFinder.ClassifyShape(group));

                        foreach (Cell cell in group.Cells)
                        {
                            board[cell] = null;
                        }
                    }

                    ConsoleBoardPrinter.Print(board);

                    Console.WriteLine("Applying gravity...");
                    board.ApplyGravity();
                    ConsoleBoardPrinter.Print(board);

                    Console.WriteLine("Spawning new pieces...");
                    initializer.SpawnNewPieces(board);
                    ConsoleBoardPrinter.Print(board);
                }
            }
            while (matchFound);

            Console.WriteLine("Final stable board:");
            ConsoleBoardPrinter.Print(board);
        }
    }
}