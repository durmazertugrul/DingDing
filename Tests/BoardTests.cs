using DingDing.Core;
using NUnit.Framework;

namespace DingDing.Tests
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void CopyIsIndependent()
        {
            Board board = TestBoards.FromText(
                "RRRRRRRR",
                "BBBBBBBB",
                "OOOOOOOO",
                "GGGGGGGG",
                "PPPPPPPP",
                "RRRRRRRR",
                "BBBBBBBB",
                "OOOOOOOO");

            Board copy = board.Copy();
            copy[0, 0] = new Piece(PieceColor.Purple, PieceKind.Normal);

            Assert.That(board[0, 0].Color, Is.EqualTo(PieceColor.Red));
        }

        [Test]
        public void FromTextReadsTopRowFirst()
        {
            Board board = TestBoards.FromText(
                "RRRRRRRR",
                "BBBBBBBB",
                "OOOOOOOO",
                "GGGGGGGG",
                "PPPPPPPP",
                "RRRRRRRR",
                "BBBBBBBB",
                "OOOOOOOO");

            for (int column = 0; column < Board.Columns; column++)
            {
                Assert.That(board[0, column].Color, Is.EqualTo(PieceColor.Red));
            }
        }

        [Test]
        public void IsInsideRejectsOutside()
        {
            Board board = new Board();

            Assert.That(board.IsInside(-1, 0), Is.False);
            Assert.That(board.IsInside(0, -1), Is.False);
            Assert.That(board.IsInside(0, Board.Columns), Is.False);
            Assert.That(board.IsInside(Board.Rows, 0), Is.False);
            Assert.That(board.IsInside(0, 0), Is.True);
            Assert.That(board.IsInside(7, 7), Is.True);
        }
    }
}