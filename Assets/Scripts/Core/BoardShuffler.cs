using System;
using System.Collections.Generic;

namespace DingDing.Core
{
    public static class BoardShuffler
    {
        // Called after every move; does nothing unless the board is locked.
        public static void ShuffleIfStuck(Board board, Random random, List<object> events)
        {
            if (SwapRules.HasAnyValidMove(board))
                return;

            Shuffle(board, random, events);
        }
        
        // Only the colours of the normal pieces move; specials stay where they are.
        public static void Shuffle(Board board, Random random, List<object> events)
        {
            List<Cell> normalCells = new List<Cell>();

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Piece piece = board[row, column];
                    if (piece != null && !piece.IsSpecial())
                        normalCells.Add(new Cell(row, column));
                }
            }


            List<PieceColor> colors = new List<PieceColor>();
            foreach (var cell in normalCells)
            {
                colors.Add(board[cell].Color);
            }

            for (int attempt = 1; attempt <= 100; attempt++)
            {
                ShuffleColors(colors, random);

                for (int index = 0; index < normalCells.Count; index++)
                {
                    board[normalCells[index]] = new Piece(colors[index], PieceKind.Normal);
                }

                if (MatchFinder.FindMatchGroups(board).Count == 0
                    && SwapRules.HasAnyValidMove(board))
                {
                    events.Add(new BoardShuffled(board.Copy()));
                    return;
                }
            }
            
            RefillWithoutMatches(board, normalCells, random);
            events.Add(new BoardShuffled(board.Copy()));
        }

        // Fisher-Yates: walk from the end, swap each item with itself or
        // something before it.
        private static void ShuffleColors(List<PieceColor> colors, Random random)
        {
            for (int index = colors.Count - 1; index > 0; index--)
            {
                int otherIndex = random.Next(0, index + 1);

                PieceColor held = colors[index];
                colors[index] = colors[otherIndex];
                colors[otherIndex] = held;
            }
        }


        private static void RefillWithoutMatches(Board board, List<Cell> normalCells, Random random)
        {
            foreach (var cell in normalCells)
            {
                board[cell] = null;
            }

            foreach (var cell in normalCells)
            {
                board[cell] = new Piece(BoardInitializer.NextColor(random),PieceKind.Normal);

                while (SwapRules.HasRunThrough(board, cell))
                {
                    board[cell] = new Piece(BoardInitializer.NextColor(random),PieceKind.Normal);
                }
            }

        }

    }
}