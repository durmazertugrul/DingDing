using System.Collections.Generic;
using DingDing.Core;
using DingDing.Core.Cards;

namespace DingDing.Simulator.Bots
{
    // The player who reads the card and the screen: health low means finish
    // it, moves short means buy moves, otherwise take the strongest card.
    public class HeuristicChooser : ICardChooser
    {
        public CardOffer Choose(Match match, List<CardOffer> offers)
        {
            CardOffer finisher = Find(offers, CardId.Finisher);
            if (finisher != null && match.OpponentHealth * 100 < match.Level.StartingHealth * 33) return finisher;

            CardOffer water = Find(offers, CardId.Water);
            int nextRound = match.CurrentRound + 1;
            if (water != null && match.Level.MovesPerRound[nextRound] <= GameRules.LowMovesLimit) return water;

            CardOffer heavyPunch = Find(offers, CardId.HeavyPunch);
            if (heavyPunch != null) return heavyPunch;

            foreach (var id in GameRules.CardPriority)
            {
                CardOffer offer = Find(offers, id);
                if (offer != null) return offer;
            }

            return offers[0];
        }

        private static CardOffer Find(List<CardOffer> offers, CardId id)
        {
            foreach (var offer in offers)
            {
                if (offer.Card.Id == id) return offer;
            }

            return null;
        }
    }
}