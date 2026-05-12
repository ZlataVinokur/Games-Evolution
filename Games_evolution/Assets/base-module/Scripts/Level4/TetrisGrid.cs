using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TetrisGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 20;
    public GameObject blockPrefab;
    public Transform gridContainer;

    public System.Action OnPiecePlaced;
    public System.Action<int> OnRowCleared;
    public System.Action OnGameOver;

    private bool[,] grid;
    private Dictionary<Vector2Int, GameObject> blocksMap;

    private Color flashColor = Color.white;
    private float flashDuration = 0.2f;

    private void Awake()
    {
        grid = new bool[width, height];
        blocksMap = new Dictionary<Vector2Int, GameObject>();
    }

    public bool IsCellFree(int x, int y)
    {
        if (x < 0 || x >= width) return false;
        if (y < 0) return false;
        if (y >= height) return true;
        return !grid[x, y];
    }

    public void PlacePiece(TetrisPiece piece)
    {
        Color pieceColor = piece.GetColor();
        foreach (Vector2Int cell in piece.GetCells())
        {
            int x = piece.x + cell.x;
            int y = piece.y + cell.y;
            if (y >= 0 && y < height && x >= 0 && x < width)
            {
                grid[x, y] = true;
                Vector3 worldPos = GetWorldPosition(x, y);
                GameObject block = Instantiate(blockPrefab, worldPos, Quaternion.identity, gridContainer);
                SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = pieceColor;
                blocksMap[new Vector2Int(x, y)] = block;
            }
        }

        StartCoroutine(ClearRowsWithAnimation());
        StartCoroutine(CheckGameOverAfterAnimation());
        StartCoroutine(InvokePiecePlaced());
    }

    // Преобразует координаты сетки в мировые (с учётом позиции GameBoard)
    public Vector3 GetWorldPosition(int x, int y)
    {
        return transform.position + new Vector3(x + 0.5f, y + 0.5f, 0);
    }

    // Преобразует мировые координаты в координаты сетки (с учётом позиции GameBoard)
    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;
        int x = Mathf.RoundToInt(localPos.x - 0.5f);
        int y = Mathf.RoundToInt(localPos.y - 0.5f);
        return new Vector2Int(x, y);
    }

    private IEnumerator ClearRowsWithAnimation()
    {
        List<int> rowsToClear = new List<int>();
        for (int y = 0; y < height; y++)
            if (IsRowFull(y)) rowsToClear.Add(y);

        if (rowsToClear.Count == 0) yield break;

        foreach (int y in rowsToClear)
            for (int x = 0; x < width; x++)
            {
                Vector2Int key = new Vector2Int(x, y);
                if (blocksMap.ContainsKey(key))
                {
                    SpriteRenderer sr = blocksMap[key].GetComponent<SpriteRenderer>();
                    if (sr != null) StartCoroutine(FlashBlock(sr));
                }
            }

        yield return new WaitForSeconds(flashDuration);

        int rowsCleared = 0;
        foreach (int y in rowsToClear)
        {
            DeleteRow(y);
            rowsCleared++;
        }

        for (int i = rowsToClear.Count - 1; i >= 0; i--)
            ShiftRowsDown(rowsToClear[i] + 1, rowsToClear[i]);

        if (rowsCleared > 0)
            OnRowCleared?.Invoke(CalculateRowScore(rowsCleared));
    }

    private IEnumerator FlashBlock(SpriteRenderer sr)
    {
        Color originalColor = sr.color;
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    private IEnumerator CheckGameOverAfterAnimation()
    {
        yield return new WaitForSeconds(flashDuration + 0.05f);
        for (int x = 0; x < width; x++)
            if (grid[x, height - 1]) { OnGameOver?.Invoke(); break; }
    }

    private IEnumerator InvokePiecePlaced()
    {
        yield return new WaitForSeconds(flashDuration + 0.05f);
        OnPiecePlaced?.Invoke();
    }

    private bool IsRowFull(int y)
    {
        for (int x = 0; x < width; x++)
            if (!grid[x, y]) return false;
        return true;
    }

    private void DeleteRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            grid[x, y] = false;
            Vector2Int key = new Vector2Int(x, y);
            if (blocksMap.ContainsKey(key))
            {
                Destroy(blocksMap[key]);
                blocksMap.Remove(key);
            }
        }
    }

    private void ShiftRowsDown(int startY, int deletedY)
    {
        for (int y = startY; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y - 1] = grid[x, y];
                Vector2Int oldKey = new Vector2Int(x, y);
                Vector2Int newKey = new Vector2Int(x, y - 1);
                if (blocksMap.ContainsKey(oldKey))
                {
                    GameObject block = blocksMap[oldKey];
                    blocksMap.Remove(oldKey);
                    block.transform.position = GetWorldPosition(x, y - 1);
                    blocksMap[newKey] = block;
                }
            }
            for (int x = 0; x < width; x++)
                grid[x, height - 1] = false;
        }
    }

    private int CalculateRowScore(int rows)
    {
        switch (rows)
        {
            case 1: return 100;
            case 2: return 300;
            case 3: return 500;
            case 4: return 800;
            default: return 0;
        }
    }

    public void ResetGrid()
    {
        StopAllCoroutines();
        foreach (var block in blocksMap.Values) Destroy(block);
        blocksMap.Clear();
        grid = new bool[width, height];
    }
}