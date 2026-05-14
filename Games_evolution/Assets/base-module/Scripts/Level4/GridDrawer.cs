using UnityEngine;

[RequireComponent(typeof(TetrisGrid))]
public class TetrisGridLines : MonoBehaviour
{
    [Header("Внешний вид сетки")]
    public Color lineColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    public float lineWidth = 0.05f;

    private TetrisGrid grid;
    private int width, height;

    void Start()
    {
        grid = GetComponent<TetrisGrid>();
        width = grid.width;
        height = grid.height;
        DrawGrid();
    }

    void DrawGrid()
    {
        // Родительский объект для линий (чтобы не засорять иерархию)
        Transform linesParent = new GameObject("GridLines").transform;
        linesParent.SetParent(transform);
        linesParent.localPosition = Vector3.zero;

        // Вертикальные линии (между столбцами и по краям)
        for (int x = 0; x <= width; x++)
        {
            GameObject lineObj = new GameObject($"VLine_{x}");
            lineObj.transform.SetParent(linesParent);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.positionCount = 2;

            // Вычисляем мировые координаты границы x (без смещения 0.5)
            Vector3 start = transform.position + new Vector3(x, 0, 0);
            Vector3 end   = transform.position + new Vector3(x, height, 0);
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }

        // Горизонтальные линии (между рядами и по краям)
        for (int y = 0; y <= height; y++)
        {
            GameObject lineObj = new GameObject($"HLine_{y}");
            lineObj.transform.SetParent(linesParent);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.positionCount = 2;

            Vector3 start = transform.position + new Vector3(0, y, 0);
            Vector3 end   = transform.position + new Vector3(width, y, 0);
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
    }
}