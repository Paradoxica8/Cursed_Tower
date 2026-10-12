using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerClickHandler
{
    private double maxHealth;
    private double currentHealth;
    private double goldReward;
    private bool isBoss;
    private bool isDead;

    public void Setup(double health, double gold, bool boss)
    {
        maxHealth = health;
        currentHealth = health;
        goldReward = gold;
        isBoss = boss;
        isDead = false;

        if (isBoss)
            transform.localScale = Vector3.one * 1.6f;

        if (TowerManager.Instance != null)
            TowerManager.Instance.RegisterEnemy(this);

        GameEvents.OnEnemyHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public bool IsBoss() => isBoss;
    public bool IsDead() => isDead;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDead) return;
        if (ClickManager.Instance == null) return;

        ClickManager.Instance.PerformClick(this);
    }

    public void TakeDamage(double damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        GameEvents.OnEnemyHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(goldReward);

        GameEvents.OnEnemyDied?.Invoke();

        Destroy(gameObject);
    }
}