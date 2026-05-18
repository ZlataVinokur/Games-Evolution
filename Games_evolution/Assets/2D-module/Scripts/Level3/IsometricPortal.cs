using UnityEngine;

public class IsometricPortal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RPGLevelManager.Instance.OnBossDefeated();
        }
    }
}