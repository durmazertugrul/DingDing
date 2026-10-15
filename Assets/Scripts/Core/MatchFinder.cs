using System.Collections.Generic;

namespace DingDing.Core
{
    public static class MatchFinder
    {
        public static List<MatchGroup> FindMatchGroups(Board board)
        {
            return MergeRuns(FindRuns(board));
        }

        // A run is three or more same-color pieces side by side in a row or a column.
        // Special pieces carry no color, so they never join a run.
        private static List<Run> FindRuns(Board board)
        {
            List<Run> runs = new List<Run>();

            for (int row = 0; row < Board.Rows; row++)
            {
                int runStart = 0;

                // One step past the last column, so a run ending at the edge is closed too.
                for (int column = 1; column <= Board.Columns; column++)
                {
                    if (ContinuesHorizontally(board, row, column))
                    {
                        continue;
                    }

                    PieceColor runColor = ColorAt(board, row, runStart);
                    int runLength = column - runStart;

                    if (runLength >= 3 && runColor != PieceColor.None)
                    {
                        List<Cell> cells = new List<Cell>();

                        for (int runColumn = runStart; runColumn < column; runColumn++)
                        {
                            cells.Add(new Cell(row, runColumn));
                        }

                        runs.Add(new Run(runColor, true, cells));
                    }

                    runStart = column;
                }
            }

            for (int column = 0; column < Board.Columns; column++)
            {
                int runStart = 0;

                for (int row = 1; row <= Board.Rows; row++)
                {
                    if (ContinuesVertically(board, row, column))
                    {
                        continue;
                    }

                    PieceColor runColor = ColorAt(board, runStart, column);
                    int runLength = row - runStart;

                    if (runLength >= 3 && runColor != PieceColor.None)
                    {
                        List<Cell> cells = new List<Cell>();

                        for (int runRow = runStart; runRow < row; runRow++)
                        {
                            cells.Add(new Cell(runRow, column));
                        }

                        runs.Add(new Run(runColor, false, cells));
                    }

                    runStart = row;
                }
            }

            return runs;
        }

        private static bool ContinuesHorizontally(Board board, int row, int column)
        {
            if (column >= Board.Columns)
            {
                return false;
            }

            PieceColor color = ColorAt(board, row, column);
            return color != PieceColor.None && color == ColorAt(board, row, column - 1);
        }

        private static bool ContinuesVertically(Board board, int row, int column)
        {
            if (row >= Board.Rows)
            {
                return false;
            }

            PieceColor color = ColorAt(board, row, column);
            return color != PieceColor.None && color == ColorAt(board, row - 1, column);
        }

        // Two runs belong to the same group when they share a color and a cell.
        private static List<MatchGroup> MergeRuns(List<Run> runs)
        {
            List<MatchGroup> groups = new List<MatchGroup>();

            foreach (Run run in runs)
            {
                List<MatchGroup> touchedGroups = new List<MatchGroup>();

                foreach (MatchGroup group in groups)
                {
                    if (group.Color == run.Color && group.SharesCellWith(run))
                    {
                        touchedGroups.Add(group);
                    }
                }

                if (touchedGroups.Count == 0)
                {
                    MatchGroup newGroup = new MatchGroup(run.Color);
                    newGroup.AddRun(run);
                    groups.Add(newGroup);
                    continue;
                }

                // One run can bridge groups that were separate until now.
                MatchGroup target = touchedGroups[0];

                for (int index = 1; index < touchedGroups.Count; index++)
                {
                    target.AbsorbGroup(touchedGroups[index]);
                    groups.Remove(touchedGroups[index]);
                }

                target.AddRun(run);
            }

            return groups;
        }

        public static PieceKind ClassifyShape(MatchGroup group)
        {
            Run longestRun = LongestRunOf(group);

            if (longestRun.Length >= 5)
            {
                return PieceKind.Gong;
            }

            if (group.HorizontalRuns.Count > 1 || group.VerticalRuns.Count > 1)
            {
                // H and similar shapes make no special piece
                return PieceKind.Normal;
            }

            if (group.HorizontalRuns.Count == 1 && group.VerticalRuns.Count == 1)
            {
                // L, T and plus shapes
                return PieceKind.Belt;
            }

            if (longestRun.Length == 4 && longestRun.IsHorizontal)
            {
                return PieceKind.Medal;
            }

            if (longestRun.Length == 4 && !longestRun.IsHorizontal)
            {
                return PieceKind.Trophy;
            }

            return PieceKind.Normal;
        }

        private static Run LongestRunOf(MatchGroup group)
        {
            Run longestRun = null;

            foreach (Run run in group.HorizontalRuns)
            {
                if (longestRun == null || run.Length > longestRun.Length)
                {
                    longestRun = run;
                }
            }

            foreach (Run run in group.VerticalRuns)
            {
                if (longestRun == null || run.Length > longestRun.Length)
                {
                    longestRun = run;
                }
            }

            return longestRun;
        }

        private static PieceColor ColorAt(Board board, int row, int column)
        {
            Piece piece = board[row, column];

            if (piece == null)
            {
                return PieceColor.None;
            }

            return piece.Color;
        }
    }
}