using UnityEngine;

 
public class WallGuard : Enemy
    {
        [SerializeField] private float fireRate = 1.5f;
        [SerializeField] private GameObject enemyBulletPrefab;
        [SerializeField] private Transform firePoint;
        private float nextFire;

        private void Start()
        {
            base.Start();
            // Прикрепляемся к ближайшей стене (слева или справа)
            Transform wall = GameObject.FindGameObjectWithTag("Wall")?.transform;
            if (wall != null) transform.SetParent(wall);
        }

        private void Update()
        {
            if (Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate;
            }
        }

        private void Fire()
        {
            if (enemyBulletPrefab == null || firePoint == null) return;
            Vector2 direction = transform.position.x < 0 ? Vector2.right : Vector2.left;
            GameObject bulletObj = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
            EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();
            if (bullet != null) bullet.Init(direction);
        }

        private void OnDestroy()
        {
            if (transform.parent != null) transform.SetParent(null);
        }
    }
 