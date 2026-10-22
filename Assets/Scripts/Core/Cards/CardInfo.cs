namespace DingDing.Core.Cards
{
    // What the corner screen needs to know about a card before it is picked.
    public class CardInfo
    {
        public CardId Id { get; }
        public CardClass CardClass { get; }
        public bool NeedsColor { get; }          // only Read the Opponent

        public CardInfo(CardId id, CardClass cardClass, bool needsColor)
        {
            Id = id;
            CardClass = cardClass;
            NeedsColor = needsColor;
        }
        

    }

}