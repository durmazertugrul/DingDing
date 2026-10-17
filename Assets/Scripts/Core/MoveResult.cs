using System.Collections.Generic;

namespace DingDing.Core
{
    public class MoveResult
    {
        public List<ClearStep> Steps { get; }
        public List<(PieceKind Kind, int GroupSize)> CreatedSpecials { get; }
        public int CascadeCount { get; set; }


        public MoveResult()
        {
            Steps = new List<ClearStep>();
            CreatedSpecials = new List<(PieceKind Kind, int GroupSize)>();
            CascadeCount = 0;
        }
    }
}