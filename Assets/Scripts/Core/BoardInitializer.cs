using System;

namespace DingDing.Core
{
    public class BoardInitializer
    {
        private readonly Random random;

        public BoardInitializer(Random random)
        {
            this.random = random;
        }

        public PieceColor NextColor()
        {
            return (PieceColor)random.Next(1, 6); //Colors between 1 to 5
        }

        public void FillRandom(Board board)
        {
            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    board[row, column] = new Piece(NextColor(), PieceKind.Normal);
                }
            }
        }

        public void SpawnNewPieces(Board board)
        {
            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    if (board.IsEmpty(row, column))
                    {
                        board[row, column] = new Piece(NextColor(), PieceKind.Normal);
                    }
                }
            }
        }
    }
}