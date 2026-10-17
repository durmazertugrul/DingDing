using System;

namespace DingDing.Core
{
    public static class BoardInitializer
    {
        public static PieceColor NextColor(Random random) 
        {
            //PieceColor.None is 0, the five colors are 1 through 5
            return (PieceColor)random.Next(1,6);
        }

        public static void SpawnNewPieces(Board board, Random random)
        {
            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                   if(board.IsEmpty(row, column)) 
                    {
                        board[row, column] = new Piece(NextColor(random), PieceKind.Normal);
                    }
                }
            }
        }

        private static bool WouldCreateMatchAt(Board board, int row, int column, PieceColor color) 
        {
            // Cells are filled top to bottom and left to right, so only the two cells
            // to the left and the two cells above hold a piece yet.

            if (column >= 2) 
            {
                Piece leftPiece = board[row, column - 1];
                Piece secondLeftPiece = board[row, column - 2];
                if (leftPiece !=null && secondLeftPiece != null && leftPiece.Color == color && secondLeftPiece.Color == color)
                    return true;
            }

            if (row >= 2)
            {
                Piece upPiece = board[row-1, column];
                Piece secondUpPiece = board[row-2, column];
                if (upPiece != null && secondUpPiece != null && upPiece.Color == color && secondUpPiece.Color == color)
                    return true;
            }

            return false;
        }

        public static Board CreateStartingBoard(Random random)
        {
            //Checks if there are any matches when the pieces are first created; if so, assigns a new color
            Board board;
            do
            {
                board = new Board();

                for (int row = 0; row <= Board.Rows-1; row++)
                {
                    for (int column = 0; column <= Board.Columns - 1; column++)
                    {
                        PieceColor candidateColor;

                        do
                        {
                            candidateColor = NextColor(random);
                        } while (WouldCreateMatchAt(board, row, column, candidateColor));

                        board[row, column] = new Piece(candidateColor, PieceKind.Normal);
                    }
                }
            } while (!SwapRules.HasAnyValidMove(board));
            return board;
        }


    }
}