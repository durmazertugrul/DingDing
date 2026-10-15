using System.Collections.Generic;
using DingDing.Core;
using NUnit.Framework;

namespace DingDing.Tests
{
    [TestFixture]
    public class ShapeTests
    {
        private static PieceKind ShapeOfSingleGroup(Board board)
        {
            List<MatchGroup> groups = MatchFinder.FindMatchGroups(board);
            Assert.That(groups.Count, Is.EqualTo(1), "expected exactly one group");
            return MatchFinder.ClassifyShape(groups[0]);
        }

        [Test]
        public void StraightThreeIsNormal()
        {
            Board board = TestBoards.FromText(
                "RRRBOGOG",
                "OPBROBBR",
                "OPPGPRGR",
                "GBRROBPG",
                "RBPGBOBR",
                "GPPRPBPO",
                "PGRBGORP",
                "BRBPGGOP");

            Assert.That(ShapeOfSingleGroup(board), Is.EqualTo(PieceKind.Normal));
        }

        [Test]
        public void HorizontalFourIsMedal()
        {
            Board board = TestBoards.FromText(
                "RRRRBOGO",
                "GOPBROBB",
                "ROPPGPRG",
                "RGBRROBP",
                "GRBPGBOB",
                "RGPPRPBP",
                "OPGRBGOR",
                "PBRBPGGO");

            Assert.That(ShapeOfSingleGroup(board), Is.EqualTo(PieceKind.Medal));
        }

        [Test]
        public void VerticalFourIsTrophy()
        {
            Board board = TestBoards.FromText(
                "RBOGOGOP",
                "RBROBBRO",
                "RPPGPRGR",
                "RGBRROBP",
                "GRBPGBOB",
                "RGPPRPBP",
                "OPGRBGOR",
                "PBRBPGGO");

            Assert.That(ShapeOfSingleGroup(board), Is.EqualTo(PieceKind.Trophy));
        }

        [Test]
        public void LShapeIsBelt()
        {
            Board board = TestBoards.FromText(
                "RRRBOGOG",
                "ROPBROBB",
                "RROPPGPR",
                "GRGBRROB",
                "PGRBPGBO",
                "BRGPPRPB",
                "POPGRBGO",
                "RPBRBPGG");

            Assert.That(ShapeOfSingleGroup(board), Is.EqualTo(PieceKind.Belt));
        }

        [Test]
        public void TShapeIsBelt()
        {
            Board board = TestBoards.FromText(
                "RRROBRRO",
                "PRGPROPO",
                "PRPPOBOB",
                "GBRBBPBB",
                "ROBRPRGR",
                "PGPRRBOP",
                "POBOGBBR",
                "ORGOBGPG");

            Assert.That(ShapeOfSingleGroup(board), Is.EqualTo(PieceKind.Belt));
        }

        [Test]
        public void StraightFiveIsGong()
        {
            Board board = TestBoards.FromText(
                "RRRRRBOG",
                "OGOPBROB",
                "BROPPGPR",
                "GRGBRROB",
                "PGRBPGBO",
                "BRGPPRPB",
                "POPGRBGO",
                "RPBRBPGG");

            Assert.That(ShapeOfSingleGroup(board), Is.EqualTo(PieceKind.Gong));
        }

        [Test]
        public void StraightSixIsGong()
        {
            Board board = TestBoards.FromText(
                "RRRRRRBO",
                "GOGOPBRO",
                "BBROPPGP",
                "RGRGBRRO",
                "BPGRBPGB",
                "OBRGPPRP",
                "BPOPGRBG",
                "ORPBRBPG");

            Assert.That(ShapeOfSingleGroup(board), Is.EqualTo(PieceKind.Gong));
        }

        [Test]
        public void TWithFiveArmIsGong()
        {
            Board board = TestBoards.FromText(
                "BRPGBGPR",
                "ORPGRBOP",
                "RRRPOPPR",
                "RRBGGPRB",
                "BROGPOPP",
                "PGPROOBO",
                "POGORPRO",
                "RGGORRPP");

            Assert.That(ShapeOfSingleGroup(board), Is.EqualTo(PieceKind.Gong));
        }

        [Test]
        public void HShapeIsNormal()
        {
            Board board = TestBoards.FromText(
                "RBROGOGO",
                "RRRPBROB",
                "RBRROPPG",
                "PRGRGBRR",
                "OBPGRBPG",
                "BOBRGPPR",
                "PBPOPGRB",
                "GORPBRBP");

            Assert.That(ShapeOfSingleGroup(board), Is.EqualTo(PieceKind.Normal));
        }

        [Test]
        public void TwoSeparateGroups()
        {
            Board board = TestBoards.FromText(
                "RRROOBGG",
                "BORRPPOP",
                "BGBORBPP",
                "RRPPGBOB",
                "RBOGBOPP",
                "PGOPBBBP",
                "OOPOGPOR",
                "GOPRBRPG");

            List<MatchGroup> groups = MatchFinder.FindMatchGroups(board);

            Assert.That(groups.Count, Is.EqualTo(2));
        }

        [Test]
        public void SpecialBreaksRun()
        {
            Board board = TestBoards.FromText(
                "RRmRRORB",
                "OBBRPROO",
                "BRGPBGOG",
                "GGPORPBB",
                "RBBOBGPB",
                "BGBRPOGG",
                "RBOBRRBP",
                "OOBOROGP");

            List<MatchGroup> groups = MatchFinder.FindMatchGroups(board);

            Assert.That(groups.Count, Is.EqualTo(0));
        }
    }
}