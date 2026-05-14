using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private int rows = 3;
    [SerializeField] private int cols = 8;
    [SerializeField] private float startX = -5.5f;
    [SerializeField] private float startY = 3f;
    [SerializeField] private float spacingX = 1.6f;
    [SerializeField] private float spacingY = 0.6f;
    
    void Start()
    {
        BuildLevel();
    }
    
    void BuildLevel()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector2 position = new Vector2(
                    startX + col * spacingX,
                    startY + row * spacingY
                );
                
                GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity);
                
                // Разная прочность для разных рядов
                Brick brickScript = brick.GetComponent<Brick>();
                if (brickScript != null)
                {
                    // Нижний ряд - 1 удар, средний - 2, верхний - 3
                    brickScript.SetHealth(row + 1);
                }
            }
        }
    }
}