using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

 
public class Level2Manager : MonoBehaviour
    {
        [SerializeField] private DialogueSystem dialogueSystem;
        [SerializeField] private EncyclopediaManager encyclopediaManager;
        [SerializeField] private Portal portal;
        [SerializeField] private Transform player;
        [SerializeField] private PlatformGenerator platformGenerator;
        [SerializeField] private float weaponSpawnHeight = 50f; // 1/3 высоты, примерно
        [SerializeField] private GameObject weaponPickupPrefab; // объект оружия на платформе
        [SerializeField] private GameObject portalPrefab;

        [Header("Kill Counter")]
        [SerializeField] private int killsRequired = 5;
        private int currentKills = 0;
        private bool portalActivated = false;
        private bool weaponSpawned = false;

        private void Start()
        {
            if (dialogueSystem == null) dialogueSystem = FindObjectOfType<DialogueSystem>();
            if (encyclopediaManager == null) encyclopediaManager = FindObjectOfType<EncyclopediaManager>();
            if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;

            // Стартовый диалог
            ShowDialogue("В платформерах всегда нужно двигаться вверх. Но здесь стены кусаются. Держись середины!", Speaker.Encyclopedia);
        }

        private void Update()
        {
            // Проверка высоты для появления оружия
            if (!weaponSpawned && player.position.y >= weaponSpawnHeight)
            {
                SpawnWeapon();
                weaponSpawned = true;
            }

            // Проверка диалогов на высоте (25%, 35%, 75%) - можно по player.position.y
            // Для простоты проверим по сравнению с максимальной возможной высотой, но у нас бесконечно.
            // Используем флаги.
        }

        public void RegisterKill(int points)
        {
            currentKills++;
            UpdateKillUI();
            if (currentKills == 1 && !weaponSpawned)
            {
                // Показываем диалог после первого убийства (предполагая, что оружие уже подобрано)
            }
            if (currentKills >= killsRequired && !portalActivated)
            {
                ActivatePortal();
            }
        }

        void ActivatePortal()
        {
            // Находим самую верхнюю активную платформу
            Transform topPlatform = null;
            float maxY = float.MinValue;
            foreach (Transform plat in platformGenerator.ActivePlatforms)
            {
                if (plat.position.y > maxY)
                {
                    maxY = plat.position.y;
                    topPlatform = plat;
                }
            }
            if (topPlatform != null)
            {
                Vector3 portalPos = topPlatform.position + Vector3.up * 1.5f;
                Instantiate(portalPrefab, portalPos, Quaternion.identity);
                portalActivated = true;
                ShowDialogue("Выход открыт! Доберись до портала наверху.", Speaker.Encyclopedia);
            }
        }

        void SpawnWeapon()
        {
            // Найти платформу примерно на высоте weaponSpawnHeight
            Transform platform = null;
            foreach (Transform plat in platformGenerator.ActivePlatforms)
            {
                if (Mathf.Abs(plat.position.y - weaponSpawnHeight) < 2f)
                {
                    platform = plat;
                    break;
                }
            }
            if (platform == null) // если нет подходящей, создадим на ближайшей выше
            {
                float closestDist = Mathf.Infinity;
                foreach (Transform plat in platformGenerator.ActivePlatforms)
                {
                    if (plat.position.y > weaponSpawnHeight && plat.position.y - weaponSpawnHeight < closestDist)
                    {
                        closestDist = plat.position.y - weaponSpawnHeight;
                        platform = plat;
                    }
                }
            }
            if (platform != null)
            {
                Instantiate(weaponPickupPrefab, platform.position + Vector3.up * 1.5f, Quaternion.identity);
                ShowDialogue("Видишь тех, кто мешает подъёму? Нажми пробел — и они станут пикселями. Попробуй!", Speaker.Encyclopedia);
            }
        }

        void ShowDialogue(string text, Speaker speaker)
        {
            dialogueSystem.ShowDialogue(new string[] { text }, speaker.ToString(), "neutral");
        }

        void UpdateKillUI()
            {
                // Реализовать UI счётчика убийств
            }

        public void RestartLevel()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level2_Platformer");
        }

        public void SkipLevel()
        {
            // Загружаем Level3_RPG, помечая уровень как пройденный? По условию - скип.
            // Можно установить флаг и загрузить
            GameManager.Instance.CompleteLevel("Level2");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level3_RPG");
        }
    }
 