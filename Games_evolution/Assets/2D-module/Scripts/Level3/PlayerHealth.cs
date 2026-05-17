using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject[] heartIcons;
    public GameObject gameManager; // ссылка на GameManager

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateUI();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        int hearts = Mathf.CeilToInt((float)this.currentHealth / this.maxHealth * heartIcons.Length);
        for (int i = 0; i < heartIcons.Length; i++)
            heartIcons[i].SetActive(i < hearts);
    }

    void Die()
    {
        // Вызов GameOver
        if (GameManager.Instance != null)
            GameManager.Instance.ShowGameOver();
        else
            Debug.Log("Игрок умер");
    }
}