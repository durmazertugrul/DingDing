using System.Collections.Generic;
using DingDing.Core;
using DingDing.Core.Cards;

namespace DingDing.Simulator.Bots
{
    public interface ICardChooser
    {
        CardOffer Choose(Match match, List<CardOffer> offers);
    }
}