using DingDing.Core;
using DingDing.Core.Cards;
using NUnit.Framework;

namespace DingDing.Tests
{
    // The expected numbers below assume DamagePerPiece 1 and
    // SpecialBonusDamage 5. Changing either one should fail these on purpose.
    public class DamageTests
    {
        private static Match TestMatch()
        {
            LevelInfo level = new LevelInfo(1, Difficulty.Normal, 100, 12, 12, 12);
            Match match = new Match(level, 42);
            match.RoundMoveCount = 12;
            match.MovesLeftInRound = 12;
            return match;
        }

        private static ClearStep StepOf(params PieceColor[] colors)
        {
            ClearStep step = new ClearStep();

            foreach (var color in colors)
            {
                step.ClearedColors.Add(color);
            }

            return step;
        }

        private static ClearStep StepOf(PieceColor color, int count)
        {
            ClearStep step = new ClearStep();

            for (int index = 0; index < count; index++)
            {
                step.ClearedColors.Add(color);
            }

            return step;
        }

        [Test]
        public void PlainStep()
        {
            ClearStep step = StepOf(PieceColor.Red, 3);

            int damage = DamageCalculator.StepDamage(TestMatch(), step,
                                                      new CardEffect());

            Assert.That(damage, Is.EqualTo(3));
        }

        [Test]
        public void HeavyPunchStep()
        {
            ClearStep step = StepOf(PieceColor.Red, 10);
            CardEffect effect = CardTable.CreateEffect(CardId.HeavyPunch,
                                                        PieceColor.None);

            int damage = DamageCalculator.StepDamage(TestMatch(), step, effect);

            Assert.That(damage, Is.EqualTo(13));
        }

        [Test]
        public void RoundsHalfUp()
        {
            // Ten pieces at 85 percent is 8.5, which must land on 9, not 8.
            ClearStep step = StepOf(PieceColor.Red, 10);
            CardEffect effect = CardTable.CreateEffect(CardId.Water,
                                                        PieceColor.None);

            int damage = DamageCalculator.StepDamage(TestMatch(), step, effect);

            Assert.That(damage, Is.EqualTo(9));
        }

        [Test]
        public void ReadTheOpponentStep()
        {
            // Three boosted pieces at 200 and three others at 75 is 8.25.
            ClearStep step = StepOf(PieceColor.Red, PieceColor.Red, PieceColor.Red,
                                    PieceColor.Blue, PieceColor.Blue,
                                    PieceColor.Blue);

            CardEffect effect = CardTable.CreateEffect(CardId.ReadTheOpponent,
                                                        PieceColor.Red);

            int damage = DamageCalculator.StepDamage(TestMatch(), step, effect);

            Assert.That(damage, Is.EqualTo(8));
        }

        [Test]
        public void SpecialClearedColorCounts()
        {
            // The four pieces a medal wiped out are boosted; the special
            // bonus itself is not.
            ClearStep step = StepOf(PieceColor.Red, 4);
            step.SpecialActivations = 1;

            CardEffect effect = CardTable.CreateEffect(CardId.ReadTheOpponent,
                                                        PieceColor.Red);

            int damage = DamageCalculator.StepDamage(TestMatch(), step, effect);

            Assert.That(damage, Is.EqualTo(13));
        }

        [Test]
        public void ScoreOfStep()
        {
            MoveResult result = new MoveResult();

            result.Steps.Add(StepOf(PieceColor.Red, 5));

            ClearStep secondStep = StepOf(PieceColor.Blue, 3);
            secondStep.SpecialActivations = 1;
            result.Steps.Add(secondStep);

            result.CreatedSpecials.Add((PieceKind.Medal, 4));

            // 5*10 + 3*10 + 1*50 + 4*25
            Assert.That(ScoreCalculator.ScoreFrom(result), Is.EqualTo(230));
        }
    }
}