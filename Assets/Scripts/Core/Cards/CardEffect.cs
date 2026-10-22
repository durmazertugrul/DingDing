namespace DingDing.Core.Cards
{
    public class CardEffect
    {
        public int MoveModifier = 0;

        public int DamagePercent = 100;          // all damage
        public int PiecePercent = 100;            // piece damage only
        public int SpecialBonusPercent = 100;     // the special-piece bonus only

        public PieceColor BoostedColor = PieceColor.None;
        public int BoostedColorPercent = 100;
        public int OtherColorsPercent = 100;

        public bool IsFinisher = false;
        public int FinisherBelowPercent = 100;
        public int FinisherAbovePercent = 100;

        public bool IsConditioning = false;
        public int FirstHalfPercent = 100;
        public int LastMovesPercent = 100;
        public int LastMovesCount = 0;

        public PieceKind StartingSpecial = PieceKind.Normal;   // Normal means none
    }
}