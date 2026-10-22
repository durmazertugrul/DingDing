using DingDing.Core;
using NUnit.Framework;

namespace DingDing.Tests
{
    // Guards the hand-typed level table, which gets edited every tuning round.
    public class LevelTests
    {
        [Test]
        public void EveryLevelHasThreeRounds()
        {
            for (int number = 1; number <= LevelTable.LevelCount; number++)
            {
                LevelInfo level = LevelTable.GetLevel(number);

                for (int round = 1; round <= GameRules.RoundCount; round++)
                {
                    Assert.That(level.MovesPerRound[round], Is.GreaterThan(0),
                        "Level " + number + " round " + round + " has no moves");
                }
            }
        }

        [Test]
        public void HealthRisesWithLevel()
        {
            for (int number = 2; number <= LevelTable.LevelCount; number++)
            {
                int previousHealth = LevelTable.GetLevel(number - 1).StartingHealth;
                int currentHealth = LevelTable.GetLevel(number).StartingHealth;

                Assert.That(currentHealth, Is.GreaterThanOrEqualTo(previousHealth),
                    "Level " + number + " is softer than the one before it");
            }
        }

        [Test]
        public void GetLevelReturnsRequestedNumber()
        {
            for (int number = 1; number <= LevelTable.LevelCount; number++)
            {
                Assert.That(LevelTable.GetLevel(number).Number, Is.EqualTo(number));
            }
        }

        [Test]
        public void NewMatchStartsAtFullHealth()
        {
            LevelInfo level = LevelTable.GetLevel(4);
            Match match = new Match(level, 42);

            Assert.That(match.OpponentHealth, Is.EqualTo(level.StartingHealth));
            Assert.That(match.CurrentRound, Is.EqualTo(0));
            Assert.That(match.Score, Is.EqualTo(0));
            Assert.That(match.Phase, Is.EqualTo(MatchPhase.Playing));
            Assert.That(match.Outcome, Is.EqualTo(MatchOutcome.Undecided));

            for (int round = 1; round <= GameRules.RoundCount; round++)
            {
                Assert.That(match.RoundResults[round],
                    Is.EqualTo(RoundResult.NotPlayed));
            }
        }
    }
}