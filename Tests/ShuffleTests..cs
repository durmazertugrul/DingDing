using System;
using System.Collections.Generic;
using DingDing.Core;
using NUnit.Framework;

namespace DingDing.Tests
{
    [TestFixture]
    public class ShuffleTests
    {
        // Each row is the previous row shifted by one, so no two neighbours
        // share a colour and no swap can line up three.
        private static Board StuckBoard()
        {
            return TestBoards.FromText(
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");
        }

        // The same board with (0,1) and (1,2) turned red, which opens one swap.
        private static Board OneMoveBoard()
        {
            return TestBoards.FromText(
                "RROGPRBO",
                "BORPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");
        }

        private static int[] CountColors(Board board)
        {
            int[] counts = new int[6];

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Piece piece = board[row, column];
                    if (piece == null || piece.IsSpecial()) continue;

                    counts[(int)piece.Color] = counts[(int)piece.Color] + 1;
                }
            }

            return counts;
        }

        [Test]
        public void StuckBoardGetsShuffled()
        {
            Board board = StuckBoard();
            List<object> events = new List<object>();

            BoardShuffler.ShuffleIfStuck(board, new Random(1), events);

            Assert.That(SwapRules.HasAnyValidMove(board), Is.True,
                "the shuffled board is still stuck");
            Assert.That(MatchFinder.FindMatchGroups(board).Count, Is.EqualTo(0),
                "the shuffled board starts with a match");
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<BoardShuffled>());
        }

        [Test]
        public void SpecialsStayInPlace()
        {
            Board board = TestBoards.FromText(
                "mBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGt");

            List<object> events = new List<object>();

            // A board holding a special is never stuck, so the work is called
            // directly; the point is that shuffling leaves specials alone.
            BoardShuffler.Shuffle(board, new Random(2), events);

            Assert.That(board[0, 0].Kind, Is.EqualTo(PieceKind.Medal));
            Assert.That(board[7, 7].Kind, Is.EqualTo(PieceKind.Trophy));
        }

        [Test]
        public void BoardWithMoveUntouched()
        {
            Board board = OneMoveBoard();
            Board before = board.Copy();
            List<object> events = new List<object>();

            BoardShuffler.ShuffleIfStuck(board, new Random(3), events);

            Assert.That(events.Count, Is.EqualTo(0));

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Assert.That(board[row, column].Color,
                        Is.EqualTo(before[row, column].Color),
                        "the board changed at (" + row + ", " + column + ")");
                }
            }
        }

        [Test]
        public void ColorCountsKept()
        {
            Board board = StuckBoard();
            int[] before = CountColors(board);
            List<object> events = new List<object>();

            BoardShuffler.Shuffle(board, new Random(4), events);

            int[] after = CountColors(board);

            for (int colorIndex = 1; colorIndex < 6; colorIndex++)
            {
                Assert.That(after[colorIndex], Is.EqualTo(before[colorIndex]),
                    "colour " + (PieceColor)colorIndex + " changed count");
            }
        }
    }
}