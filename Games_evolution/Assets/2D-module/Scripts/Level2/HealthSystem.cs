using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

 
public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 5;
        [SerializeField] private int currentHealth;
        [SerializeField] private Image heartPrefab; // Префаб спрайта сердечка
        [SerializeField] private Transform heartsContainer; // Панель для сердечек
        [SerializeField] private List<Image> hearts = new List<Image>();

        public delegate void OnDeath();
        public event OnDeath DeathEvent;

        public int CurrentHealth => currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
            InitializeHearts();
        }

        private void InitializeHearts()
        {
            // Создаём сердечки в UI
            for (int i = 0; i < maxHealth; i++)
            {
                Image heart = Instantiate(heartPrefab, heartsContainer);
                hearts.Add(heart);
                heart.enabled = true;
            }
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            if (currentHealth < 0) currentHealth = 0;
            UpdateHearts();
            if (currentHealth <= 0) DeathEvent?.Invoke();
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            UpdateHearts();
        }

        public void AddShield(bool active)
        {
            // Можно временно активировать щит (дополнительный скрипт на игроке)
            Shield shield = GetComponent<Shield>();
            if (shield == null) shield = gameObject.AddComponent<Shield>();
            shield.Activate();
        }

        private void UpdateHearts()
        {
            for (int i = 0; i < hearts.Count; i++)
            {
                hearts[i].enabled = i < currentHealth;
            }
        }
    }
 