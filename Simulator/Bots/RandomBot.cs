using System;
using System.Collections.Generic;
using DingDing.Core;

namespace DingDing.Simulator.Bots
{
    // Plays any legal move. The floor of what a player could score.
    public class RandomBot : IActionBot
    {
        private Random botRandom;

        public RandomBot(Random random)
        {
            botRandom = random;
        }

        public PlayerAction ChooseAction(Match match)
        {
            List<PlayerAction> candidates = BotMoves.ListCandidates(match);
            if (candidates.Count == 0) return null;

            return candidates[botRandom.Next(0, candidates.Count)];
        }
    }
}