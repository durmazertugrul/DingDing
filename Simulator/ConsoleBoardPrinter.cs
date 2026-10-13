using System;

namespace DingDing.Simulator
{
    public class ConsoleBoardPrinter
    {

        public void Print(grid) // Prints the 2D array (game board) to the console.
        {
            Console.WriteLine();
            for (int y = 0; y < grid.Height-1; y++)
            {
                for (int x = 0; x < grid.Width-1; x++)
                {
                    if (grid.Cells[x, y] == TileType.Empty)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write((int)Cells[x, y] + " ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

    }
}

