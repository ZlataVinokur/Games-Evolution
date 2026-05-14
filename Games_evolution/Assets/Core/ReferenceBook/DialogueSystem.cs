using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Obsolete("Используйте UnifiedInfoSystem вместо DialogueSystem")]
public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowDialogue(string[] lines, string speaker = "player", string emotion = "neutral") =>
        UnifiedInfoSystem.Instance?.ShowDialogue(lines, speaker, emotion);

    public bool IsDialogueActive() =>
        UnifiedInfoSystem.Instance?.IsShowingAnything() ?? false;
}