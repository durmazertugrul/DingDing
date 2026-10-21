using System.Collections.Generic;

namespace DingDing.Core
{
    //The game's system records every action taken. Unity translates these events into animations.
    public class PiecesSwapped
    {
        public Cell FirstCell { get; }
        public Cell SecondCell { get; }

        public PiecesSwapped(Cell firstCell, Cell secondCell)
        {
            FirstCell = firstCell;
            SecondCell = secondCell;
        }
    }
    public class BoardShuffled
    {
        public Board NewLayout { get; }

        public BoardShuffled(Board newLayout)
        {
            NewLayout = newLayout;
        }
    }


    public class SpecialsActivated
    {
        public List<SpecialActivation> Activations { get; }

        public SpecialsActivated(List<SpecialActivation> activations) 
        {
            Activations = activations;
        }

    }
    public class CellsCleared
    {
        public List<Cell> Cells { get; }
        public CellsCleared(List<Cell> cells)
        {
            Cells = cells;
        }
    }
    public class SpecialCreated
    {
        public Cell Cell { get; }
        public PieceKind Kind { get; }
        public List<Cell> SourceCells { get; } //the pieces that slid into the special

        public SpecialCreated(Cell cell, PieceKind kind, List<Cell> sourceCells)
        {
            Cell = cell;
            Kind = kind;
            SourceCells = sourceCells;
        }

    }
    public class PiecesFell
    {
        public List<(Cell From, Cell To)> Moves { get; }
        public PiecesFell(List<(Cell From, Cell To)> moves)
        {
            Moves = moves;
        }
    }

    public class PiecesSpawned
    {
        public List<(Cell Cell, Piece Piece)> Pieces { get; }

        public PiecesSpawned(List<(Cell Cell, Piece Piece)> pieces)
        {
            Pieces = pieces;
        }
    }
}