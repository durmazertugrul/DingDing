using System;
using DingDing.Core;

namespace DingDing.Simulator
{
    public static class ConsoleBoardPrinter
    {
        public static void Print(Board board)
        {
            Console.WriteLine();

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Console.Write(LetterOf(board[row, column]) + " ");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static char LetterOf(Piece piece)
        {
            if (piece == null)
            {
                return '.';
            }

            switch (piece.Kind)
            {
                case PieceKind.Medal: return 'm';
                case PieceKind.Trophy: return 't';
                case PieceKind.Belt: return 'b';
                case PieceKind.Gong: return 'g';
            }

            switch (piece.Color)
            {
                case PieceColor.Red: return 'R';
                case PieceColor.Blue: return 'B';
                case PieceColor.Orange: return 'O';
                case PieceColor.Green: return 'G';
                case PieceColor.Purple: return 'P';
            }

            return '?';
        }
    }
}