using System.Collections.Generic;
using DingDing.Core;
using DingDing.Core.Cards;
using NUnit.Framework;

namespace DingDing.Tests
{
    public class MatchFlowTests
    {
        // Short rounds and a health pool nothing can empty, so the flow can be
        // driven to its end without a knockout getting in the way.
        private static LevelInfo EndlessLevel()
        {
            return new LevelInfo(1, Difficulty.Normal, 1000000, 3, 3, 3);
        }

        private static bool PlayFirstValidMove(Match match)
        {
            List<(Cell, Cell)> swaps = SwapRules.ListValidSwaps(match.Board);
            if (swaps.Count == 0) return false;

            PlayerAction action = new PlayerAction(ActionType.Swap,
                                                    swaps[0].Item1,
                                                    swaps[0].Item2);

            return MatchFlow.PlayAction(match, action) != null;
        }

        // Picks a card that leaves the board alone, so board comparisons stay
        // meaningful. Two of the eight cards drop a special piece on it.
        private static CardOffer HarmlessOffer(List<CardOffer> offers)
        {
            foreach (var offer in offers)
            {
                if (offer.Card.Id == CardId.EarlyAttack) continue;
                if (offer.Card.Id == CardId.CoachsAdvice) continue;

                return offer;
            }

            return offers[0];
        }

        private static void PlayUntilRoundEnds(Match match)
        {
            for (int guard = 0; guard < 50; guard++)
            {
                if (match.Phase != MatchPhase.Playing) return;
                if (!PlayFirstValidMove(match)) return;
            }
        }

        [Test]
        public void FullMatchRuns()
        {
            var startup = MatchFlow.StartMatch(EndlessLevel(), 42);
            Match match = startup.Match;

            for (int guard = 0; guard < 100; guard++)
            {
                if (match.Phase == MatchPhase.Finished) break;

                if (match.Phase == MatchPhase.Corner)
                {
                    List<CardOffer> offers = CardRules.OfferCards(match);
                    MatchFlow.SelectCornerCard(match, HarmlessOffer(offers));
                    continue;
                }

                PlayFirstValidMove(match);
            }

            Assert.That(match.Phase, Is.EqualTo(MatchPhase.Finished));
            Assert.That(match.CurrentRound, Is.EqualTo(GameRules.RoundCount));
        }

        [Test]
        public void BoardKeptBetweenRounds()
        {
            var startup = MatchFlow.StartMatch(EndlessLevel(), 7);
            Match match = startup.Match;

            PlayUntilRoundEnds(match);
            Assert.That(match.Phase, Is.EqualTo(MatchPhase.Corner));

            Board before = match.Board.Copy();

            List<CardOffer> offers = CardRules.OfferCards(match);
            MatchFlow.SelectCornerCard(match, HarmlessOffer(offers));

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int column = 0; column < Board.Columns; column++)
                {
                    Piece expected = before[row, column];
                    Piece actual = match.Board[row, column];

                    Assert.That(actual.Color, Is.EqualTo(expected.Color));
                    Assert.That(actual.Kind, Is.EqualTo(expected.Kind));
                }
            }
        }

        [Test]
        public void EarlyAttackNotOnEdge()
        {
            CardInfo earlyAttack = new CardInfo(CardId.EarlyAttack,
                                                 CardClass.Attack, false);
            int medalsSeen = 0;

            for (int seed = 1; seed <= 200; seed++)
            {
                var startup = MatchFlow.StartMatch(EndlessLevel(), seed);
                Match match = startup.Match;

                PlayUntilRoundEnds(match);
                if (match.Phase != MatchPhase.Corner) continue;

                List<object> events = MatchFlow.SelectCornerCard(match,
                    new CardOffer(earlyAttack, PieceColor.None));

                foreach (var gameEvent in events)
                {
                    SpecialCreated created = gameEvent as SpecialCreated;
                    if (created == null) continue;
                    if (created.Kind != PieceKind.Medal) continue;

                    medalsSeen++;
                    Assert.That(created.Cell.Row, Is.InRange(1, Board.Rows - 2));
                    Assert.That(created.Cell.Column,
                        Is.InRange(1, Board.Columns - 2));
                }
            }

            Assert.That(medalsSeen, Is.GreaterThan(0), "no medal was ever placed");
        }

        [Test]
        public void KnockoutStopsEventList()
        {
            var startup = MatchFlow.StartMatch(EndlessLevel(), 42);
            Match match = startup.Match;
            match.OpponentHealth = 1;

            List<(Cell, Cell)> swaps = SwapRules.ListValidSwaps(match.Board);
            PlayerAction action = new PlayerAction(ActionType.Swap,
                                                    swaps[0].Item1,
                                                    swaps[0].Item2);

            List<object> events = MatchFlow.PlayAction(match, action);

            OpponentDamaged lastEvent = events[events.Count - 1]
                as OpponentDamaged;

            Assert.That(lastEvent, Is.Not.Null,
                "the list must end on the damage that finished the match");
            Assert.That(lastEvent.HealthAfter, Is.EqualTo(0));
            Assert.That(match.Phase, Is.EqualTo(MatchPhase.Finished));
            Assert.That(match.Outcome, Is.EqualTo(MatchOutcome.Knockout));
        }

        [Test]
        public void KnockoutInRoundTwo()
        {
            var startup = MatchFlow.StartMatch(EndlessLevel(), 7);
            Match match = startup.Match;

            PlayUntilRoundEnds(match);
            List<CardOffer> offers = CardRules.OfferCards(match);
            MatchFlow.SelectCornerCard(match, HarmlessOffer(offers));

            Assert.That(match.CurrentRound, Is.EqualTo(2));

            match.OpponentHealth = 1;
            PlayFirstValidMove(match);

            Assert.That(match.RoundResults[1], Is.Not.EqualTo(RoundResult.NotPlayed));
            Assert.That(match.RoundResults[2], Is.EqualTo(RoundResult.Knockout));
            Assert.That(match.RoundResults[3], Is.EqualTo(RoundResult.NotPlayed));
        }

        [Test]
        public void LossAfterThreeRounds()
        {
            var startup = MatchFlow.StartMatch(EndlessLevel(), 42);
            Match match = startup.Match;

            for (int guard = 0; guard < 100; guard++)
            {
                if (match.Phase == MatchPhase.Finished) break;

                if (match.Phase == MatchPhase.Corner)
                {
                    List<CardOffer> offers = CardRules.OfferCards(match);
                    MatchFlow.SelectCornerCard(match, HarmlessOffer(offers));
                    continue;
                }

                PlayFirstValidMove(match);
            }

            Assert.That(match.Outcome, Is.EqualTo(MatchOutcome.Loss));
            Assert.That(match.KnockoutMove, Is.EqualTo(0));
        }

        [Test]
        public void LastMoveCascadeCounts()
        {
            // Nothing may fall between the damage events and the round total,
            // including the cascade the last move sets off.
            var startup = MatchFlow.StartMatch(EndlessLevel(), 11);
            Match match = startup.Match;

            int damageSeen = 0;

            for (int guard = 0; guard < 50; guard++)
            {
                if (match.Phase != MatchPhase.Playing) break;

                List<(Cell, Cell)> swaps = SwapRules.ListValidSwaps(match.Board);
                if (swaps.Count == 0) break;

                PlayerAction action = new PlayerAction(ActionType.Swap,
                                                        swaps[0].Item1,
                                                        swaps[0].Item2);

                List<object> events = MatchFlow.PlayAction(match, action);
                if (events == null) break;

                foreach (var gameEvent in events)
                {
                    OpponentDamaged damaged = gameEvent as OpponentDamaged;
                    if (damaged != null) damageSeen = damageSeen + damaged.Amount;
                }
            }

            Assert.That(match.RoundDamage[1], Is.EqualTo(damageSeen));
            Assert.That(match.RoundDamage[1], Is.GreaterThan(0));
        }

        [Test]
        public void RoundWonExactlyThird()
        {
            Assert.That(MatchFlow.RoundWasWon(100, 300), Is.True);
        }

        [Test]
        public void RoundLostBelowThird()
        {
            Assert.That(MatchFlow.RoundWasWon(99, 300), Is.False);
        }

        [Test]
        public void LeftoverAndEarlyBonus()
        {
            var startup = MatchFlow.StartMatch(EndlessLevel(), 42);
            Match match = startup.Match;

            match.RoundMoveCount = 12;
            match.MovesLeftInRound = 5;
            match.OpponentHealth = 1;

            int scoreBefore = match.Score;
            PlayFirstValidMove(match);

            // Four moves were left over, and two rounds were never needed.
            int expectedBonus = 4 * GameRules.PointsPerLeftoverMove
                              + 2 * GameRules.EarlyKnockoutBonusPerRound;

            Assert.That(match.Score - scoreBefore,
                Is.GreaterThanOrEqualTo(expectedBonus));
        }

        [Test]
        public void KnockoutMoveValue()
        {
            var startup = MatchFlow.StartMatch(EndlessLevel(), 42);
            Match match = startup.Match;

            match.RoundMoveCount = 12;
            match.MovesLeftInRound = 7;      // five moves already spent
            match.OpponentHealth = 1;

            PlayFirstValidMove(match);

            Assert.That(match.KnockoutMove, Is.EqualTo(6));
        }

        [Test]
        public void RejectedActionCostsNothing()
        {
            var startup = MatchFlow.StartMatch(EndlessLevel(), 42);
            Match match = startup.Match;

            int movesBefore = match.MovesLeftInRound;
            int scoreBefore = match.Score;

            PlayerAction action = new PlayerAction(ActionType.Swap,
                                                    new Cell(0, 0),
                                                    new Cell(5, 5));

            Assert.That(MatchFlow.PlayAction(match, action), Is.Null);
            Assert.That(match.MovesLeftInRound, Is.EqualTo(movesBefore));
            Assert.That(match.Score, Is.EqualTo(scoreBefore));
        }

        [Test]
        public void NoShuffleAfterLastMove()
        {
            var startup = MatchFlow.StartMatch(EndlessLevel(), 42);
            Match match = startup.Match;

            match.CurrentRound = GameRules.RoundCount;
            match.RoundMoveCount = 1;
            match.MovesLeftInRound = 1;

            List<(Cell, Cell)> swaps = SwapRules.ListValidSwaps(match.Board);
            PlayerAction action = new PlayerAction(ActionType.Swap,
                                                    swaps[0].Item1,
                                                    swaps[0].Item2);

            List<object> events = MatchFlow.PlayAction(match, action);

            foreach (var gameEvent in events)
            {
                Assert.That(gameEvent, Is.Not.InstanceOf<BoardShuffled>());
            }

            Assert.That(match.Phase, Is.EqualTo(MatchPhase.Finished));
        }

        [Test]
        public void SameSeedSameMatch()
        {
            Match firstMatch = MatchFlow.StartMatch(EndlessLevel(), 99).Match;
            Match secondMatch = MatchFlow.StartMatch(EndlessLevel(), 99).Match;

            PlayWholeMatch(firstMatch);
            PlayWholeMatch(secondMatch);

            Assert.That(secondMatch.Score, Is.EqualTo(firstMatch.Score));
            Assert.That(secondMatch.Outcome, Is.EqualTo(firstMatch.Outcome));
            Assert.That(secondMatch.RoundDamage[1],
                Is.EqualTo(firstMatch.RoundDamage[1]));
        }

        private static void PlayWholeMatch(Match match)
        {
            for (int guard = 0; guard < 100; guard++)
            {
                if (match.Phase == MatchPhase.Finished) return;

                if (match.Phase == MatchPhase.Corner)
                {
                    List<CardOffer> offers = CardRules.OfferCards(match);
                    MatchFlow.SelectCornerCard(match, HarmlessOffer(offers));
                    continue;
                }

                if (!PlayFirstValidMove(match)) return;
            }
        }
    }
}