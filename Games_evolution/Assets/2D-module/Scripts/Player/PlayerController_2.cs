using UnityEngine;

public abstract class PlayerController_2 : MonoBehaviour
{
    public abstract void HandleInput();
    public virtual void Move() { }
}