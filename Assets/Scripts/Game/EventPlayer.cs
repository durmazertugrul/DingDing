using DG.Tweening;
using DingDing.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventPlayer : MonoBehaviour
{
    // Walks the event list the engine produced and turns every entry into
    // an animation, one after another, while input stays locked.

    public BoardView boardView;
    public FxPool fxPool;
    public ScreenShake screenShake;
    public HudView hudView;
    public AnimationSettings settings = new AnimationSettings();

    public Sprite starSprite;
    public Sprite shardSprite;
    public Sprite sparkSprite;
    public Sprite raySprite;
    public Sprite ringSprite;
    public Sprite shockwaveSprite;
    public Sprite trailSprite;
    public Sprite glowSprite;

    public Color goldColor = Color.yellow;

    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        DOTween.SetTweensCapacity(1000, 200);
    }
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

            else if (gameEvent is BoardShuffled shuffled)
                yield return PlayShuffle(shuffled);

            else if (gameEvent is OpponentDamaged damaged)
                  PlayDamage(damaged);
        }

        IsPlaying = false;
    }

    // The health bar runs alongside the pops instead of waiting for them.
    private void PlayDamage(OpponentDamaged damaged)
    {
        hudView.ApplyDamage(damaged.HealthAfter);
    }
        
    private IEnumerator PlayShuffle(BoardShuffled shuffled)
    {
        // The tiles gather in the middle, then the new layout spreads out.
        Vector2 centre = boardView.CentrePosition;

        Sequence gatherSequence = DOTween.Sequence();

        for (int row = 0; row < Board.Rows; row++)
        {
            for (int column = 0; column < Board.Columns; column++)
            {
                TileView tileView = boardView.TileAt(new Cell(row, column));
                if (tileView == null) continue;

                gatherSequence.Join(tileView.MoveTo(centre,
                    settings.shuffleGatherDuration, Ease.InQuad));
            }
        }

        yield return gatherSequence.WaitForCompletion();

        boardView.BuildFrom(shuffled.NewLayout);

        Sequence spreadSequence = DOTween.Sequence();

        for (int row = 0; row < Board.Rows; row++)
        {
            for (int column = 0; column < Board.Columns; column++)
            {
                Cell cell = new Cell(row, column);
                TileView tileView = boardView.TileAt(cell);
                if (tileView == null) continue;

                tileView.SetPosition(centre);
                spreadSequence.Join(tileView.MoveTo(boardView.PositionOf(cell),
                    settings.shuffleSpreadDuration, Ease.OutQuad));
            }
        }

        yield return spreadSequence.WaitForCompletion();
    }



    private IEnumerator PlaySwap(PiecesSwapped swapped)
    {
        TileView firstTile = boardView.TileAt(swapped.FirstCell);
        TileView secondTile = boardView.TileAt(swapped.SecondCell);

        float halfDuration = settings.swapDuration * 0.5f;

        Sequence sequence = DOTween.Sequence();
        sequence.Join(firstTile.MoveTo(boardView.PositionOf(swapped.SecondCell),
            settings.swapDuration, settings.swapEase));
        sequence.Join(secondTile.MoveTo(boardView.PositionOf(swapped.FirstCell),
            settings.swapDuration, settings.swapEase));

        // The dragged tile grows a little so it reads as the one being moved.
        sequence.Join(firstTile.RectTransform
            .DOScale(settings.dragGrowScale, halfDuration));
        sequence.Insert(halfDuration, firstTile.RectTransform
            .DOScale(1f, halfDuration));

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

            // The colour has to be read before the tile goes back to the pool.
            SpawnPopFx(cell, tileView.Piece);

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

            TileView landingTile = tileView;
            Tween fallTween = tileView.MoveTo(boardView.PositionOf(move.To),
                duration, settings.fallEase);
            fallTween.OnComplete(() => landingTile.LandSquash(settings));

            sequence.Join(fallTween);
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
            tileView.SetPosition(targetPosition
                + new Vector2(0f, boardView.BoardHeight));

            float duration = (entry.Cell.Row + 1) * settings.fallDurationPerCell;

            TileView landingTile = tileView;
            Tween fallTween = tileView.MoveTo(targetPosition, duration,
                settings.fallEase);
            fallTween.OnComplete(() => landingTile.LandSquash(settings));

            sequence.Join(fallTween);
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

            mergeSequence.Join(tileView.MoveTo(targetPosition,
                settings.specialMergeDuration, Ease.InQuad));
        }

        yield return mergeSequence.WaitForCompletion();

        foreach (var sourceCell in created.SourceCells)
        {
            boardView.ReturnTileAt(sourceCell);
        }

        // A soft glow behind the new piece while it grows.
        fxPool.PlayBurst(glowSprite, goldColor, targetPosition,boardView.CellSize.x * 2f, settings.specialGrowDuration, 1.4f);

        boardView.ReturnTileAt(created.Cell);
        TileView specialTile = boardView.PlaceSpecial(created.Cell, created.Kind);
        yield return specialTile.GrowFromZero(settings).WaitForCompletion();
    }

    private IEnumerator PlayActivations(SpecialsActivated activated)
    {
        // The chain plays in order: the medal first, then whatever it hit.
        foreach (var activation in activated.Activations)
        {
            yield return PlayActivation(activation);
        }
    }

    private IEnumerator PlayActivation(SpecialActivation activation)
    {
        Vector2 centre = boardView.PositionOf(activation.Cell);

        if (activation.Kind == PieceKind.Medal)
            yield return PlayLineBlast(activation, centre, true);
        else if (activation.Kind == PieceKind.Trophy)
            yield return PlayLineBlast(activation, centre, false);
        else if (activation.Kind == PieceKind.Belt)
            yield return PlayAreaBlast(activation, centre);
        else
            yield return PlayGongBlast(activation, centre);
    }

    private IEnumerator PlayLineBlast(SpecialActivation activation,
        Vector2 centre, bool horizontal)
    {
        float cellSize = boardView.CellSize.x;

        fxPool.PlayBurst(raySprite, goldColor, centre, cellSize * 2f,
            settings.burstDuration, settings.burstEndScale);

        // Two gold streaks leave the centre and run off the board.
        Vector2 awayVector = horizontal
            ? new Vector2(boardView.BoardWidth, 0f)
            : new Vector2(0f, -boardView.BoardHeight);

        fxPool.PlayTrail(trailSprite, goldColor, centre, centre + awayVector,
            cellSize, settings.trailDuration);
        fxPool.PlayTrail(trailSprite, goldColor, centre, centre - awayVector,
            cellSize, settings.trailDuration);

        screenShake.Shake(settings.shortShakeDuration, settings.shortShakeStrength);

        yield return PopCellsFromCentre(activation, activation.Cell,
            settings.trailStepDelay).WaitForCompletion();
    }

    private IEnumerator PlayAreaBlast(SpecialActivation activation, Vector2 centre)
    {
        fxPool.PlayBurst(shockwaveSprite, goldColor, centre,
            boardView.CellSize.x, settings.ringDuration, settings.ringEndScale);

        screenShake.Shake(settings.hardShakeDuration, settings.hardShakeStrength);

        yield return PopCellsFromCentre(activation, activation.Cell,
            settings.blastStepDelay).WaitForCompletion();
    }

    private IEnumerator PlayGongBlast(SpecialActivation activation, Vector2 centre)
    {
        float cellSize = boardView.CellSize.x;

        for (int waveIndex = 0; waveIndex < 3; waveIndex++)
        {
            fxPool.PlayBurst(ringSprite, goldColor, centre, cellSize,
                settings.ringDuration, settings.ringEndScale + waveIndex * 1.5f);

            yield return new WaitForSeconds(0.08f);
        }

        screenShake.Shake(settings.longShakeDuration, settings.longShakeStrength);

        yield return PopCellsFromCentre(activation, activation.Cell,
            settings.blastStepDelay).WaitForCompletion();
    }

    // Pops the cells of one blast outwards from its centre.
    private Sequence PopCellsFromCentre(SpecialActivation activation,
        Cell centreCell, float stepDelay)
    {
        Sequence sequence = DOTween.Sequence();

        List<Cell> cells = new List<Cell>(activation.ClearedCells);
        cells.Sort((firstCell, secondCell) =>
            DistanceBetween(centreCell, firstCell)
                .CompareTo(DistanceBetween(centreCell, secondCell)));

        float delay = 0f;

        foreach (var cell in cells)
        {
            TileView tileView = boardView.TileAt(cell);
            if (tileView == null) continue;

            SpawnPopFx(cell, tileView.Piece);

            Cell clearedCell = cell;
            Sequence pop = tileView.Pop(settings);
            pop.OnComplete(() => boardView.ReturnTileAt(clearedCell));

            sequence.Insert(delay, pop);
            delay = delay + stepDelay;
        }

        return sequence;
    }

    private static int DistanceBetween(Cell firstCell, Cell secondCell)
    {
        return Mathf.Abs(firstCell.Row - secondCell.Row)
             + Mathf.Abs(firstCell.Column - secondCell.Column);
    }

    private void SpawnPopFx(Cell cell, Piece piece)
    {
        Vector2 position = boardView.PositionOf(cell);
        Color color = boardView.ColorOf(piece);
        float size = boardView.CellSize.x;

        // A white flash in the middle, then coloured debris over it.
        fxPool.PlayBurst(starSprite, Color.white, position, size,
            settings.burstDuration, settings.burstEndScale);

        for (int index = 0; index < 6; index++)
        {
            float angle = index * 60f;
            Vector2 offset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)) * settings.shardDistance;

            fxPool.PlayFlyOut(shardSprite, color, position, offset,
                size * 0.4f, settings.shardDuration);
        }

        for (int index = 0; index < 5; index++)
        {
            Vector2 offset = Random.insideUnitCircle * settings.sparkDistance;

            fxPool.PlayFlyOut(sparkSprite, color, position, offset,
                size * 0.2f, settings.sparkDuration);
        }
    }
}