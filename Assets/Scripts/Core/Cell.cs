namespace DingDing.Core
{
    // Represents a board cell using its row and column indices,
    // with value equality, hash code generation, and string formatting.
    public struct Cell
    {
        public int Row { get; }
        public int Column { get; }

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override bool Equals(object other)
        {
            if (!(other is Cell))
            {
                return false;
            }

            Cell otherCell = (Cell)other;
            return Row == otherCell.Row && Column == otherCell.Column;
        }

        public override int GetHashCode()
        {
            return Row * Board.Columns + Column;
        }

        public override string ToString()
        {
            return "(" + Row + ", " + Column + ")";
        }
    }
}