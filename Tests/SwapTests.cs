using DingDing.Core;
using NUnit.Framework;

namespace DingDing.Tests
{
    [TestFixture]
    public class SwapTests
    {
        // Each row is the previous row shifted by one, so no two neighbors
        // share a color and no swap can line up three.
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

        // The same board with (0,1) and (1,2) turned red.
        // Swapping (1,2) up into (0,2) lines up three reds on the top row.
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

        [Test]
        public void NonNeighborsRejected()
        {
            Board board = StuckBoard();

            bool result = SwapRules.IsValidSwap(board, new Cell(0, 0), new Cell(0, 2));

            Assert.That(result, Is.False);
        }

        [Test]
        public void EmptyCellRejected()
        {
            Board board = TestBoards.FromText(
                ".BOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            bool result = SwapRules.IsValidSwap(board, new Cell(0, 0), new Cell(0, 1));

            Assert.That(result, Is.False);
        }

        [Test]
        public void TwoSpecialsRejected()
        {
            Board board = TestBoards.FromText(
                "mtOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            bool result = SwapRules.IsValidSwap(board, new Cell(0, 0), new Cell(0, 1));

            Assert.That(result, Is.False);
        }

        [Test]
        public void SpecialWithNormalAccepted()
        {
            Board board = TestBoards.FromText(
                "mBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            bool result = SwapRules.IsValidSwap(board, new Cell(0, 0), new Cell(0, 1));

            Assert.That(result, Is.True);
        }

        [Test]
        public void SwapWithoutMatchRejected()
        {
            Board board = OneMoveBoard();

            bool result = SwapRules.IsValidSwap(board, new Cell(0, 4), new Cell(0, 5));

            Assert.That(result, Is.False);
        }

        [Test]
        public void SwapWithMatchAccepted()
        {
            Board board = OneMoveBoard();

            bool result = SwapRules.IsValidSwap(board, new Cell(0, 2), new Cell(1, 2));

            Assert.That(result, Is.True);
        }

        [Test]
        public void SwapIsUndone()
        {
            Board board = OneMoveBoard();
            Board before = board.Copy();

            SwapRules.IsValidSwap(board, new Cell(0, 2), new Cell(1, 2));

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Piece beforePiece = before[row, column];
                    Piece afterPiece = board[row, column];

                    Assert.That(afterPiece.Color, Is.EqualTo(beforePiece.Color),
                        "color changed at (" + row + ", " + column + ")");
                    Assert.That(afterPiece.Kind, Is.EqualTo(beforePiece.Kind),
                        "kind changed at (" + row + ", " + column + ")");
                }
            }
        }

        [Test]
        public void StuckBoardHasNoMove()
        {
            Board board = StuckBoard();

            Assert.That(SwapRules.HasAnyValidMove(board), Is.False);
        }
    }
}