namespace DingDing.Core
{
    // Represents the 8x8 game board, provides indexed access to pieces,
    // checks cell boundaries and occupancy, and creates a deep copy of the board.
    public class Board
    {
        public const int Rows = 8;
        public const int Columns = 8;

        private Piece[,] pieces = new Piece[Rows, Columns];

        public Piece this[int row, int column]
        {
            get { return pieces[row, column]; }
            set { pieces[row, column] = value; }
        }

        public Piece this[Cell cell]
        {
            get { return pieces[cell.Row, cell.Column]; }
            set { pieces[cell.Row, cell.Column] = value; }
        }

        public bool IsInside(int row, int column)
        {
            return row >= 0 && row < Rows && column >= 0 && column < Columns;
        }

        public bool IsEmpty(int row, int column)
        {
            return pieces[row, column] == null;
        }

        public Board Copy()
        {
            Board copy = new Board();

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Piece source = pieces[row, column];

                    if (source == null)
                    {
                        copy[row, column] = null;
                    }
                    else
                    {
                        copy[row, column] = new Piece(source.Color, source.Kind);
                    }
                }
            }

            return copy;
        }
    }
}