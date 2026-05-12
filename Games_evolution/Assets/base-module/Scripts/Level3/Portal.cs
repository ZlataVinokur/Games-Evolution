using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    public Transform exitPortal;
    public float teleportCooldown = 0.3f;
    
    private bool isTeleporting = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;
        if (!other.CompareTag("Player") && !other.CompareTag("Ghost")) return;
        
        StartCoroutine(TeleportObject(other));
    }
    
    IEnumerator TeleportObject(Collider2D other)
    {
        isTeleporting = true;
        GetComponent<Collider2D>().enabled = false;
        
        if (other.CompareTag("Player"))
        {
            PlayerController3 player = other.GetComponent<PlayerController3>();
            if (player != null)
            {
                // ← Берём текущее направление игрока
                Vector2 currentDir = player.GetCurrentDirection();
                
                // Отключаем коллайдер игрока на время
                Collider2D playerCollider = other.GetComponent<Collider2D>();
                if (playerCollider != null) playerCollider.enabled = false;
                
                // Телепортируем с сохранением направления
                player.TeleportTo(exitPortal.position, currentDir);
                
                yield return new WaitForSeconds(0.2f);
                if (playerCollider != null) playerCollider.enabled = true;
            }
        }
        else if (other.CompareTag("Ghost"))
        {
            other.transform.position = exitPortal.position;
        }
        
        yield return new WaitForSeconds(teleportCooldown);
        GetComponent<Collider2D>().enabled = true;
        isTeleporting = false;
    }
}