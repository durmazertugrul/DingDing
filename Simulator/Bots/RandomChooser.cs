using System;
using System.Collections.Generic;
using DingDing.Core;
using DingDing.Core.Cards;

namespace DingDing.Simulator.Bots
{
    // The player who picks a card without reading it.
    public class RandomChooser : ICardChooser
    {
        private Random chooserRandom;

        public RandomChooser(Random random)
        {
            chooserRandom = random;
        }

        public CardOffer Choose(Match match, List<CardOffer> offers)
        {
            return offers[chooserRandom.Next(0, offers.Count)];
        }
    }
}