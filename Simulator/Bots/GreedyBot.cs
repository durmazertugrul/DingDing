using System;
using System.Collections.Generic;
using DingDing.Core;

namespace DingDing.Simulator.Bots
{
    // Tries every legal move on a throwaway copy and keeps the one that
    // takes the most health. A practical ceiling, not perfect play: it
    // looks one move ahead and never plans a cascade.
    public class GreedyBot : IActionBot
    {
        private Random botRandom;

        public GreedyBot(Random random)
        {
            botRandom = random;
        }

        public PlayerAction ChooseAction(Match match)
        {
            List<PlayerAction> candidates = BotMoves.ListCandidates(match);
            if (candidates.Count == 0) return null;

            PlayerAction bestAction = null;
            int bestDamage = -1;

            foreach (var candidate in candidates)
            {
                Match copy = match.CopyForBot(new Random(botRandom.Next()));

                int healthBefore = copy.OpponentHealth;
                MatchFlow.PlayAction(copy, candidate);
                int damage = healthBefore - copy.OpponentHealth;

                // A tie keeps the first candidate, so the same seeds give
                // the same match every time.
                if (damage > bestDamage)
                {
                    bestAction = candidate;
                    bestDamage = damage;
                }
            }

            return bestAction;
        }
    }
}