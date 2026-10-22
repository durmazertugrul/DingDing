using DingDing.Core.Cards;

namespace DingDing.Core
{
    // Score for one whole move. Unlike damage, the timing of score does not
    // change anything, so there is no need to break it into steps.
    public static class ScoreCalculator
    {
        public static int ScoreFrom(MoveResult result) 
        {
            int score = 0;

            foreach (var step in result.Steps)
            {
                score = score + step.ClearedColors.Count * GameRules.PointsPerPiece;
                score = score + step.SpecialActivations * GameRules.PointsPerActivation;
            }
                

            foreach (var createdSpecial in result.CreatedSpecials)
            {
                int pieceCount = createdSpecial.GroupSize;
                score = score + pieceCount * GameRules.PointsPerCreatedSpecialPiece;
            }


            return score;
        }
    }
}