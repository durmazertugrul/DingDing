using System.Collections.Generic;

namespace DingDing.Core
{
    public static class SpecialActivator
    {
        public static (List<SpecialActivation>, HashSet<Cell>) ActivateSpecials(Board board, List<Cell> startCells) 
        {
            List<SpecialActivation> activations = new List<SpecialActivation>();
            HashSet<Cell> allCleared = new HashSet<Cell>();
            Queue<Cell> pendingCells = new Queue<Cell>();

            foreach (var startCell in startCells)
            {
                pendingCells.Enqueue(startCell);
            }

            while (pendingCells.Count > 0)
            {
                Cell currentCell = pendingCells.Dequeue();

                if (allCleared.Contains(currentCell))
                {
                    continue; //one piece never blasts twice
                }

                allCleared.Add(currentCell);

                PieceKind kind = board[currentCell].Kind;
                SpecialActivation activation = new SpecialActivation(currentCell, kind);

                foreach (var cell in AffectedCellsOf(board, currentCell))
                {
                    if (allCleared.Contains(cell))
                        continue;
                    if (board[cell] == null)
                        continue;

                    activation.ClearedCells.Add(cell);

                    if (board[cell].IsSpecial())
                        pendingCells.Enqueue(cell); //the chain carries on
                   else 
                        allCleared.Add(cell);
                }

                activations.Add(activation);
            }

            return (activations, allCleared);
        }

        private static List<Cell> AffectedCellsOf(Board board, Cell cell)
        {
            List<Cell> cells = new List<Cell>();
            PieceKind kind = board[cell].Kind;

            if (kind == PieceKind.Medal) //if piece is medal add that cell a medal piece 
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    cells.Add(new Cell(cell.Row, column));
                }
            }

            else if (kind == PieceKind.Trophy)//if piece is trophy add that cell a trophy piece 
            {
                for (int row = 0; row < Board.Rows; row++)
                {
                    cells.Add(new Cell(row, cell.Column));
                }
            }

            else if (kind == PieceKind.Belt)//if piece is belt add that cell a belt piece 
            {
                for (int row = cell.Row - 1 ; row <= cell.Row + 1 ; row++)
                {
                    for (int column = cell.Column - 1; column <= cell.Column + 1; column++)
                    {
                        if (board.IsInside(row, column))//checks to make sure the belt inside the board
                            cells.Add(new Cell(row, column));
                    }
                }
            }

            else if (kind == PieceKind.Gong)//if piece is Gong add that cell a Gong piece 
            {
                PieceColor targetColor = MostCommonColor(board); //which color is the most common its the target color is

                for (int row = 0; row < Board.Rows; row++)
                {
                    for (int column = 0; column < Board.Columns; column++)
                    {
                        Piece piece = board[row, column]; 
                        if (piece != null && piece.Color == targetColor)
                            cells.Add(new Cell(row, column));
                    }
                }
            }

            return cells;
        }

        private static PieceColor MostCommonColor(Board board)
        {
            int[] counts = new int[6]; //index 0 is None and stays unused

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Piece piece = board[row, column];
                    if (piece != null && !piece.IsSpecial())
                        counts[(int)piece.Color] = counts[(int)piece.Color] + 1; //parse the color to int to count
                }
            }

            int highestCount = 0;
            for (int colorIndex = 1; colorIndex < 6; colorIndex++)
            {
                if (counts[colorIndex] > highestCount)
                    highestCount = counts[colorIndex];
            }

            if (counts[(int)PieceColor.Red] == highestCount) //Red color is superior, wins a tie
                return PieceColor.Red;

            for(int colorIndex = 1; colorIndex < 6; colorIndex++)
            {
                if (counts[colorIndex] == highestCount)
                    return (PieceColor)colorIndex; //otherwise the first color in enum order
            }

            return PieceColor.Red;
        }
        
    }
}
