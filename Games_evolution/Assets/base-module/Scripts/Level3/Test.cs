using UnityEngine;

public class GhostDebug : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ghost collision with: " + collision.gameObject.tag);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Ghost trigger with: " + other.tag);
    }
}