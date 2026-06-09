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
    public float minYDistance = 2.5f;
    public float maxYDistance = 4.5f;
    public float minX = -3.5f;
    public float maxX = 3.5f;
    public int initialPlatforms = 8;
    public float deleteBelowY = -10f;

    private List<GameObject> activePlatforms = new List<GameObject>();
    private float nextPlatformY;
    private GameObject lastGeneratedPlatform = null;
    private bool lastWasSpecial = false;

    [Header("Враги")]
    public GameObject[] enemyPrefabs;
    public float enemySpawnChance = 0.3f;

    [Header("Бонусы")]
    public GameObject[] bonusPrefabs;
    [Range(0f, 1f)]
    public float bonusSpawnChance = 0.2f;
    private float lastBonusY = -1000f;
    public float bonusCooldownY = 10f;

    [Header("Оружие")]
    public GameObject weaponPickupPrefab;
    private float lastWeaponSpawnY = -1000f;
    public float weaponSpawnIntervalY = 10f;
    private bool weaponAlreadyCollected = false; // станет true, когда игрок подберёт оружие

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.transform.position.y + 8f > nextPlatformY - 2f)
        {
            GeneratePlatform();
        }

        // Удаление платформ ниже границы
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
        // Выбор типа платформы
        GameObject platformPrefab = normalPlatform;
        float rand = Random.Range(0f, 1f);
        bool trySpecial = true;
        if (lastWasSpecial) trySpecial = false;

        if (trySpecial)
        {
            if (rand < 0.2f)
            {
                platformPrefab = bouncyPlatform;
                lastWasSpecial = true;
            }
            else if (rand < 0.35f)
            {
                platformPrefab = breakablePlatform;
                lastWasSpecial = true;
            }
            else
            {
                platformPrefab = normalPlatform;
                lastWasSpecial = false;
            }
        }
        else
        {
            platformPrefab = normalPlatform;
            lastWasSpecial = false;
        }

        float xPos = Random.Range(minX, maxX);
        float yPos = nextPlatformY;
        GameObject platform = Instantiate(platformPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
        activePlatforms.Add(platform);

        // Определяем, что будет на платформе (только один тип)
        // Приоритет: оружие (если ещё не подобрано) > бонус > враг
        bool hasSpecial = false;

        // 1. Оружие
        if (!weaponAlreadyCollected && weaponPickupPrefab != null &&
            (nextPlatformY - lastWeaponSpawnY) >= weaponSpawnIntervalY && !hasSpecial)
        {
            // Проверяем, нет ли уже чего-то на этой платформе (дети)
            // Можно просто спавнить, но чтобы не пересекалось, проверим коллизии
            Vector3 weaponPos = new Vector3(xPos, yPos + 0.8f, 0);
            // Небольшая проверка, нет ли в радиусе 0.5м других объектов (опционально)
            Collider2D[] colliders = Physics2D.OverlapCircleAll(weaponPos, 0.5f);
            bool occupied = false;
            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy") || col.CompareTag("HealthBonus") || col.CompareTag("ShieldBonus"))
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied)
            {
                Instantiate(weaponPickupPrefab, weaponPos, Quaternion.identity);
                lastWeaponSpawnY = nextPlatformY;
                hasSpecial = true;
            }
        }

        // 2. Бонус (если оружие не спавнилось)
        if (!hasSpecial && bonusPrefabs != null && bonusPrefabs.Length > 0 &&
            (nextPlatformY - lastBonusY) >= bonusCooldownY && Random.value < bonusSpawnChance)
        {
            Vector3 bonusPos = new Vector3(xPos, yPos + 0.7f, 0);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(bonusPos, 0.5f);
            bool occupied = false;
            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy") || col.CompareTag("WeaponPickup"))
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied)
            {
                GameObject bonusPrefab = bonusPrefabs[Random.Range(0, bonusPrefabs.Length)];
                Instantiate(bonusPrefab, bonusPos, Quaternion.identity);
                lastBonusY = nextPlatformY;
                hasSpecial = true;
            }
        }

        // 3. Враг (если ничего не спавнилось)
        if (!hasSpecial && enemyPrefabs != null && enemyPrefabs.Length > 0 && Random.value < enemySpawnChance)
        {
            Vector3 enemyPos = new Vector3(xPos, yPos + 0.5f, 0);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyPos, 0.5f);
            bool occupied = false;
            foreach (var col in colliders)
            {
                if (col.CompareTag("HealthBonus") || col.CompareTag("ShieldBonus") || col.CompareTag("WeaponPickup"))
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied)
            {
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
            }
        }

        nextPlatformY += Random.Range(minYDistance, maxYDistance);
    }

    // Вызывается из PixelPlatformerController при подборе оружия
    public void OnWeaponCollected()
    {
        weaponAlreadyCollected = true;
    }
}