using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;   // префабы врагов
    [SerializeField] private float spawnInterval = 3f;    // задержка между появлениями
    [SerializeField] private float spawnHeightOffset = 2f; // на сколько выше камеры спавнить
    [SerializeField] private float minX = -7f;
    [SerializeField] private float maxX = 7f;

    private Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        float x = Random.Range(minX, maxX);
        float y = mainCam.position.y + spawnHeightOffset;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(prefab, new Vector2(x, y), Quaternion.identity);
    }
}