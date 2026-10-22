using DingDing.Core.Cards;
using System;

namespace DingDing.Core
{
    // Damage for one clear step. A move with cascades produces several steps
    // and each one is applied on its own, so a knockout lands on the right step.
    public static class DamageCalculator
    {
        public static int StepDamage(Match match, ClearStep step, CardEffect effect)
        {
            decimal pieceDamage = 0m;

            foreach (var color in step.ClearedColors)
            {
                int colorPercent = CardRules.ColorPercent(effect, color);
                pieceDamage = pieceDamage + GameRules.DamagePerPiece * colorPercent / 100m;
            }

            pieceDamage = pieceDamage * effect.PiecePercent / 100m;

            decimal specialBonus = step.SpecialActivations * GameRules.SpecialBonusDamage * effect.SpecialBonusPercent / 100m;

            decimal total = pieceDamage + specialBonus;

            foreach (var percent in CardRules.DamagePercents(effect, match))
            {
                total = total * percent / 100m;
            }

            return (int)Math.Round(total, MidpointRounding.AwayFromZero);

        }
    }
}