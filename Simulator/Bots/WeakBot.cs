using System;
using System.Collections.Generic;
using DingDing.Core;

namespace DingDing.Simulator.Bots
{
    // Takes the legal move that does the least damage. Random play is not a
    // floor in a match-3 game: any clear deals damage and cascades arrive on
    // their own, so a real floor has to choose badly on purpose.
    public class WeakBot : IActionBot
    {
        private Random botRandom;

        public WeakBot(Random random)
        {
            botRandom = random;
        }

        public PlayerAction ChooseAction(Match match)
        {
            List<PlayerAction> candidates = BotMoves.ListCandidates(match);
            if (candidates.Count == 0) return null;

            PlayerAction worstAction = null;
            int worstDamage = int.MaxValue;

            foreach (var candidate in candidates)
            {
                Match copy = match.CopyForBot(new Random(botRandom.Next()));

                int healthBefore = copy.OpponentHealth;
                MatchFlow.PlayAction(copy, candidate);
                int damage = healthBefore - copy.OpponentHealth;

                if (damage < worstDamage)
                {
                    worstAction = candidate;
                    worstDamage = damage;
                }
            }

            return worstAction;
        }
    }
}