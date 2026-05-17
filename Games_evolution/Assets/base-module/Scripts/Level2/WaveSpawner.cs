using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public string waveName;
    public List<EnemySpawnInfo> enemiesToSpawn;
    public float timeBetweenSpawns = 1f;
    public bool isFinalWave = false;
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public Vector2 spawnPosition;
    public int count = 1;
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Настройки волн")]
    [SerializeField] private List<Wave> waves;
    [SerializeField] private float timeBetweenWaves = 3f;
    
    [Header("Границы спавна")]
    [SerializeField] private float leftBoundary = -6f;
    [SerializeField] private float rightBoundary = 6f;
    [SerializeField] private float topBoundary = 4f;
    private bool gameStarted = false;
    private int currentWaveIndex = 0;
    private int enemiesRemaining = 0;
    private bool isSpawning = false;
    private Level2Manager levelManager;
    private WaveUI waveUI;
    void Start()
    {
        waveUI = FindObjectOfType<WaveUI>();
        levelManager = GetComponent<Level2Manager>();
        if (levelManager == null)
        {
            levelManager = FindObjectOfType<Level2Manager>();
        }
        
        
    }
    public void StartGame()
    {
        gameStarted = true;
        StartWave();  // запускаем первую волну
    }
    public void StartWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            if (waveUI != null)
            {
                waveUI.UpdateWave(currentWaveIndex + 1, waves.Count);
            }
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
        else
        {
            if (levelManager != null)
            {
                levelManager.CompleteLevel();
            }
        }
    }
    
    private IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;
        
        int totalEnemies = 0;
        foreach (var enemyInfo in wave.enemiesToSpawn)
        {
            totalEnemies += enemyInfo.count;
        }
        enemiesRemaining = totalEnemies;
        
        Debug.Log($"Starting wave {currentWaveIndex + 1}: {wave.waveName} with {totalEnemies} enemies");
        
        foreach (var enemyInfo in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                Vector2 spawnPos = enemyInfo.spawnPosition;
                
                if (spawnPos == Vector2.zero)
                {
                    float randomX = Random.Range(leftBoundary, rightBoundary);
                    spawnPos = new Vector2(randomX, topBoundary);
                }
                
                Instantiate(enemyInfo.enemyPrefab, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(wave.timeBetweenSpawns);
            }
        }
        
        isSpawning = false;
    }
    
    public void OnEnemyDestroyed()
    {
        enemiesRemaining--;
        Debug.Log($"Enemies remaining: {enemiesRemaining}");
        
        if (enemiesRemaining <= 0 && !isSpawning)
        {
            currentWaveIndex++;
            StartCoroutine(NextWaveDelay());
        }
    }
    
    private IEnumerator NextWaveDelay()
    {
        Debug.Log($"Wave completed! Next wave in {timeBetweenWaves} seconds");
        yield return new WaitForSeconds(timeBetweenWaves);
     
        StartWave();
    }
    
    public void AddWave(Wave wave)
    {
        if (waves == null)
        {
            waves = new List<Wave>();
        }
        waves.Add(wave);
    }
}