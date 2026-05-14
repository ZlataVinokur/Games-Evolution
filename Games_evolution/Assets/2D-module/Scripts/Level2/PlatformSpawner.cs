using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] platformPrefabs;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnAheadDistance = 10f;
    [SerializeField] private float spawnXMin = -2.5f;
    [SerializeField] private float spawnXMax = 2.5f;
    [SerializeField] private float minVerticalGap = 2.5f;
    [SerializeField] private float maxVerticalGap = 4f;
    [SerializeField] private float destroyBelowOffset = 10f;
    [Header("Health Pickups")]
    [SerializeField] private float pickupChance = 0.3f;       // 30% шанс
    [SerializeField] private string pickupObjectName = "HealthPickup"; // имя дочернего объекта

    private float highestY;
    private Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
        highestY = player.position.y + 5f;
        for (int i = 0; i < 10; i++)
            SpawnPlatformAt(highestY + i * 3f);
    }

    void Update()
    {
        if (player.position.y + spawnAheadDistance > highestY)
            SpawnNextPlatform();

        CleanupOldPlatforms();
    }

    void SpawnNextPlatform()
    {
        float y = highestY + Random.Range(minVerticalGap, maxVerticalGap);
        SpawnPlatformAt(y);
        highestY = y;
    }

    void SpawnPlatformAt(float y)
    {
        float x = Random.Range(spawnXMin, spawnXMax);
        int type = Random.Range(0, platformPrefabs.Length);
        GameObject plat = Instantiate(platformPrefabs[type], new Vector2(x, y), Quaternion.identity);

        // Попытка включить бонус
        if (Random.value < pickupChance)
        {
            Transform pickup = plat.transform.Find(pickupObjectName);
            if (pickup != null)
            {
                pickup.gameObject.SetActive(true);
            }
        }
    }

    void CleanupOldPlatforms()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
        float camY = mainCam.position.y;
        foreach (var p in platforms)
        {
            if (p.transform.position.y < camY - destroyBelowOffset)
                Destroy(p);
        }
    }
}