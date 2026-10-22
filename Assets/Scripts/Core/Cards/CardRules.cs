using System.Collections.Generic;

namespace DingDing.Core.Cards
{
    public static class CardRules
    {
        public static CardEffect ActiveEffect(Match match)
        {
            CardEffect effect = match.RoundEffects[match.CurrentRound];
            if (effect == null)
            {
                return new CardEffect();
            }

            return effect;
        }

        public static int ColorPercent(CardEffect effect, PieceColor color)
        {
            if (effect.BoostedColor == PieceColor.None) return 100;
            if (color == effect.BoostedColor) return effect.BoostedColorPercent;

            return effect.OtherColorsPercent;
        }

        
        public static List<int> DamagePercents(CardEffect effect, Match match)
        {
            List<int> percents = new List<int>();
            percents.Add(effect.DamagePercent);

            if (effect.IsFinisher)
            {
                if (match.OpponentHealth * 100 < match.Level.StartingHealth * 33)
                    percents.Add(effect.FinisherBelowPercent);
                else
                    percents.Add(effect.FinisherAbovePercent);
            }

            if (effect.IsConditioning)
            {
                int movesUsedBefore = match.RoundMoveCount - match.MovesLeftInRound;

                // The last moves win when both halves of the rule apply.
                if (match.MovesLeftInRound <= effect.LastMovesCount)
                    percents.Add(effect.LastMovesPercent);
                else if (movesUsedBefore < match.RoundMoveCount / 2)
                    percents.Add(effect.FirstHalfPercent);
            }

            return percents;
        }

        // Three different cards, none of them picked earlier in this match.
        public static List<CardOffer> OfferCards(Match match)
        {
            List<CardInfo> candidates = new List<CardInfo>();

            foreach (var card in CardTable.Cards)
            {
                if (!match.ChosenCards.Contains(card))
                    candidates.Add(card);
            }

            for (int index = candidates.Count - 1; index > 0; index--)
            {
                int otherIndex = match.Random.Next(0, index + 1);

                CardInfo held = candidates[index];
                candidates[index] = candidates[otherIndex];
                candidates[otherIndex] = held;
            }

            match.CurrentOffers = new List<CardOffer>();

            for (int index = 0; index < 3; index++)
            {
                CardInfo card = candidates[index];

                
                PieceColor color = PieceColor.None;
                if (card.NeedsColor)
                    color = BoardInitializer.NextColor(match.Random);

                match.CurrentOffers.Add(new CardOffer(card, color));
            }

            return match.CurrentOffers;
        }

        public static void PickCard(Match match, CardOffer offer)
        {
            int nextRound = match.CurrentRound + 1;

            match.RoundEffects[nextRound] = CardTable.CreateEffect(offer.Card.Id,offer.Color);
            match.ChosenCards.Add(offer.Card);
        }
    }
}