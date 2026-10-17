using System;
using System.Collections.Generic;
using DingDing.Core;
using NUnit.Framework;

namespace DingDing.Tests
{
    [TestFixture]
    public class ResolveTests
    {
        private static PlayerAction Swap(int firstRow, int firstColumn,
                                         int secondRow, int secondColumn)
        {
            return new PlayerAction(ActionType.Swap,
                new Cell(firstRow, firstColumn),
                new Cell(secondRow, secondColumn));
        }

        private static PlayerAction Tap(int row, int column)
        {
            Cell cell = new Cell(row, column);
            return new PlayerAction(ActionType.Tap, cell, cell);
        }

        // A special piece can fall right after it is born, so the tests read the
        // event instead of the board: the event holds the cell it was born in.
        private static List<SpecialCreated> CreatedSpecialEvents(List<object> events)
        {
            List<SpecialCreated> created = new List<SpecialCreated>();

            foreach (object gameEvent in events)
            {
                if (gameEvent is SpecialCreated specialCreated)
                {
                    created.Add(specialCreated);
                }
            }

            return created;
        }

        // Three reds line up on the top row, no special piece is born.
        [Test]
        public void SimpleMatchClears()
        {
            Board board = TestBoards.FromText(
                "RROGPRBO",
                "BORPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            List<object> events = new List<object>();
            MoveResult result = MoveResolver.ResolveAction(
                board, Swap(1, 2, 0, 2), new Random(1), events);

            Assert.That(result.Steps[0].ClearedColors.Count, Is.EqualTo(3));
            Assert.That(result.Steps[0].SpecialActivations, Is.EqualTo(0));
            Assert.That(result.CreatedSpecials.Count, Is.EqualTo(0));

            foreach (PieceColor clearedColor in result.Steps[0].ClearedColors)
            {
                Assert.That(clearedColor, Is.EqualTo(PieceColor.Red));
            }
        }

        // Four reds line up; the medal is born where the dragged piece landed.
        [Test]
        public void MedalSpawnsAtDraggedCell()
        {
            Board board = TestBoards.FromText(
                "RRORPRBO",
                "BORPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            List<object> events = new List<object>();
            MoveResolver.ResolveAction(board, Swap(1, 2, 0, 2), new Random(1), events);

            List<SpecialCreated> created = CreatedSpecialEvents(events);

            Assert.That(created.Count, Is.EqualTo(1));
            Assert.That(created[0].Kind, Is.EqualTo(PieceKind.Medal));
            Assert.That(created[0].Cell, Is.EqualTo(new Cell(0, 2)));
        }

        // A horizontal run and a vertical run meet at (2,2).
        [Test]
        public void BeltSpawnsAtIntersection()
        {
            Board board = TestBoards.FromText(
                "RBOGPRBO",
                "BORPRBOG",
                "RRPBBOGP",
                "GPRBOGPR",
                "PRROGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            List<object> events = new List<object>();
            MoveResolver.ResolveAction(board, Swap(1, 2, 2, 2), new Random(1), events);

            List<SpecialCreated> created = CreatedSpecialEvents(events);

            Assert.That(created.Count, Is.EqualTo(1));
            Assert.That(created[0].Kind, Is.EqualTo(PieceKind.Belt));
            Assert.That(created[0].Cell, Is.EqualTo(new Cell(2, 2)));
        }

        // A T shape with a five piece arm, so the gong wins over the belt.
        [Test]
        public void GongFromTSpawnsAtDraggedCell()
        {
            Board board = TestBoards.FromText(
                "RBOGPRBO",
                "BORPRBOG",
                "OGRRBOGP",
                "GRRBOGPR",
                "PRROGPRB",
                "RBRGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            List<object> events = new List<object>();
            MoveResolver.ResolveAction(board, Swap(2, 3, 3, 3), new Random(1), events);

            List<SpecialCreated> created = CreatedSpecialEvents(events);

            Assert.That(created.Count, Is.EqualTo(1));
            Assert.That(created[0].Kind, Is.EqualTo(PieceKind.Gong));
            Assert.That(created[0].Cell, Is.EqualTo(new Cell(3, 3)));
        }

        // One swap opens a four on the row above and a four on the row below.
        [Test]
        public void TwoGroupsTwoSpecials()
        {
            Board board = TestBoards.FromText(
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "BRRROGPR",
                "RBBBGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            List<object> events = new List<object>();
            MoveResolver.ResolveAction(board, Swap(3, 0, 4, 0), new Random(1), events);

            List<SpecialCreated> created = CreatedSpecialEvents(events);

            Assert.That(created.Count, Is.EqualTo(2));

            bool bornAtFirstCell = false;
            bool bornAtSecondCell = false;

            foreach (SpecialCreated specialCreated in created)
            {
                Assert.That(specialCreated.Kind, Is.EqualTo(PieceKind.Medal));

                if (specialCreated.Cell.Equals(new Cell(3, 0)))
                    bornAtFirstCell = true;

                if (specialCreated.Cell.Equals(new Cell(4, 0)))
                    bornAtSecondCell = true;
            }

            Assert.That(bornAtFirstCell, Is.True, "no medal was born at (3, 0)");
            Assert.That(bornAtSecondCell, Is.True, "no medal was born at (4, 0)");
        }

        // The player clears three reds; the pieces that fall line up four blues.
        // A cascade never creates a special piece.
        [Test]
        public void NoSpecialFromCascade()
        {
            Board board = TestBoards.FromText(
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOBBRBOG",
                "BBRROOGP");

            List<object> events = new List<object>();
            MoveResult result = MoveResolver.ResolveAction(
                board, Swap(6, 4, 7, 4), new Random(1), events);

            Assert.That(result.CascadeCount, Is.GreaterThanOrEqualTo(1));
            Assert.That(result.CreatedSpecials.Count, Is.EqualTo(0));
            Assert.That(result.Steps[1].ClearedColors.Count, Is.EqualTo(4));
        }

        // Tapping a medal clears its row; the pieces that fall line up three purples.
        [Test]
        public void TapCountsAsCascade()
        {
            Board board = TestBoards.FromText(
                "RBOGPRBO",
                "BOGPRBOG",
                "PGPRBOGP",
                "GPmBOGPR",
                "PRBOGPRB",
                "PBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            List<object> events = new List<object>();
            MoveResult result = MoveResolver.ResolveAction(
                board, Tap(3, 2), new Random(1), events);

            Assert.That(result.Steps[0].SpecialActivations, Is.EqualTo(1));
            Assert.That(result.CascadeCount, Is.GreaterThanOrEqualTo(1));
            Assert.That(result.CreatedSpecials.Count, Is.EqualTo(0));
        }

        // The event carries the four pieces that slid into the medal.
        [Test]
        public void SourceCellsListed()
        {
            Board board = TestBoards.FromText(
                "RRORPRBO",
                "BORPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            List<object> events = new List<object>();
            MoveResolver.ResolveAction(board, Swap(1, 2, 0, 2), new Random(1), events);

            List<SpecialCreated> created = CreatedSpecialEvents(events);

            Assert.That(created.Count, Is.EqualTo(1));
            Assert.That(created[0].SourceCells.Count, Is.EqualTo(4));
        }

        // The medal blasts first, then the trophy it hit.
        [Test]
        public void ChainOrderInEvent()
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

            List<object> events = new List<object>();
            MoveResolver.ResolveAction(board, Tap(3, 2), new Random(1), events);

            SpecialsActivated activated = null;

            foreach (object gameEvent in events)
            {
                if (gameEvent is SpecialsActivated specialsActivated)
                {
                    activated = specialsActivated;
                    break;
                }
            }

            Assert.That(activated, Is.Not.Null, "no SpecialsActivated event was recorded");
            Assert.That(activated.Activations.Count, Is.EqualTo(2));
            Assert.That(activated.Activations[0].Kind, Is.EqualTo(PieceKind.Medal));
            Assert.That(activated.Activations[1].Kind, Is.EqualTo(PieceKind.Trophy));
        }

        // The same board, the same action and the same seed give the same events.
        [Test]
        public void SameSeedSameEvents()
        {
            string firstSignature = SignatureOfRun();
            string secondSignature = SignatureOfRun();

            Assert.That(secondSignature, Is.EqualTo(firstSignature));
        }

        private static string SignatureOfRun()
        {
            Board board = TestBoards.FromText(
                "RROGPRBO",
                "BORPRBOG",
                "OGPRBOGP",
                "GPRBOGPR",
                "PRBOGPRB",
                "RBOGPRBO",
                "BOGPRBOG",
                "OGPRBOGP");

            List<object> events = new List<object>();
            MoveResolver.ResolveAction(board, Swap(1, 2, 0, 2), new Random(7), events);

            string signature = "";

            foreach (object gameEvent in events)
            {
                signature = signature + gameEvent.GetType().Name + ";";
            }

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    signature = signature + (int)board[row, column].Color;
                }
            }

            return signature;
        }
    }
}