using System;
using DingDing.Core;

namespace DingDing.Tests
{
    public static class TestBoards
    {
        public static Board FromText(params string[] rows)
        {
            if (rows.Length != Board.Rows)
            {
                throw new ArgumentException(
                    $"A board needs exactly {Board.Rows} rows, but {rows.Length} were given.");
            }

            Board board = new Board();

            for (int row = 0; row < Board.Rows; row++)
            {
                string rowText = rows[row];

                if (rowText.Length != Board.Columns)
                {
                    throw new ArgumentException(
                        $"Row {row} needs exactly {Board.Columns} letters, but it has {rowText.Length}.");
                }

                for (int column = 0; column < Board.Columns; column++)
                {
                    board[row, column] = PieceFromLetter(rowText[column]);
                }
            }

            return board;
        }

        private static Piece PieceFromLetter(char letter)
        {
            switch (letter)
            {
                case 'R': return new Piece(PieceColor.Red, PieceKind.Normal);
                case 'B': return new Piece(PieceColor.Blue, PieceKind.Normal);
                case 'O': return new Piece(PieceColor.Orange, PieceKind.Normal);
                case 'G': return new Piece(PieceColor.Green, PieceKind.Normal);
                case 'P': return new Piece(PieceColor.Purple, PieceKind.Normal);

                case 'm': return new Piece(PieceColor.None, PieceKind.Medal);
                case 't': return new Piece(PieceColor.None, PieceKind.Trophy);
                case 'b': return new Piece(PieceColor.None, PieceKind.Belt);
                case 'g': return new Piece(PieceColor.None, PieceKind.Gong);

                case '.': return null;

                default:
                    throw new ArgumentException($"Unknown board letter: '{letter}'.");
            }
        }
    }
}