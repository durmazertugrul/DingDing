using DingDing.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour
{
    // Creates 64 cell slots, works out each cell's position inside the board,
    // then walks the Board and puts a pooled tile on every occupied cell.

    public Sprite[] normalSprites; // indexed by PieceColor, element 0 unused
    public Sprite[] specialSprites; // indexed by PieceKind, element 0 unused

    public BoardInput boardInput;
    public Color[] normalFxColors;
    public Color specialFxColor; 
    public Vector2 CellSize { get; private set; }
    public GameObject cellPrefab;
    public TileView tilePrefab;
    public RectTransform grid; // holds the 64 cell slots and the layout group
    public RectTransform tilesParent; // holds the tiles, same rect as grid

    private TileView[,] tileViews = new TileView[Board.Rows, Board.Columns];
    private Vector2[,] cellPositions = new Vector2[Board.Rows, Board.Columns];

    // Destroying exploded tiles and instantiating replacements would create
    // and destroy dozens of objects during every cascade.
    private Queue<TileView> tilePool = new Queue<TileView>();

    // How far above the board a spawned tile starts.
    public float BoardHeight { get; private set; }
    public float BoardWidth { get; private set; }

    private void Awake()
    {
        BuildCells();
        CacheCellPositions();
    }

    private void BuildCells()
    {
        for (int index = 0; index < Board.Rows * Board.Columns; index++)
        {
            Instantiate(cellPrefab, grid, false);
        }
    }

    // The layout group holds the only copy of the cell size and spacing,
    // so the tile positions are worked out from its own settings.
    private void CacheCellPositions()
    {
        GridLayoutGroup layout = grid.GetComponent<GridLayoutGroup>();

        Vector2 cellSize = layout.cellSize;
        Vector2 spacing = layout.spacing;
        RectOffset padding = layout.padding;

        for (int row = 0; row < Board.Rows; row++)
        {
            for (int column = 0; column < Board.Columns; column++)
            {
                float x = padding.left + column * (cellSize.x + spacing.x) + cellSize.x * 0.5f;

                float y = padding.top + row * (cellSize.y + spacing.y) + cellSize.y * 0.5f;

                cellPositions[row, column] = new Vector2(x, -y);
            }
        }

        BoardHeight = padding.top + Board.Rows * cellSize.y + (Board.Rows - 1) * spacing.y;
        BoardWidth = padding.left + Board.Columns * cellSize.x + (Board.Columns - 1) * spacing.x;
        CellSize = cellSize;
    }

    // Where the tiles gather during a shuffle.
    public Vector2 CentrePosition
    {
        get { return new Vector2(BoardWidth * 0.5f, -BoardHeight * 0.5f); }
    }

    private Sprite SpriteFor(Piece piece)
    {
        if (piece.IsSpecial())
            return specialSprites[(int)piece.Kind];

        return normalSprites[(int)piece.Color];
    }

    public Vector2 PositionOf(Cell cell)
    {
        return cellPositions[cell.Row, cell.Column];
    }

    public TileView TileAt(Cell cell)
    {
        return tileViews[cell.Row, cell.Column];
    }

    public void BuildFrom(Board board)
    {
        ReturnAllTiles();

        for (int row = 0; row < Board.Rows; row++)
        {
            for (int column = 0; column < Board.Columns; column++)
            {
                Piece piece = board[row, column];
                if (piece == null) continue;

                Cell cell = new Cell(row, column);
                TileView tileView = TakeTileForSpawn(cell, piece);
                tileView.SetPosition(cellPositions[row, column]);
            }
        }
    }

    // Takes a tile from the pool, dresses it for this piece and records it.
    // The caller decides where it starts, so a spawned tile can drop in.
    public TileView TakeTileForSpawn(Cell cell, Piece piece)
    {
        TileView tileView = TakeFromPool();
        tileView.Cell = cell;
        tileView.SetPiece(piece, SpriteFor(piece));
        tileViews[cell.Row, cell.Column] = tileView;
        return tileView;
    }

    public TileView PlaceSpecial(Cell cell, PieceKind kind)
    {
        Piece piece = new Piece(PieceColor.None, kind);
        TileView tileView = TakeTileForSpawn(cell, piece);
        tileView.SetPosition(cellPositions[cell.Row, cell.Column]);
        return tileView;
    }

    public void SwapTiles(Cell firstCell, Cell secondCell)
    {
        TileView firstTile = tileViews[firstCell.Row, firstCell.Column];
        TileView secondTile = tileViews[secondCell.Row, secondCell.Column];

        tileViews[firstCell.Row, firstCell.Column] = secondTile;
        tileViews[secondCell.Row, secondCell.Column] = firstTile;

        if (firstTile != null) firstTile.Cell = secondCell;
        if (secondTile != null) secondTile.Cell = firstCell;
    }

    public void MoveTileRecord(Cell fromCell, Cell toCell)
    {
        TileView tileView = tileViews[fromCell.Row, fromCell.Column];

        tileViews[toCell.Row, toCell.Column] = tileView;
        tileViews[fromCell.Row, fromCell.Column] = null;

        if (tileView != null) tileView.Cell = toCell;
    }

    public void ReturnTileAt(Cell cell)
    {
        TileView tileView = tileViews[cell.Row, cell.Column];
        if (tileView == null) return;

        ReturnToPool(tileView);
        tileViews[cell.Row, cell.Column] = null;
    }

    private TileView TakeFromPool()
    {
        TileView tileView;

        if (tilePool.Count > 0)
            tileView = tilePool.Dequeue();
        else
            tileView = Instantiate(tilePrefab, tilesParent, false);

        tileView.boardInput = boardInput;
        tileView.gameObject.SetActive(true);
        return tileView;
    }

    public void ReturnToPool(TileView tileView)
    {
        // A tween still running would keep writing to this object after it is
        // handed out again, so it is killed before the tile goes back.
        tileView.ResetVisualState();
        tileView.gameObject.SetActive(false);
        tilePool.Enqueue(tileView);
    }

    private void ReturnAllTiles()
    {
        for (int row = 0; row < Board.Rows; row++)
        {
            for (int column = 0; column < Board.Columns; column++)
            {
                if (tileViews[row, column] != null)
                {
                    ReturnToPool(tileViews[row, column]);
                    tileViews[row, column] = null;
                }
            }
        }
    }

    // Effect sprites are white, so they take the colour of the piece that popped.
    public Color ColorOf(PieceColor color)
    {
        return normalFxColors[(int)color];
    }

    public Sprite SpriteOf(PieceColor color)
    {
        return normalSprites[(int)color];
    }
    public Color ColorOf(Piece piece)
    {
        if (piece.IsSpecial())
            return specialFxColor;

        return ColorOf(piece.Color);
    }

}