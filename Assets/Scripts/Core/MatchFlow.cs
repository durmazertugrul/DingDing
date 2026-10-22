using DingDing.Core.Cards;
using System.Collections.Generic;

namespace DingDing.Core
{
    // The only door the game and the simulator knock on. Everything about
    // rounds, the corner and the end of a match lives here.
    public static class MatchFlow
    {
        public static (Match Match, List<object> Events) StartMatch(
            LevelInfo levelInfo, int seed)
        {
            Match match = new Match(levelInfo, seed);
            match.Board = BoardInitializer.CreateStartingBoard(match.Random);
            match.CurrentRound = 1;

            List<object> events = new List<object>();
            StartRound(match, events);
            return (match, events);
        }

        public static List<object> PlayAction(Match match, PlayerAction action)
        {
            if (match.Phase != MatchPhase.Playing)
                return null;

            if (action.Type == ActionType.Swap
                && !SwapRules.IsValidSwap(match.Board, action.FirstCell,
                                           action.SecondCell))
                return null;

            if (action.Type == ActionType.Tap)
            {
                Piece piece = match.Board[action.FirstCell];
                if (piece == null || !piece.IsSpecial())
                    return null;
            }

            List<object> events = new List<object>();
            MoveResult result = MoveResolver.ResolveAction(match, action, events);

            match.Score = match.Score + ScoreCalculator.ScoreFrom(result);
            match.MovesLeftInRound = match.MovesLeftInRound - 1;

            if (match.OpponentHealth == 0)
            {
                FinishWithKnockout(match);
                return events;
            }

            if (match.MovesLeftInRound == 0)
                EndRound(match);

            if (match.Phase == MatchPhase.Playing)
                BoardShuffler.ShuffleIfStuck(match.Board, match.Random, events);

            return events;
        }

        public static List<object> SelectCornerCard(Match match, CardOffer offer)
        {
            if (match.Phase != MatchPhase.Corner)
                return null;

            CardRules.PickCard(match, offer);
            match.CurrentRound = match.CurrentRound + 1;

            List<object> events = new List<object>();
            StartRound(match, events);      // the board carries over untouched
            return events;
        }

        // A round is won when its damage covered its share of the health pool.
        // Written as a multiplication so there is no division to round.
        public static bool RoundWasWon(int damageThisRound, int startingHealth)
        {
            return damageThisRound * GameRules.RoundCount >= startingHealth;
        }

        private static void StartRound(Match match, List<object> events)
        {
            CardEffect effect = CardRules.ActiveEffect(match);

            int plannedMoves = match.Level.MovesPerRound[match.CurrentRound];
            match.RoundMoveCount = plannedMoves + effect.MoveModifier;
            match.MovesLeftInRound = match.RoundMoveCount;
            match.DamageThisRound = 0;

            if (effect.StartingSpecial != PieceKind.Normal)
            {
                Cell cell = InnerNormalCell(match);
                match.Board[cell] = new Piece(PieceColor.None,
                                               effect.StartingSpecial);
                events.Add(new SpecialCreated(cell, effect.StartingSpecial,
                                               new List<Cell>()));
            }

            BoardShuffler.ShuffleIfStuck(match.Board, match.Random, events);
            match.Phase = MatchPhase.Playing;
        }

        // Never the outer ring, so a line blast cannot be wasted along an edge.
        private static Cell InnerNormalCell(Match match)
        {
            for (int attempt = 1; attempt <= 100; attempt++)
            {
                int row = match.Random.Next(1, Board.Rows - 1);
                int column = match.Random.Next(1, Board.Columns - 1);

                Piece piece = match.Board[row, column];
                if (piece != null && !piece.IsSpecial())
                    return new Cell(row, column);
            }

            return new Cell(1, 1);
        }

        private static void EndRound(Match match)
        {
            int round = match.CurrentRound;
            match.RoundDamage[round] = match.DamageThisRound;

            int damageThisRound = match.DamageThisRound;
            int startingHealth = match.Level.StartingHealth;

            if (RoundWasWon(damageThisRound, startingHealth))
                match.RoundResults[round] = RoundResult.Won;
            else
                match.RoundResults[round] = RoundResult.Lost;

            if (round == GameRules.RoundCount)
            {
                match.Outcome = MatchOutcome.Loss;
                match.Phase = MatchPhase.Finished;
            }
            else
            {
                match.Phase = MatchPhase.Corner;
            }
        }

        private static void FinishWithKnockout(Match match)
        {
            int round = match.CurrentRound;
            match.RoundDamage[round] = match.DamageThisRound;
            match.RoundResults[round] = RoundResult.Knockout;

            // Counted after the move was spent, so it includes the last move.
            match.KnockoutMove = match.RoundMoveCount - match.MovesLeftInRound;

            match.Score = match.Score
                        + match.MovesLeftInRound * GameRules.PointsPerLeftoverMove;

            int roundsNotNeeded = GameRules.RoundCount - round;
            match.Score = match.Score
                        + roundsNotNeeded * GameRules.EarlyKnockoutBonusPerRound;

            match.Outcome = MatchOutcome.Knockout;
            match.Phase = MatchPhase.Finished;
        }
    }
}