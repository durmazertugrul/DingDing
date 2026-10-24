using DG.Tweening;
using DingDing.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hands player actions to the engine, plays back whatever comes out, and
// updates the HUD once the animations are done. Holds no rules of its own.
public class MatchController : MonoBehaviour
{
    public BoardView boardView;
    public EventPlayer eventPlayer;
    public HudView hud;

    public int levelNumber = 1;
    public int seed = 0;            // zero means a new match every time

    private Match match;
    public CornerScreen cornerScreen;
    public MatchEndSequence matchEnd;

    private void Start()
    {
        LevelInfo level = LevelTable.GetLevel(levelNumber);
        int matchSeed = seed != 0 ? seed : Random.Range(1, int.MaxValue);

        var startup = MatchFlow.StartMatch(level, matchSeed);
        match = startup.Match;

        boardView.BuildFrom(match.Board);
        hud.Bind(match);

        StartCoroutine(PlayAndFollowUp(startup.Events));
    }

    public void OnSwap(Cell firstCell, Cell secondCell)
    {
        Play(new PlayerAction(ActionType.Swap, firstCell, secondCell));
    }

    public void OnTap(Cell cell)
    {
        Play(new PlayerAction(ActionType.Tap, cell, cell));
    }

    private void Play(PlayerAction action)
    {
        // A second move while the first one is still playing would pull the
        // board and the tiles apart.
        if (eventPlayer.IsPlaying)
            return;

        List<object> events = MatchFlow.PlayAction(match, action);

        if (events == null)
        {
            PlayRejected(action);
            return;
        }

        StartCoroutine(PlayAndFollowUp(events));
    }

    // The counters wait for the animations; otherwise the move counter would
    // drop while the pieces are still falling.
    private IEnumerator PlayAndFollowUp(List<object> events)
    {
        yield return eventPlayer.PlayEvents(events);

        if (match.Outcome == MatchOutcome.Knockout)
        {
            matchEnd.Begin(match);
            yield break;
        }

        hud.Refresh();

        if (match.Phase == MatchPhase.Corner) cornerScreen.Open(match);
        if (match.Phase == MatchPhase.Finished) matchEnd.Begin(match);
    }

    // The engine turned the action down; nothing changed on the board.
    private void PlayRejected(PlayerAction action)
    {
        if (action.Type == ActionType.Tap)
        {
            TileView tappedTile = boardView.TileAt(action.FirstCell);
            if (tappedTile != null)
                tappedTile.Wobble(eventPlayer.settings);
            return;
        }

        TileView firstTile = boardView.TileAt(action.FirstCell);
        TileView secondTile = boardView.TileAt(action.SecondCell);
        if (firstTile == null || secondTile == null)
            return;

        Vector2 firstPosition = boardView.PositionOf(action.FirstCell);
        Vector2 secondPosition = boardView.PositionOf(action.SecondCell);
        Vector2 halfway = (firstPosition + secondPosition) * 0.5f;
        float halfDuration = eventPlayer.settings.invalidSwapDuration * 0.5f;

        Sequence sequence = DOTween.Sequence();
        sequence.Join(firstTile.MoveTo(halfway, halfDuration, Ease.OutQuad));
        sequence.Join(secondTile.MoveTo(halfway, halfDuration, Ease.OutQuad));
        sequence.Append(firstTile.MoveTo(firstPosition, halfDuration, Ease.OutQuad));
        sequence.Join(secondTile.MoveTo(secondPosition, halfDuration, Ease.OutQuad));

        firstTile.Wobble(eventPlayer.settings);
    }
}