using UnityEngine;

 
public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 0.33f; // 3 выстрела в секунду
        [SerializeField] private SpriteRenderer weaponSprite; // Отображается на персонаже

        private float nextFireTime = 0f;
        private PlatformerController player;

        private void Start()
        {
            player = GetComponentInParent<PlatformerController>();
        }

        public void TryFire()
        {
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }

        private void Fire()
        {
            if (bulletPrefab == null || firePoint == null || player == null) return;

            Vector2 direction = player.FacingRight ? Vector2.right : Vector2.left;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null) bulletScript.Init(direction);
        }

        // При подборе оружия устанавливаем его видимость
        public void Equip(bool show)
        {
            if (weaponSprite != null) weaponSprite.enabled = show;
        }
    }
 