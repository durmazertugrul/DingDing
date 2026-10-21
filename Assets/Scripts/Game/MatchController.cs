using DG.Tweening;
using DingDing.Core;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    public BoardView boardView;
    public EventPlayer eventPlayer;
    public int seed = 42;

    private Board board;
    private System.Random random;


    private void Start()
    {
        random = new System.Random(seed);
        board = BoardInitializer.CreateStartingBoard(random);
        boardView.BuildFrom(board);
    }

    public void OnSwap(Cell firstCell, Cell secondCell)
    {
        if (!SwapRules.IsValidSwap(board, firstCell, secondCell))
        {
            PlayInvalidSwap(firstCell, secondCell);
            return;
        }

        Play(new PlayerAction(ActionType.Swap, firstCell, secondCell));
    }

    private void PlayInvalidSwap(Cell firstCell, Cell secondCell)
    {
        TileView firstTile = boardView.TileAt(firstCell);
        TileView secondTile = boardView.TileAt(secondCell);
        if (firstTile == null || secondTile == null)
            return;

        Vector2 firstPosition = boardView.PositionOf(firstCell);
        Vector2 secondPosition = boardView.PositionOf(secondCell);
        Vector2 halfway = (firstPosition + secondPosition) * 0.5f;
        float halfDuration = eventPlayer.settings.invalidSwapDuration * 0.5f;

        Sequence sequence = DOTween.Sequence();
        sequence.Join(firstTile.MoveTo(halfway, halfDuration, Ease.OutQuad));
        sequence.Join(secondTile.MoveTo(halfway, halfDuration, Ease.OutQuad));
        sequence.Append(firstTile.MoveTo(firstPosition, halfDuration, Ease.OutQuad));
        sequence.Join(secondTile.MoveTo(secondPosition, halfDuration, Ease.OutQuad));

        firstTile.Wobble(eventPlayer.settings);
    }



    public void OnTap(Cell cell)
    {
        Piece piece = board[cell];
        if (piece == null)
            return;

        if (!piece.IsSpecial())
        {
            TileView tileView = boardView.TileAt(cell);
            if (tileView != null)
                tileView.Wobble(eventPlayer.settings);
            return;
        }
        Play(new PlayerAction(ActionType.Tap, cell, cell));
    }

    private void Play(PlayerAction action)
    {
        List<object> events = new List<object>();
        MoveResolver.ResolveAction(board, action, random, events);
        BoardShuffler.ShuffleIfStuck(board, random, events);
        StartCoroutine(eventPlayer.PlayEvents(events));
    }
}