using System.Collections.Generic;
using DingDing.Core;
using DingDing.Core.Cards;
using NUnit.Framework;

namespace DingDing.Tests
{
    public class CardTests
    {
        // A level of its own, so the tuning table can change freely without
        // breaking these tests. 100 health makes the percent thresholds exact.
        private static LevelInfo TestLevel()
        {
            return new LevelInfo(1, Difficulty.Normal, 100, 12, 12, 12);
        }

        private static Match TestMatch(int seed)
        {
            return new Match(TestLevel(), seed);
        }

        [Test]
        public void EachCardCreatesItsEffect()
        {
            CardEffect heavyPunch = CardTable.CreateEffect(CardId.HeavyPunch,
                                                           PieceColor.None);
            Assert.That(heavyPunch.DamagePercent, Is.EqualTo(130));
            Assert.That(heavyPunch.MoveModifier, Is.EqualTo(-2));

            CardEffect combination = CardTable.CreateEffect(CardId.Combination,
                                                             PieceColor.None);
            Assert.That(combination.SpecialBonusPercent, Is.EqualTo(200));
            Assert.That(combination.PiecePercent, Is.EqualTo(75));

            CardEffect finisher = CardTable.CreateEffect(CardId.Finisher,
                                                          PieceColor.None);
            Assert.That(finisher.IsFinisher, Is.True);
            Assert.That(finisher.FinisherBelowPercent, Is.EqualTo(150));
            Assert.That(finisher.FinisherAbovePercent, Is.EqualTo(80));

            CardEffect earlyAttack = CardTable.CreateEffect(CardId.EarlyAttack,
                                                             PieceColor.None);
            Assert.That(earlyAttack.StartingSpecial, Is.EqualTo(PieceKind.Medal));
            Assert.That(earlyAttack.MoveModifier, Is.EqualTo(-1));

            CardEffect water = CardTable.CreateEffect(CardId.Water, PieceColor.None);
            Assert.That(water.MoveModifier, Is.EqualTo(3));
            Assert.That(water.DamagePercent, Is.EqualTo(85));

            CardEffect coachsAdvice = CardTable.CreateEffect(CardId.CoachsAdvice,
                                                              PieceColor.None);
            Assert.That(coachsAdvice.StartingSpecial, Is.EqualTo(PieceKind.Belt));
            Assert.That(coachsAdvice.MoveModifier, Is.EqualTo(-1));

            CardEffect conditioning = CardTable.CreateEffect(CardId.Conditioning,
                                                              PieceColor.None);
            Assert.That(conditioning.IsConditioning, Is.True);
            Assert.That(conditioning.FirstHalfPercent, Is.EqualTo(80));
            Assert.That(conditioning.LastMovesPercent, Is.EqualTo(140));
            Assert.That(conditioning.LastMovesCount, Is.EqualTo(5));

            CardEffect readTheOpponent =
                CardTable.CreateEffect(CardId.ReadTheOpponent, PieceColor.Orange);
            Assert.That(readTheOpponent.BoostedColor, Is.EqualTo(PieceColor.Orange));
            Assert.That(readTheOpponent.BoostedColorPercent, Is.EqualTo(200));
            Assert.That(readTheOpponent.OtherColorsPercent, Is.EqualTo(75));
        }

        [Test]
        public void EffectOnlyInChosenRound()
        {
            Match match = TestMatch(42);
            match.CurrentRound = 1;

            CardInfo heavyPunch = new CardInfo(CardId.HeavyPunch,
                                               CardClass.Attack, false);
            CardRules.PickCard(match, new CardOffer(heavyPunch, PieceColor.None));

            match.CurrentRound = 2;
            Assert.That(CardRules.ActiveEffect(match).DamagePercent, Is.EqualTo(130));

            match.CurrentRound = 3;
            Assert.That(CardRules.ActiveEffect(match).DamagePercent, Is.EqualTo(100));
        }

        [Test]
        public void FinisherBelowThreshold()
        {
            Match match = TestMatch(42);
            match.OpponentHealth = 32;

            CardEffect effect = CardTable.CreateEffect(CardId.Finisher,
                                                        PieceColor.None);
            List<int> percents = CardRules.DamagePercents(effect, match);

            Assert.That(percents, Contains.Item(150));
        }

        [Test]
        public void FinisherAtThreshold()
        {
            Match match = TestMatch(42);
            match.OpponentHealth = 33;

            CardEffect effect = CardTable.CreateEffect(CardId.Finisher,
                                                        PieceColor.None);
            List<int> percents = CardRules.DamagePercents(effect, match);

            Assert.That(percents, Contains.Item(80));
        }

        [Test]
        public void ConditioningLastFive()
        {
            Match match = TestMatch(42);
            match.RoundMoveCount = 12;
            match.MovesLeftInRound = 5;

            CardEffect effect = CardTable.CreateEffect(CardId.Conditioning,
                                                        PieceColor.None);
            List<int> percents = CardRules.DamagePercents(effect, match);

            Assert.That(percents, Contains.Item(140));
        }

        [Test]
        public void ConditioningFirstHalf()
        {
            Match match = TestMatch(42);
            match.RoundMoveCount = 12;
            match.MovesLeftInRound = 7;      // five moves used

            CardEffect effect = CardTable.CreateEffect(CardId.Conditioning,
                                                        PieceColor.None);
            List<int> percents = CardRules.DamagePercents(effect, match);

            Assert.That(percents, Contains.Item(80));
        }

        [Test]
        public void ConditioningShortRound()
        {
            // Both halves of the rule apply here; the last moves must win.
            Match match = TestMatch(42);
            match.RoundMoveCount = 8;
            match.MovesLeftInRound = 5;      // three moves used

            CardEffect effect = CardTable.CreateEffect(CardId.Conditioning,
                                                        PieceColor.None);
            List<int> percents = CardRules.DamagePercents(effect, match);

            Assert.That(percents, Contains.Item(140));
            Assert.That(percents, Does.Not.Contain(80));
        }

        [Test]
        public void ReadColorSetAtOffer()
        {
            Match firstMatch = TestMatch(7);
            Match secondMatch = TestMatch(7);

            List<CardOffer> firstOffers = CardRules.OfferCards(firstMatch);
            List<CardOffer> secondOffers = CardRules.OfferCards(secondMatch);

            for (int index = 0; index < firstOffers.Count; index++)
            {
                Assert.That(secondOffers[index].Card.Id,
                    Is.EqualTo(firstOffers[index].Card.Id));
                Assert.That(secondOffers[index].Color,
                    Is.EqualTo(firstOffers[index].Color));
            }

            // Wherever the card does turn up, it carries a color already.
            bool cardWasSeen = false;

            for (int seed = 1; seed <= 50; seed++)
            {
                foreach (var offer in CardRules.OfferCards(TestMatch(seed)))
                {
                    if (offer.Card.Id != CardId.ReadTheOpponent) continue;

                    cardWasSeen = true;
                    Assert.That(offer.Color, Is.Not.EqualTo(PieceColor.None));
                }
            }

            Assert.That(cardWasSeen, Is.True, "the card never appeared in 50 offers");
        }

        [Test]
        public void ChosenCardNotOfferedAgain()
        {
            Match match = TestMatch(42);
            match.CurrentRound = 1;

            List<CardOffer> firstOffers = CardRules.OfferCards(match);
            CardInfo pickedCard = firstOffers[0].Card;
            CardRules.PickCard(match, firstOffers[0]);

            match.CurrentRound = 2;
            List<CardOffer> secondOffers = CardRules.OfferCards(match);

            foreach (var offer in secondOffers)
            {
                Assert.That(offer.Card.Id, Is.Not.EqualTo(pickedCard.Id));
            }
        }
    }
}