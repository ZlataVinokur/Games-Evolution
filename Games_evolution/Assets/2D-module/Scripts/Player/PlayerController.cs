using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public abstract void HandleInput();
    public virtual void Move() { }
}