using System;
using System.Collections.Generic;

namespace DingDing.Core
{
    internal class BoardInitializer
    {
        private Random random = new Random();

        public void FillRandom(Grid grid) // Fills the board randomly at the start of the game.
        {
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    grid.Cells[x, y] = (TileType)random.Next(0, 5);
                }
            }
        
        }

        public void SpawnNewTiles(Grid grid) // Fills the empty spaces left after matches are cleared.
        {
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.Cells[x, y] == TileType.Empty)
                    {
                        grid.Cells[x, y] = (TileType)random.Next(0, 5);
                    }
                }
            }
        }

    }
}
