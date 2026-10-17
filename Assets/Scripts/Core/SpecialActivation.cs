using System.Collections.Generic;

namespace DingDing.Core 
{
    public class SpecialActivation
    {
        public Cell Cell { get; }
        public PieceKind Kind { get; }
        public List<Cell> ClearedCells { get; } //the cells this one blast cleared


        public SpecialActivation(Cell cell, PieceKind kind)
        {
            Cell = cell;
            Kind = kind;
            ClearedCells = new List<Cell>();
        }


    }
}
