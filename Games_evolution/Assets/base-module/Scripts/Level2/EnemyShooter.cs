using UnityEngine;

public class EnemyShooter : Enemy
{
    [Header("Стреляющий враг")]
    [SerializeField] private int burstCount = 3; // Количество пуль в очереди
    [SerializeField] private float burstDelay = 0.2f; // Задержка между пулями
    
    protected override void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            StartCoroutine(BurstShoot());
        }
    }
    
    private System.Collections.IEnumerator BurstShoot()
    {
        canShoot = false;
        
        for (int i = 0; i < burstCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.isEnemyBullet = true;
            }
            
            yield return new WaitForSeconds(burstDelay);
        }
        
        canShoot = true;
    }
}