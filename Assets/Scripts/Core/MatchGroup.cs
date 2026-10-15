using System.Collections.Generic;

namespace DingDing.Core
{
    public class Run
    {
        public PieceColor Color { get; }
        public bool IsHorizontal { get; }
        public List<Cell> Cells { get; }

        public Run(PieceColor color, bool isHorizontal, List<Cell> cells)
        {
            Color = color;
            IsHorizontal = isHorizontal;
            Cells = cells;
        }

        public int Length
        {
            get { return Cells.Count; }
        }
    }

    public class MatchGroup
    {
        public PieceColor Color { get; }
        public List<Cell> Cells { get; }
        public List<Run> HorizontalRuns { get; }
        public List<Run> VerticalRuns { get; }

        public MatchGroup(PieceColor color)
        {
            Color = color;
            Cells = new List<Cell>();
            HorizontalRuns = new List<Run>();
            VerticalRuns = new List<Run>();
        }

        public void AddRun(Run run)
        {
            if (run.IsHorizontal)
            {
                HorizontalRuns.Add(run);
            }
            else
            {
                VerticalRuns.Add(run);
            }

            AddCells(run.Cells);
        }

        public void AbsorbGroup(MatchGroup other)
        {
            HorizontalRuns.AddRange(other.HorizontalRuns);
            VerticalRuns.AddRange(other.VerticalRuns);
            AddCells(other.Cells);
        }

        public bool SharesCellWith(Run run)
        {
            foreach (Cell cell in run.Cells)
            {
                if (Cells.Contains(cell))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddCells(List<Cell> cellsToAdd)
        {
            foreach (Cell cell in cellsToAdd)
            {
                if (!Cells.Contains(cell))
                {
                    Cells.Add(cell);
                }
            }
        }
    }
}