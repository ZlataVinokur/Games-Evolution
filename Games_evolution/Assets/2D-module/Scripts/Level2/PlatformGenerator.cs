using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
    {
        [System.Serializable]
        public class PlatformType
        {
            public string name;
            public GameObject prefab;
            public float width; // необязательно, для отладки
            [Range(0f, 1f)] public float spawnChance = 0.33f;
        }

        [SerializeField] private List<PlatformType> platformTypes;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float generationHeightOffset = 8f; // генерируем на сколько выше игрока
        [SerializeField] private float minVerticalStep = 2.5f; // 150px ~ 2.5 units если pixels per unit = 100
        [SerializeField] private float maxVerticalStep = 4.5f; // ~250px
        [SerializeField] private float minX = -2.2f; // зависит от размера экрана и стен
        [SerializeField] private float maxX = 2.2f;
        [SerializeField] private int poolSize = 15;

        private float lastGeneratedY;
        private List<GameObject> activePlatforms = new List<GameObject>();
        private Queue<GameObject>[] pools; // по одному пулу на тип

        public List<Transform> ActivePlatformTransforms
        {
            get
            {
                List<Transform> list = new List<Transform>();
                foreach (GameObject plat in activePlatforms) list.Add(plat.transform);
                return list;
            }
        }
        public List<Transform> ActivePlatforms
        {
            get { return ActivePlatformTransforms; }
        }

        private void Start()
        {
            if (playerTransform == null)
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            InitializePools();
            lastGeneratedY = playerTransform.position.y;
            // Создаём начальные платформы под игроком
            GenerateInitialPlatforms();
        }

        private void Update()
        {
            // Если игрок поднялся выше lastGeneratedY на определённую величину, генерируем новые
            if (playerTransform.position.y + generationHeightOffset > lastGeneratedY)
            {
                GeneratePlatformAbove();
                // Чистим платформы ушедшие далеко вниз
                CleanupPlatformsBelow(playerTransform.position.y - 10f);
            }
        }

        void InitializePools()
        {
            pools = new Queue<GameObject>[platformTypes.Count];
            for (int i = 0; i < platformTypes.Count; i++)
            {
                pools[i] = new Queue<GameObject>();
                for (int j = 0; j < poolSize; j++)
                {
                    GameObject obj = Instantiate(platformTypes[i].prefab);
                    obj.SetActive(false);
                    pools[i].Enqueue(obj);
                }
            }
        }

        GameObject GetPlatformFromPool(int typeIndex)
        {
            if (pools[typeIndex].Count > 0)
            {
                GameObject obj = pools[typeIndex].Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(platformTypes[typeIndex].prefab);
                return obj;
            }
        }

        public void ReturnPlatformToPool(GameObject platform, int typeIndex)
        {
            platform.SetActive(false);
            pools[typeIndex].Enqueue(platform);
            activePlatforms.Remove(platform);
        }

        void GenerateInitialPlatforms()
        {
            float currentY = playerTransform.position.y - 3f;
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = new Vector2(Random.Range(minX, maxX), currentY);
                SpawnPlatformAt(pos);
                currentY += Random.Range(minVerticalStep, maxVerticalStep);
            }
            lastGeneratedY = currentY;
        }

        void GeneratePlatformAbove()
        {
            float newY = lastGeneratedY + Random.Range(minVerticalStep, maxVerticalStep);
            Vector2 pos = new Vector2(Random.Range(minX, maxX), newY);
            SpawnPlatformAt(pos);
            lastGeneratedY = newY;
        }

        void SpawnPlatformAt(Vector2 position)
        {
            int typeIndex = ChoosePlatformType();
            GameObject platform = GetPlatformFromPool(typeIndex);
            platform.transform.position = position;
            platform.transform.rotation = Quaternion.identity;
            activePlatforms.Add(platform);

            // Компонент для автоматического удаления
            Platform platformScript = platform.GetComponent<Platform>();
            if (platformScript != null)
            {
                platformScript.Init(this, typeIndex);
            }
        }

        int ChoosePlatformType()
        {
            float totalChance = 0;
            foreach (var t in platformTypes) totalChance += t.spawnChance;
            float rand = Random.value * totalChance;
            float cumulative = 0;
            for (int i = 0; i < platformTypes.Count; i++)
            {
                cumulative += platformTypes[i].spawnChance;
                if (rand <= cumulative) return i;
            }
            return 0;
        }

        void CleanupPlatformsBelow(float yThreshold)
        {
            for (int i = activePlatforms.Count - 1; i >= 0; i--)
            {
                if (activePlatforms[i].transform.position.y < yThreshold)
                {
                    // Находим тип и возвращаем в пул
                    Platform plat = activePlatforms[i].GetComponent<Platform>();
                    if (plat != null)
                    {
                        ReturnPlatformToPool(activePlatforms[i], plat.PlatformTypeIndex);
                    }
                }
            }
        }

        // Публичный метод для оповещения, что платформа покинута (для авто-исчезновения)
        public void OnPlatformLeft(GameObject platform)
        {
            // не делаем ничего, управление через Platform
        }
    }
