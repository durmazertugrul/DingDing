using System;
using System.Collections.Generic;

namespace DingDing.Core 
{
    public static class SwapRules 
    {
        public static bool IsValidSwap(Board board, Cell firstCell, Cell secondCell) 
        {
            if (board == null) 
                return false;

            int rowDistance = Math.Abs(firstCell.Row - secondCell.Row);
            int columnDistance = Math.Abs(firstCell.Column - secondCell.Column);
            if (rowDistance + columnDistance != 1) return false;

            Piece firstPiece = board[firstCell];
            Piece secondPiece = board[secondCell];


            if (firstPiece == null || secondPiece == null) { return false; }
            if (firstPiece.IsSpecial() && secondPiece.IsSpecial()) { return false; }
            if (firstPiece.IsSpecial() || secondPiece.IsSpecial()) { return true; }
            


            return SwapCreatesMatch(board, firstCell, secondCell);
        }

        private static bool SwapCreatesMatch(Board board, Cell firstCell, Cell secondCell) 
        {
            Piece firstPiece = board[firstCell];
            Piece secondPiece = board[secondCell];

            board[firstCell] = secondPiece;
            board[secondCell] = firstPiece;

            bool result = HasRunThrough(board, firstCell) || HasRunThrough(board, secondCell);

            Piece swappedBack = board[firstCell];
            board[firstCell] = board[secondCell];
            board[secondCell] = swappedBack;

            return result;
        }

        public static bool HasRunThrough(Board board, Cell cell)
        {
            PieceColor color = ColorAt(board, cell.Row, cell.Column);

            if (color == PieceColor.None) return false;

            int horizontalCount = 1;

            int leftColumn = cell.Column - 1;
            while (leftColumn >= 0 && ColorAt(board, cell.Row, leftColumn) == color)
            {
                horizontalCount++;
                leftColumn--;
            }

            int rightColumn = cell.Column + 1;
            while (rightColumn < Board.Columns
                   && ColorAt(board, cell.Row, rightColumn) == color)
            {
                horizontalCount++;
                rightColumn++;
            }

            if (horizontalCount >= 3) return true;

            int verticalCount = 1;

            int upRow = cell.Row - 1;
            while (upRow >= 0 && ColorAt(board, upRow, cell.Column) == color)
            {
                verticalCount++;
                upRow--;
            }

            int downRow = cell.Row + 1;
            while (downRow < Board.Rows && ColorAt(board, downRow, cell.Column) == color)
            {
                verticalCount++;
                downRow++;
            }

            if (verticalCount >= 3) return true;

            return false;
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

        public static bool HasAnyValidMove(Board board)
        {
            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Piece piece = board[row, column];
                    if (piece != null && piece.IsSpecial()) { return true; }
                }
            }

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Cell cell = new Cell(row, column);

                    if (column + 1 < Board.Columns)
                    {
                        if (IsValidSwap(board, cell, new Cell(row, column+1))) { return true; }
                    }

                    if (row + 1 < Board.Rows)
                    {
                        if (IsValidSwap(board, cell, new Cell(row+1, column))) { return true; }
                    }
                }
            }
            return false;
        }

        public static List<(Cell, Cell)> ListValidSwaps(Board board) 
        {
            List<(Cell, Cell)> swaps = new List<(Cell, Cell)>();
            
            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Cell cell = new Cell(row, column);
                    
                    if (column + 1 < Board.Columns)
                    {
                        Cell rightNeighbor = new Cell(row, column + 1);
                        if (IsValidSwap(board, cell, rightNeighbor))
                            swaps.Add((cell, rightNeighbor));
                    }

                    if (row + 1 < Board.Rows)
                    {
                        Cell lowerNeighbor = new Cell(row + 1, column);
                        if (IsValidSwap(board, cell, lowerNeighbor))
                            swaps.Add((cell, lowerNeighbor));
                    }
                }
            }
            return swaps;
        }
    }
}

          