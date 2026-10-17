using System;
using System.Collections.Generic;

namespace DingDing.Core
{
    public static class BoardGravity
    {
        public static void ApplyGravity(Board board, List<object> events)
        {
            List<(Cell From, Cell To)> moves = new List<(Cell From, Cell To)>();

            for (int column = 0; column < Board.Columns; column++)
            {
                int writeRow = Board.Rows - 1; //start at the bottom row

                for (int readRow = Board.Rows - 1; readRow >= 0; readRow--)
                {
                    if (board[readRow, column] != null)
                    {
                        if (readRow != writeRow)
                        {
                            board[writeRow, column] = board[readRow, column];
                            board[readRow, column] = null;
                            moves.Add((new Cell(readRow, column), new Cell(writeRow, column)));
                        }
                        writeRow--;
                    }
                }
            }
            events.Add(new PiecesFell(moves));
        }

        public static void RefillEmptyCells(Board board, Random random, List<object> events) 
        {
            List<(Cell Cell, Piece Piece)> spawned = new List<(Cell Cell, Piece Piece)>();

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    if (board[row, column] == null)
                    {
                        Piece piece = new Piece(BoardInitializer.NextColor(random), PieceKind.Normal);
                        board[row, column] = piece;
                        spawned.Add((new Cell(row, column), piece));
                    }
                }
            }
            events.Add(new PiecesSpawned(spawned));
        }

        public static void DropAndRefill(Board board, Random random, List<object> events)
        {
            ApplyGravity(board, events);
            RefillEmptyCells(board, random, events);
        }

    }
}