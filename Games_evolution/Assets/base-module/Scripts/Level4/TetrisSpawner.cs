using UnityEngine;
using System.Collections.Generic;

public class TetrisSpawner : MonoBehaviour
{
    public TetrisGrid grid;
    public GameObject piecePrefab;
    public GameObject cellPrefab;
    public Transform spawnPoint;          // Двигайте этот объект в сцене – фигура будет появляться там
    public TetrisGameManager gameManager;
    private bool spawningEnabled = false;
    private List<List<Vector2Int>> shapes = new List<List<Vector2Int>>()
    {
        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0), new Vector2Int(3,0) },
        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },
        new List<Vector2Int> { new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(2,1) },
        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(2,1) },
        new List<Vector2Int> { new Vector2Int(2,0), new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(0,1) },
        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(2,1) },
        new List<Vector2Int> { new Vector2Int(2,0), new Vector2Int(2,1), new Vector2Int(1,1), new Vector2Int(0,1) }
    };

    private List<Color> shapeColors = new List<Color>()
    {
        new Color(0f, 1f, 1f), new Color(1f, 1f, 0f), new Color(0.8f, 0f, 0.8f),
        new Color(0f, 1f, 0f), new Color(1f, 0f, 0f), new Color(1f, 0.5f, 0f),
        new Color(0f, 0f, 1f)
    };

    private void Start()
    {
        if (grid != null) grid.OnPiecePlaced += SpawnNewPiece;
    }
    public void EnableSpawning()
    {
        spawningEnabled = true;
        SpawnNewPiece(); // первая фигура
    }
    private void OnDestroy()
    {
        if (grid != null) grid.OnPiecePlaced -= SpawnNewPiece;
    }

    public void SpawnNewPiece()
    {
        if (!spawningEnabled) return; 
        if (gameManager != null && gameManager.IsGameOver) return;

        int randomIndex = Random.Range(0, shapes.Count);
        List<Vector2Int> shape = shapes[randomIndex];
        Color pieceColor = shapeColors[randomIndex];

        // Преобразуем мировую позицию spawnPoint в координаты сетки
        Vector2Int spawnGridPos = grid.GetGridPosition(spawnPoint.position);

        // Проверяем возможность спавна
        if (!CanSpawn(spawnGridPos, shape))
        {
            Debug.Log($"Game Over! Невозможно спавнить фигуру в позиции {spawnGridPos}. Попробуйте подвинуть SpawnPoint.");
            if (gameManager != null) gameManager.GameOver();
            return;
        }

        // Создаём фигуру в мировых координатах spawnPoint
        GameObject pieceObj = Instantiate(piecePrefab, spawnPoint.position, Quaternion.identity);
        TetrisPiece piece = pieceObj.GetComponent<TetrisPiece>();
        piece.Initialize(grid, spawnGridPos, shape, cellPrefab, pieceColor);
    }

    private bool CanSpawn(Vector2Int pos, List<Vector2Int> shape)
    {
        foreach (Vector2Int cell in shape)
        {
            int cellX = pos.x + cell.x;
            int cellY = pos.y + cell.y;
            if (!grid.IsCellFree(cellX, cellY)) return false;
        }
        return true;
    }
}