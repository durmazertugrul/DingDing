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
        public void HealthLooksSane()
        {
            for (int number = 1; number <= LevelTable.LevelCount; number++)
            {
                int health = LevelTable.GetLevel(number).StartingHealth;

                // A typo in the table, not a design rule: the band is wide.
                Assert.That(health, Is.InRange(50, 1000), "Level " + number + " health looks like a typo");
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