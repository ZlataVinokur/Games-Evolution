using UnityEngine;

public class PortalToNextLevel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RPGLevelManager.Instance?.CompleteLevel();
        }
    }
}