using UnityEngine;
using System.Collections.Generic;

public class TetrisPiece : MonoBehaviour
{
    public int x, y;
    public GameObject cellPrefab;

    private List<Vector2Int> cells;
    private TetrisGrid grid;
    private List<GameObject> visualBlocks = new List<GameObject>();
    private Color pieceColor;

    private float fallTimer = 0f;
    private float fallDelay = 0.5f;
    private bool controlsEnabled = true;
    public void Initialize(TetrisGrid tetrisGrid, Vector2Int spawnPos, List<Vector2Int> shape, GameObject prefab, Color color)
    {
        grid = tetrisGrid;
        x = spawnPos.x;
        y = spawnPos.y;
        cells = shape;
        cellPrefab = prefab;
        pieceColor = color;
        CreateVisual();
    }
    public void SetControlsEnabled(bool enabled)
    {
       controlsEnabled = enabled;
    }

    public List<Vector2Int> GetCells()
    {
        return cells;
    }

    public Color GetColor()
    {
        return pieceColor;
    }

    private void CreateVisual()
    {
        foreach (Vector2Int cell in cells)
        {
            Vector3 worldPos = grid.GetWorldPosition(x + cell.x, y + cell.y);
            GameObject block = Instantiate(cellPrefab, worldPos, Quaternion.identity, transform);
            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = pieceColor;
            visualBlocks.Add(block);
        }
    }

    private void UpdateVisual()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            Vector3 worldPos = grid.GetWorldPosition(x + cells[i].x, y + cells[i].y);
            visualBlocks[i].transform.position = worldPos;
        }
    }

    private void Update()
    {
        if (!controlsEnabled) return;
        if (grid == null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Move(-1, 0);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Move(1, 0);
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            Rotate();
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            Move(0, -1);
        else if (Input.GetKeyDown(KeyCode.Space))
            HardDrop();

        fallTimer += Time.deltaTime;
        if (fallTimer >= fallDelay)
        {
            fallTimer = 0f;
            Move(0, -1);
        }
    }

    private bool Move(int dx, int dy)
    {
        int newX = x + dx;
        int newY = y + dy;
        if (CanMove(newX, newY, cells))
        {
            x = newX;
            y = newY;
            UpdateVisual();
            return true;
        }
        else if (dy == -1)
        {
            grid.PlacePiece(this);
            Destroy(gameObject);
        }
        return false;
    }
    private void Rotate()
    {
        List<Vector2Int> rotated = new List<Vector2Int>();
        foreach (Vector2Int cell in cells)
            rotated.Add(new Vector2Int(cell.y, -cell.x));

        if (CanMove(x, y, rotated))
        {
            cells = rotated;
            foreach (var vb in visualBlocks) Destroy(vb);
            visualBlocks.Clear();
            CreateVisual();
        }
    }

    private void HardDrop()
    {
        while (Move(0, -1)) { }
    }

    private bool CanMove(int newX, int newY, List<Vector2Int> shape)
    {
        foreach (Vector2Int cell in shape)
        {
            int cellX = newX + cell.x;
            int cellY = newY + cell.y;
            if (!grid.IsCellFree(cellX, cellY))
                return false;
        }
        return true;
    }

    public void IncreaseFallSpeed(float multiplier)
    {
        fallDelay = Mathf.Max(0.1f, fallDelay * multiplier);
    }
}