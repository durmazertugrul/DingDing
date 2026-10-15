namespace DingDing.Core 
{
    public class Piece
    {
        public PieceColor Color { get; }
        public PieceKind Kind { get; }

        public Piece(PieceColor color, PieceKind kind)
        {
            Color = color;
            Kind = kind;
        }

        public bool IsSpecial()
        {
            return Kind != PieceKind.Normal;
        }

    }
}
