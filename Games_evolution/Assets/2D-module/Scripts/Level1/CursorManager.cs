using UnityEngine;

public enum CursorType { Default, Look, Hand, Use }

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D lookCursor;
    [SerializeField] private Texture2D handCursor;
    [SerializeField] private Texture2D useCursor;

    void Awake()
    {
        Instance = this;
        ResetCursor();
    }

    public void SetCursor(CursorType type)
    {
        Texture2D tex = type switch
        {
            CursorType.Look => lookCursor,
            CursorType.Hand => handCursor,
            CursorType.Use => useCursor,
            _ => defaultCursor
        };
        Cursor.SetCursor(tex, Vector2.zero, CursorMode.Auto);
    }

    public void ResetCursor() => Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
}