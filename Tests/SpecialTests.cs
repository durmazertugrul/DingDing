using System.Collections.Generic;
using DingDing.Core;
using NUnit.Framework;

namespace DingDing.Tests
{
    [TestFixture]
    public class SpecialTests
    {
        private static List<Cell> StartingAt(int row, int column)
        {
            return new List<Cell> { new Cell(row, column) };
        }

        [Test]
        public void MedalClearsRow()
        {
            Board board = TestBoards.FromText(
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPmBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            var (activations, allCleared) = SpecialActivator.ActivateSpecials(
                board, StartingAt(3, 2));

            Assert.That(activations.Count, Is.EqualTo(1));
            Assert.That(activations[0].Kind, Is.EqualTo(PieceKind.Medal));
            Assert.That(allCleared.Count, Is.EqualTo(8));

            for (int column = 0; column < Board.Columns; column++)
            {
                Assert.That(allCleared.Contains(new Cell(3, column)), Is.True,
                    "cell (3, " + column + ") was not cleared");
            }
        }

        [Test]
        public void TrophyClearsColumn()
        {
            Board board = TestBoards.FromText(
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGtRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            var (activations, allCleared) = SpecialActivator.ActivateSpecials(
                board, StartingAt(4, 5));

            Assert.That(activations.Count, Is.EqualTo(1));
            Assert.That(activations[0].Kind, Is.EqualTo(PieceKind.Trophy));
            Assert.That(allCleared.Count, Is.EqualTo(8));

            for (int row = 0; row < Board.Rows; row++)
            {
                Assert.That(allCleared.Contains(new Cell(row, 5)), Is.True,
                    "cell (" + row + ", 5) was not cleared");
            }
        }

        [Test]
        public void BeltInCornerStaysInside()
        {
            Board board = TestBoards.FromText(
                "bBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            var (activations, allCleared) = SpecialActivator.ActivateSpecials(
                board, StartingAt(0, 0));

            Assert.That(activations.Count, Is.EqualTo(1));
            Assert.That(allCleared.Count, Is.EqualTo(4));

            Assert.That(allCleared.Contains(new Cell(0, 0)), Is.True);
            Assert.That(allCleared.Contains(new Cell(0, 1)), Is.True);
            Assert.That(allCleared.Contains(new Cell(1, 0)), Is.True);
            Assert.That(allCleared.Contains(new Cell(1, 1)), Is.True);
        }

        // Red and Blue both appear 16 times, the other colors fewer.
        [Test]
        public void GongTieChoosesRed()
        {
            Board board = TestBoards.FromText(
                "gRRRRRRR",
                "RRRRRRRR",
                "RBBBBBBB",
                "BBBBBBBB",
                "BOOOOOOO",
                "OOOOGGGG",
                "GGGGGGPP",
                "PPPPPPPP");

            var (activations, allCleared) = SpecialActivator.ActivateSpecials(
                board, StartingAt(0, 0));

            Assert.That(activations.Count, Is.EqualTo(1));
            Assert.That(activations[0].ClearedCells.Count, Is.EqualTo(16));

            foreach (Cell cell in activations[0].ClearedCells)
            {
                Assert.That(board[cell].Color, Is.EqualTo(PieceColor.Red),
                    "gong cleared a piece that is not red at " + cell);
            }
        }

        // Blue and Green both appear 16 times, Red fewer.
        // Blue comes before Green in PieceColor, so Blue wins the tie.
        [Test]
        public void GongTieWithoutRed()
        {
            Board board = TestBoards.FromText(
                "gBBBBBBB",
                "BBBBBBBB",
                "BGGGGGGG",
                "GGGGGGGG",
                "GRRRRRRR",
                "RRRROOOO",
                "OOOOOOPP",
                "PPPPPPPP");

            var (activations, allCleared) = SpecialActivator.ActivateSpecials(
                board, StartingAt(0, 0));

            Assert.That(activations[0].ClearedCells.Count, Is.EqualTo(16));

            foreach (Cell cell in activations[0].ClearedCells)
            {
                Assert.That(board[cell].Color, Is.EqualTo(PieceColor.Blue),
                    "gong cleared a piece that is not blue at " + cell);
            }
        }

        [Test]
        public void MedalTriggersTrophy()
        {
            Board board = TestBoards.FromText(
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPmBOGtR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            var (activations, allCleared) = SpecialActivator.ActivateSpecials(
                board, StartingAt(3, 2));

            Assert.That(activations.Count, Is.EqualTo(2));
            Assert.That(activations[0].Kind, Is.EqualTo(PieceKind.Medal));
            Assert.That(activations[1].Kind, Is.EqualTo(PieceKind.Trophy));
        }

        [Test]
        public void SpecialNeverTwice()
        {
            Board board = TestBoards.FromText(
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GmRBOmtR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            var (activations, allCleared) = SpecialActivator.ActivateSpecials(
                board, StartingAt(3, 1));

            int trophyActivations = 0;

            foreach (SpecialActivation activation in activations)
            {
                if (activation.Kind == PieceKind.Trophy)
                {
                    trophyActivations++;
                }
            }

            Assert.That(trophyActivations, Is.EqualTo(1),
                "the trophy blasted more than once");
        }
    }
}