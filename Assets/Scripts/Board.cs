using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board Size")]
    public int width = 8;
    public int height = 8;

    [Header("Cell")]
    public Cell cellPrefab;

    public float spacing = 1.05f;

    public float Spacing => spacing;

    private Cell[,] cells;

    void Awake()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        cells = new Cell[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float offsetX = (width - 1) * spacing * 0.5f;
                float offsetY = (height - 1) * spacing * 0.5f;

                Vector3 position = new Vector3(
                    x * spacing - offsetX,
                    y * spacing - offsetY,
                    0
                );

                Cell cell = Instantiate(
                    cellPrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                cell.Initialize(x, y);

                cell.name = $"Cell ({x},{y})";

                cells[x, y] = cell;
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= width)
            return null;

        if (y < 0 || y >= height)
            return null;

        return cells[x, y];
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float offsetX = (width - 1) * spacing * 0.5f;
        float offsetY = (height - 1) * spacing * 0.5f;

        int x = Mathf.RoundToInt((worldPos.x + offsetX) / spacing);
        int y = Mathf.RoundToInt((worldPos.y + offsetY) / spacing);

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(int x, int y)
    {
        float offsetX = (width - 1) * spacing * 0.5f;
        float offsetY = (height - 1) * spacing * 0.5f;

        return new Vector3(
            x * spacing - offsetX,
            y * spacing - offsetY,
            0
        );
    }

    public bool CanPlace(Block block, Vector2Int origin)
    {
        foreach (Vector2Int cell in block.Cells)
        {
            int x = origin.x + cell.x;
            int y = origin.y + cell.y;

            if (x < 0 || x >= width)
                return false;

            if (y < 0 || y >= height)
                return false;

            if (cells[x, y].IsOccupied)
                return false;
        }

        return true;
    }

    public void Place(Block block, Vector2Int origin)
    {
        int i = 0;

        foreach (var offset in block.Cells)
        {
            int x = origin.x + offset.x;
            int y = origin.y + offset.y;

            Cell cell = cells[x, y];

            Transform cube = block.Cubes[i++];

            cube.SetParent(transform);

            cube.position = cell.transform.position;

            cell.SetCube(cube);
        }

        ClearRows();
        ClearColumns();
    }

    //preview
    private readonly List<Cell> previewCells = new();

    public void ShowPreview(Block block, Vector2Int origin)
    {
        ClearPreview();

        bool valid = CanPlace(block, origin);

        foreach (var offset in block.Cells)
        {
            int x = origin.x + offset.x;
            int y = origin.y + offset.y;

            if (x < 0 || x >= width || y < 0 || y >= height)
                continue;

            cells[x, y].SetPreview(valid);
            previewCells.Add(cells[x, y]);
        }
    }

    public void ClearPreview()
    {
        foreach (var cell in previewCells)
            cell.ClearPreview();

        previewCells.Clear();
    }

    public void Remove(Block block)
    {
        if (!block.IsPlaced)
            return;

        foreach (var cell in block.Cells)
        {
            cells[
                block.CurrentOrigin.x + cell.x,
                block.CurrentOrigin.y + cell.y
            ].ClearCube();
        }

        block.IsPlaced = false;
    }

    // Check for completed rows and columns
    void ClearRows()
    {
        for (int y = 0; y < height; y++)
        {
            bool full = true;

            for (int x = 0; x < width; x++)
            {
                if (!cells[x, y].IsOccupied)
                {
                    full = false;
                    break;
                }
            }

            if (!full)
                continue;

            for (int x = 0; x < width; x++)
            {
                cells[x, y].ClearCube();
            }
        }
    }

    void ClearColumns()
    {
        for (int x = 0; x < width; x++)
        {
            bool full = true;

            for (int y = 0; y < height; y++)
            {
                if (!cells[x, y].IsOccupied)
                {
                    full = false;
                    break;
                }
            }

            if (!full)
                continue;

            for (int y = 0; y < height; y++)
            {
                cells[x, y].ClearCube();
            }
        }
    }
}