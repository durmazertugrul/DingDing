using DingDing.Core;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardInput : MonoBehaviour
{
    public EventPlayer eventPlayer;
    public BoardView boardView;
    public MatchController matchController;

    private Cell? pressedCell;
    private Vector2 pressPosition;
    private bool swapSent;


    public void OnTilePressed(TileView tileView, PointerEventData eventData)
    {
        pressedCell = null;
        swapSent = false;

        if (eventPlayer.IsPlaying) return;

        pressedCell = tileView.Cell;
        pressPosition = eventData.position;
    }

    public void OnTileDragged(PointerEventData eventData)
    {
        if (pressedCell == null || swapSent) return; 

        Vector2 dragVector = eventData.position - pressPosition;

        if (dragVector.magnitude < boardView.CellSize.x * 0.35f) return;

        Cell neighbourCell = NeighbourInDirection(pressedCell.Value, dragVector);

        swapSent = true;
        matchController.OnSwap(pressedCell.Value, neighbourCell);
        pressedCell = null;
    }

    public void OnTileReleased(PointerEventData eventData)
    {
        // A finger lifted after a swap must not count as a tap as well.

        if (pressedCell == null || swapSent) return;

        matchController.OnTap(pressedCell.Value);
        pressedCell= null;
    }

    private Cell NeighbourInDirection(Cell cell, Vector2 dragVector)
    {
        if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
        {
            if (dragVector.x > 0)
                return new Cell(cell.Row, cell.Column + 1);

            return new Cell(cell.Row, cell.Column - 1);
        }

        // Screen y grows upwards while board rows grow downwards.
        if (dragVector.y > 0)
            return new Cell(cell.Row - 1, cell.Column);

        return new Cell(cell.Row + 1, cell.Column);
    }


}
