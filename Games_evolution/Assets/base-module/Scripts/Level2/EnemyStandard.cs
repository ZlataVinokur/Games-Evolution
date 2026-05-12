using UnityEngine;

public class EnemyStandard : Enemy
{
    [Header("Стандартный враг")]
    [SerializeField] private Color enemyColor = Color.red;
    
    protected override void Start()
    {
        base.Start();
        
        // Устанавливаем цвет
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = enemyColor;
        }
    }
    
    // Стандартный враг не требует дополнительной логики
}
