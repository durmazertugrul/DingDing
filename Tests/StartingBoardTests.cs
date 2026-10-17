using System;
using System.Collections.Generic;
using DingDing.Core;
using NUnit.Framework;

namespace DingDing.Tests
{
    [TestFixture]
    public class StartingBoardTests
    {
        private const int SeedCount = 1000;

        [Test]
        public void NoMatchesOnStart()
        {
            for (int seed = 0; seed < SeedCount; seed++)
            {
                Board board = BoardInitializer.CreateStartingBoard(new Random(seed));
                List<MatchGroup> groups = MatchFinder.FindMatchGroups(board);

                Assert.That(groups.Count, Is.EqualTo(0),
                    "seed " + seed + " produced a match on the starting board");
            }
        }

        [Test]
        public void AlwaysHasMove()
        {
            for (int seed = 0; seed < SeedCount; seed++)
            {
                Board board = BoardInitializer.CreateStartingBoard(new Random(seed));

                Assert.That(SwapRules.HasAnyValidMove(board), Is.True,
                    "seed " + seed + " produced a stuck starting board");
            }
        }

        [Test]
        public void SameSeedSameBoard()
        {
            Board firstBoard = BoardInitializer.CreateStartingBoard(new Random(42));
            Board secondBoard = BoardInitializer.CreateStartingBoard(new Random(42));

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Assert.That(secondBoard[row, column].Color,
                        Is.EqualTo(firstBoard[row, column].Color),
                        "boards differ at (" + row + ", " + column + ")");
                }
            }
        }

        [Test]
        public void DifferentSeedDifferentBoard()
        {
            Board firstBoard = BoardInitializer.CreateStartingBoard(new Random(1));
            Board secondBoard = BoardInitializer.CreateStartingBoard(new Random(2));

            bool foundDifference = false;

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    if (firstBoard[row, column].Color != secondBoard[row, column].Color)
                    {
                        foundDifference = true;
                    }
                }
            }

            Assert.That(foundDifference, Is.True,
                "two different seeds produced identical boards");
        }
    }
}