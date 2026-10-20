using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DingDing.Core;
using DG.Tweening;

public class EventPlayer : MonoBehaviour
{
    public BoardView boardView;
    public AnimationSettings settings = new AnimationSettings();

    public bool IsPlaying {  get; private set; }

    public IEnumerator PlayEvents(List<object> events)
    {
        IsPlaying = true;

        foreach (var gameEvent in events)
        {
            if (gameEvent is PiecesSwapped swapped)
                yield return PlaySwap(swapped);
            else if (gameEvent is SpecialsActivated activated)
                yield return PlayActivations(activated);
            else if (gameEvent is CellsCleared cleared)
                yield return PlayClear(cleared);
            else if (gameEvent is SpecialCreated created)
                yield return PlaySpecialCreated(created);
            else if (gameEvent is PiecesFell fell)
                yield return PlayFall(fell);
            else if (gameEvent is PiecesSpawned spawned)
                yield return PlaySpawn(spawned);
        }

        IsPlaying = false;
    }


    private IEnumerator PlaySwap(PiecesSwapped swapped)
    {
        TileView firstTile = boardView.TileAt(swapped.FirstCell);
        TileView secondTile = boardView.TileAt(swapped.SecondCell);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(firstTile.MoveTo(boardView.PositionOf(swapped.SecondCell), settings.swapDuration, settings.swapEase));
        sequence.Join(secondTile.MoveTo(boardView.PositionOf(swapped.FirstCell), settings.swapDuration, settings.swapEase));

        yield return sequence.WaitForCompletion();

        boardView.SwapTiles(swapped.FirstCell, swapped.SecondCell);
    }

    private IEnumerator PlayClear(CellsCleared cleared)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (var cell in cleared.Cells)
        {
            TileView tileView = boardView.TileAt(cell);
            if (tileView == null) continue;

            Cell clearedCell = cell;
            Sequence pop = tileView.Pop(settings);
            pop.OnComplete(() => boardView.ReturnTileAt(clearedCell));
            sequence.Join(pop);
        }
        
        yield return sequence.WaitForCompletion();
    }

    private IEnumerator PlayFall(PiecesFell fell)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (var move in fell.Moves)
        {
            TileView tileView = boardView.TileAt(move.From);
            if (tileView == null) continue;

            int distance = move.To.Row - move.From.Row;
            float duration = distance * settings.fallDurationPerCell;

            sequence.Join(tileView.MoveTo(boardView.PositionOf(move.To), duration, settings.fallEase));
            boardView.MoveTileRecord(move.From, move.To);
        }

        yield return sequence.WaitForCompletion();
    }

    private IEnumerator PlaySpawn(PiecesSpawned spawned)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (var entry in spawned.Pieces)
        {
            TileView tileView = boardView.TakeTileForSpawn(entry.Cell, entry.Piece);

            Vector2 targetPosition = boardView.PositionOf(entry.Cell);
            tileView.SetPosition(targetPosition + new Vector2(0, boardView.BoardHeight));

            float duration = (entry.Cell.Row + 1) * settings.fallDurationPerCell;

            sequence.Join(tileView.MoveTo(targetPosition, duration, settings.fallEase));
        }
        yield return sequence.WaitForCompletion();
    }

    private IEnumerator PlaySpecialCreated(SpecialCreated created)
    {
        Vector2 targetPosition = boardView.PositionOf(created.Cell);
        Sequence mergeSequence = DOTween.Sequence();

        foreach (var sourceCell in created.SourceCells)
        { 
            TileView tileView = boardView.TileAt(sourceCell);
            if (tileView == null) continue;
            mergeSequence.Join(tileView.MoveTo(targetPosition, settings.specialMergeDuration, Ease.InQuad));
        }

        yield return mergeSequence.WaitForCompletion();

        foreach (var sourceCell in created.SourceCells)
        {
            boardView.ReturnTileAt(sourceCell);
        }

        TileView specialTile = boardView.PlaceSpecial(created.Cell, created.Kind);
        yield return specialTile.GrowFromZero(settings).WaitForCompletion();
    }

    private IEnumerator PlayActivations(SpecialsActivated activated)
    {
            foreach (var activation in activated.Activations)
            {
                Sequence sequence = DOTween.Sequence();
                float delay = 0;

                foreach (var cell in activation.ClearedCells)
                {
                    TileView tileView = boardView.TileAt(cell);
                    if (tileView == null) continue;

                    Cell clearedCell = cell;
                    Sequence pop = tileView.Pop(settings);
                    pop.OnComplete(() => boardView.ReturnTileAt(clearedCell));

                    sequence.Insert(delay, pop);
                    delay = delay + settings.blastStepDelay;
                }

                yield return sequence.WaitForCompletion();
            }

            
    }

}