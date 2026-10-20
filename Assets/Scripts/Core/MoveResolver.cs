using System;
using System.Collections.Generic;


namespace DingDing.Core
{
    // Resolves player actions by swapping pieces or activating specials,
    // detecting matches, creating special pieces, clearing cells, and processing
    // gravity, refills, and cascades while recording events and move results.
    public static class MoveResolver 
    {
        private class PendingSpecial
        {
            public PieceKind Kind { get; }
            public Cell SpawnCell { get; }
            public List<Cell> SourceCells { get; }

            public PendingSpecial(PieceKind kind, Cell spawnCell, List<Cell> sourceCell)
            {
                Kind = kind;
                SpawnCell = spawnCell;
                SourceCells = sourceCell;
            }
        }

        public static MoveResult ResolveAction(Board board, PlayerAction action, Random random, List<object> events)
        {
            MoveResult result = new MoveResult();

            if (action.Type == ActionType.Tap)
            {
                RunSpecialStep(board, new List<Cell> { action.FirstCell }, events, result);
                BoardGravity.DropAndRefill(board, random, events);
                RunMatchLoop(board, action, random, events, result, false);
                return result;
            }


            Piece firstPiece = board[action.FirstCell];
            Piece secondPiece = board[action.SecondCell];
            board[action.FirstCell] = secondPiece;
            board[action.SecondCell] = firstPiece;
            events.Add(new PiecesSwapped(action.FirstCell, action.SecondCell));

            Cell specialCell = default;
            bool hasSpecial = false;

            if (board[action.FirstCell].IsSpecial())
            {
                specialCell = action.FirstCell;
                hasSpecial = true;
            }
            else if (board[action.SecondCell].IsSpecial())
            {
                specialCell = action.SecondCell;
                hasSpecial = true;
            }

            if (hasSpecial)
            {
                RunSpecialStep(board, new List<Cell> { specialCell }, events, result);
                BoardGravity.DropAndRefill(board, random, events);
                RunMatchLoop(board, action, random, events, result, false);
            }
            else
            {
                RunMatchLoop(board, action, random, events, result, true);
            }

            return result;
        }

        private static void RunMatchLoop(Board board, PlayerAction action, Random random, List<object> events, MoveResult result, bool firstPassIsPlayerMatch)
        {
            bool isPlayerMatch = firstPassIsPlayerMatch;

            while (true)
            {
                List<MatchGroup> groups = MatchFinder.FindMatchGroups(board);

                if (groups.Count == 0)
                    break;
                if (!isPlayerMatch)
                    result.CascadeCount++;

                ClearStep step = new ClearStep();
                List<PendingSpecial> newSpecials = new List<PendingSpecial>();

                foreach (MatchGroup group in groups)
                {
                    PieceKind kind = MatchFinder.ClassifyShape(group);

                    if (isPlayerMatch && kind != PieceKind.Normal)
                    {
                        Cell spawnCell = SpawnCellOf(group, kind, action);
                        newSpecials.Add(new PendingSpecial(kind, spawnCell, group.Cells));
                    }

                    ClearCells(board, group.Cells, step, events);
                }



                foreach (var pending in newSpecials)
                {
                    board[pending.SpawnCell] = new Piece(PieceColor.None, pending.Kind);
                    result.CreatedSpecials.Add((pending.Kind, pending.SourceCells.Count));
                    events.Add(new SpecialCreated(pending.SpawnCell, pending.Kind, pending.SourceCells));
                }

                result.Steps.Add(step);
                BoardGravity.DropAndRefill(board, random, events);
                isPlayerMatch = false; //a cascade never creates a special piece
            }
        }

        private static void RunSpecialStep(Board board, List<Cell> startCells, List<object> events, MoveResult result)
        {
            var (activations, clearedCells) = SpecialActivator.ActivateSpecials(board, startCells);

            events.Add(new SpecialsActivated(activations));

            ClearStep step = new ClearStep();
            step.SpecialActivations = activations.Count;
            ClearCells(board, clearedCells, step, events);
            result.Steps.Add(step);
        }

        private static void ClearCells(Board board, IEnumerable<Cell> cells, ClearStep step, List<object> events)
        {
            List<Cell> clearedCells = new List<Cell>();

            foreach (var cell in cells)
            {
                Piece piece = board[cell];
                if (piece == null) continue;

                if (!piece.IsSpecial())
                    step.ClearedColors.Add(piece.Color);

                board[cell] = null;
                clearedCells.Add(cell);
            }

            events.Add(new CellsCleared(clearedCells));
        }

        private static Cell SpawnCellOf(MatchGroup group, PieceKind kind, PlayerAction action)
        {
            if (kind == PieceKind.Belt)
            {
                foreach (var cell in group.HorizontalRuns[0].Cells)
                {
                    if (group.VerticalRuns[0].Cells.Contains(cell))
                        return cell; // the corner of the L or T
                }
            }

            if (group.Cells.Contains(action.SecondCell))
                return action.SecondCell; // the dragged piece's cell

            return action.FirstCell; // the other piece's cell
        }


    }


}