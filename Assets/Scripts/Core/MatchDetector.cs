using System;
using System.Collections.Generic;

namespace DingDing.Core
{
    internal class MatchDetector
    {
        public List<(int x, int y)> FindMatches(Grid grid)
        {
            var matches = new HashSet<(int x, int y)>();

            // Checks for horizontal matches.
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width - 2; x++)
                {
                    var current = grid.Cells[x, y];
                    if (current == TileType.Empty) continue;

                    if (grid.Cells[x + 1, y] == current &&
                        grid.Cells[x + 2, y] == current)
                    {
                        matches.Add((x, y));
                        matches.Add((x + 1, y));
                        matches.Add((x + 2, y));
                    }
                }
            }

            // Checks for vertical matches.
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height - 2; y++)
                {
                    var current = grid.Cells[x, y];
                    if (current == TileType.Empty) continue;

                    if (grid.Cells[x, y + 1] == current &&
                        grid.Cells[x, y + 2] == current)
                    {
                        matches.Add((x, y));
                        matches.Add((x, y + 1));
                        matches.Add((x, y + 2));
                    }
                }
            }

            return new List<(int x, int y)>(matches);
        }

        public void ClearMatches(Grid grid, List<(int x, int y)> matches) //clear matches
        {
            foreach (var (x, y) in matches)
            {
                grid.Cells[x, y] = TileType.Empty;
            }
        }

    }
}
