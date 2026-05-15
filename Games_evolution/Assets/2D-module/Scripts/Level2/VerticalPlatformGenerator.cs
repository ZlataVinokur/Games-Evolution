using UnityEngine;
using System.Collections.Generic;

public class VerticalPlatformGenerator : MonoBehaviour
{
    [Header("Префабы платформ")]
    public GameObject normalPlatform;
    public GameObject bouncyPlatform;
    public GameObject breakablePlatform;

    [Header("Параметры генерации")]
    public float startY = -5f;
    public float minYDistance = 2f;
    public float maxYDistance = 4f;
    public float minX = -6f;
    public float maxX = 6f;
    public int initialPlatforms = 8;
    public float deleteBelowY = -8f;

    private List<GameObject> activePlatforms = new List<GameObject>();
    private float nextPlatformY;

    [Header("Враги")]
    public GameObject[] enemyPrefabs;      // массив префабов врагов (Jump, Fly, Teleport)
    public float enemySpawnChance = 0.3f;  // 30% шанс спавна врага на платформе

    void Start()
    {
        nextPlatformY = startY;
        for (int i = 0; i < initialPlatforms; i++)
        {
            GeneratePlatform();
        }
    }

    void Update()
    {
        // Генерируем новые платформы, если игрок поднялся достаточно высоко
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.transform.position.y + 8f > nextPlatformY - 2f)
        {
            GeneratePlatform();
        }

        // Удаляем платформы, которые стали слишком низко
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            if (activePlatforms[i] == null)
            {
                activePlatforms.RemoveAt(i);
                continue;
            }
            if (activePlatforms[i].transform.position.y < deleteBelowY)
            {
                Destroy(activePlatforms[i]);
                activePlatforms.RemoveAt(i);
            }
        }
    }

    void GeneratePlatform()
    {
        // Выбираем тип платформы с вероятностью
        GameObject platformPrefab = normalPlatform;
        float rand = Random.Range(0f, 1f);
        if (rand < 0.2f) platformPrefab = bouncyPlatform;
        else if (rand < 0.35f) platformPrefab = breakablePlatform;

        float xPos = Random.Range(minX, maxX);
        float yPos = nextPlatformY;
        GameObject platform = Instantiate(platformPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
        activePlatforms.Add(platform);

        // Спавн врага на платформе (с вероятностью)
        if (enemyPrefabs != null && enemyPrefabs.Length > 0 && Random.value < enemySpawnChance)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector3 enemyPos = new Vector3(xPos, yPos + 0.5f, 0); // чуть выше платформы
            Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
        }

        // Увеличиваем следующую позицию
        nextPlatformY += Random.Range(minYDistance, maxYDistance);
    }
}