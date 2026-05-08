using UnityEngine;

public class CharacterExpression : MonoBehaviour
{
    public Sprite neutral;
    public Sprite surprised;
    public Sprite happy;
    public Sprite sad;
    private SpriteRenderer sr;

    void Start() => sr = GetComponent<SpriteRenderer>();

    public void SetExpression(string mood)
    {
        sr.sprite = mood switch
        {
            "surprised" => surprised,
            "happy" => happy,
            "sad" => sad,
            _ => neutral
        };
    }
}