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
            TileView tileView = boardView.TileAt(firstCell);
            if (tileView != null) 
                tileView.ShakeInvalid(eventPlayer.settings);
            return;
        }
        Play(new PlayerAction(ActionType.Swap, firstCell, secondCell));
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
        StartCoroutine(eventPlayer.PlayEvents(events));
    }
}