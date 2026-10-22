namespace DingDing.Core.Cards
{
    public class CardOffer
    {
        public CardInfo Card { get; }
        public PieceColor Color { get; }         // None unless the card needs one

        public CardOffer(CardInfo card, PieceColor color)
        {
            Card = card;
            Color = color;
        }
       
    }
}