using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class AutoDotPlacer : EditorWindow
{
    private GameObject pelletPrefab;
    private GameObject superPelletPrefab;
    private Tilemap mazeTilemap;

    [MenuItem("Tools/Auto Dot Placer")]
    public static void ShowWindow()
    {
        GetWindow<AutoDotPlacer>("Auto Dot Placer");
    }

    void OnGUI()
    {
        GUILayout.Label("Автоматическая расстановка точек", EditorStyles.boldLabel);
        
        pelletPrefab = (GameObject)EditorGUILayout.ObjectField("Pellet Prefab", pelletPrefab, typeof(GameObject), false);
        superPelletPrefab = (GameObject)EditorGUILayout.ObjectField("Super Pellet Prefab", superPelletPrefab, typeof(GameObject), false);
        mazeTilemap = (Tilemap)EditorGUILayout.ObjectField("Maze Tilemap", mazeTilemap, typeof(Tilemap), true);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Расставить точки"))
        {
            PlaceDots();
        }
        
        if (GUILayout.Button("Очистить все точки"))
        {
            ClearDots();
        }
    }

    void PlaceDots()
    {
        if (pelletPrefab == null)
        {
            Debug.LogError("Укажи Pellet Prefab!");
            return;
        }
        
        if (mazeTilemap == null)
        {
            Debug.LogError("Укажи Maze Tilemap!");
            return;
        }
        
        ClearDots();
        
        GameObject dotsHolder = new GameObject("DotsHolder");
        
        BoundsInt bounds = mazeTilemap.cellBounds;
        
        // Находим все угловые позиции для супер-точек
        List<Vector3Int> corners = GetCornerPositions(bounds);
        
        int dotCount = 0;
        int superDotCount = 0;
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase tile = mazeTilemap.GetTile(cellPos);
                
                // Если в клетке НЕТ стены (пустое место)
                if (tile == null)
                {
                    Vector3 worldPos = mazeTilemap.CellToWorld(cellPos);
                    worldPos += new Vector3(0.5f, 0.5f, 0); // Центр клетки
                    
                    GameObject dotToPlace;
                    
                    // Проверяем, угол ли это
                    if (corners.Contains(cellPos) && superPelletPrefab != null)
                    {
                        dotToPlace = superPelletPrefab;
                        superDotCount++;
                    }
                    else
                    {
                        dotToPlace = pelletPrefab;
                        dotCount++;
                    }
                    
                    GameObject dot = PrefabUtility.InstantiatePrefab(dotToPlace) as GameObject;
                    dot.transform.position = worldPos;
                    dot.transform.parent = dotsHolder.transform;
                }
            }
        }
        
        Debug.Log($"Расставлено точек: {dotCount}, супер-точек: {superDotCount}");
    }
    
    List<Vector3Int> GetCornerPositions(BoundsInt bounds)
    {
        List<Vector3Int> corners = new List<Vector3Int>();
        
        // Четыре угла области
        corners.Add(new Vector3Int(bounds.xMin, bounds.yMin, 0));      // нижний левый
        corners.Add(new Vector3Int(bounds.xMax - 1, bounds.yMin, 0));  // нижний правый
        corners.Add(new Vector3Int(bounds.xMin, bounds.yMax - 1, 0));  // верхний левый
        corners.Add(new Vector3Int(bounds.xMax - 1, bounds.yMax - 1, 0)); // верхний правый
        
        return corners;
    }
    
    void ClearDots()
    {
        GameObject existingHolder = GameObject.Find("DotsHolder");
        if (existingHolder != null)
        {
            DestroyImmediate(existingHolder);
        }
    }
}