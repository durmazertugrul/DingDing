namespace DingDing.Core.Cards
{
    public static class CardTable
    {
        private static readonly CardInfo[] cards = new CardInfo[]
        {
            new CardInfo(CardId.HeavyPunch,      CardClass.Attack, false),
            new CardInfo(CardId.Combination,     CardClass.Attack, false),
            new CardInfo(CardId.Finisher,        CardClass.Attack, false),
            new CardInfo(CardId.EarlyAttack,     CardClass.Attack, false),
            new CardInfo(CardId.Water,           CardClass.Tactic, false),
            new CardInfo(CardId.CoachsAdvice,    CardClass.Tactic, false),
            new CardInfo(CardId.Conditioning,    CardClass.Tactic, false),
            new CardInfo(CardId.ReadTheOpponent, CardClass.Tactic, true)
        };

        public static CardInfo[] Cards { get { return cards; } }



        // A fresh effect every call, so a picked card can never be mutated
        // through the one the table holds.
        public static CardEffect CreateEffect(CardId id, PieceColor boostedColor)
        {
            switch (id)
            {
                case CardId.HeavyPunch:
                    return new CardEffect { DamagePercent = 130, MoveModifier = -2 };

                case CardId.Combination:
                    return new CardEffect
                    {
                        SpecialBonusPercent = 200,
                        PiecePercent = 75
                    };

                case CardId.Finisher:
                    return new CardEffect
                    {
                        IsFinisher = true,
                        FinisherBelowPercent = 150,
                        FinisherAbovePercent = 80
                    };

                case CardId.EarlyAttack:
                    return new CardEffect
                    {
                        StartingSpecial = PieceKind.Medal,
                        MoveModifier = -1
                    };

                case CardId.Water:
                    return new CardEffect { MoveModifier = 3, DamagePercent = 85 };

                case CardId.CoachsAdvice:
                    return new CardEffect
                    {
                        StartingSpecial = PieceKind.Belt,
                        MoveModifier = -1
                    };

                case CardId.Conditioning:
                    return new CardEffect
                    {
                        IsConditioning = true,
                        FirstHalfPercent = 80,
                        LastMovesPercent = 140,
                        LastMovesCount = 5
                    };

                case CardId.ReadTheOpponent:
                    return new CardEffect
                    {
                        BoostedColor = boostedColor,
                        BoostedColorPercent = 200,
                        OtherColorsPercent = 75
                    };
            }

            return new CardEffect();
        }
            
    }
}