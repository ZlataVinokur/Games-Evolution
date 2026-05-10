using UnityEngine;
using System.Collections;

 
public class Zipper : Enemy
    {
        [SerializeField] private float teleportInterval = 2f;
        private float timer;
        private PlatformGenerator generator;

        private void Start()
        {
            base.Start();
            generator = FindObjectOfType<PlatformGenerator>();
            timer = teleportInterval;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                TeleportToRandomPlatform();
                timer = teleportInterval;
            }
        }

        private void TeleportToRandomPlatform()
        {
            if (generator == null || generator.ActivePlatforms.Count == 0) return;
            Transform randomPlatform = generator.ActivePlatforms[Random.Range(0, generator.ActivePlatforms.Count)];
            transform.position = randomPlatform.position + Vector3.up * 1f; // над платформой
        }
    }
 