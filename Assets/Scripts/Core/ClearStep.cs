using System.Collections.Generic;

namespace DingDing.Core
{
    public class ClearStep
    {
        public List<PieceColor> ClearedColors { get; } //one color per normal piece cleared
        public int SpecialActivations { get; set; } //how many special pieces blasted

        public ClearStep()
        {
            ClearedColors = new List<PieceColor>();
            SpecialActivations = 0;
        }
    }
}