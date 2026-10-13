using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public static ClickManager Instance { get; private set; }

    [Header("Base Stats")]
    [SerializeField] private double baseClickDamage = 1;
    [SerializeField] private double damageGrowthPerLevel = 1.15;

    [Header("Upgrade Cost")]
    [SerializeField] private double baseUpgradeCost = 10;
    [SerializeField] private double costGrowth = 1.15;

    [Header("Critical")]
    [SerializeField] private float critChance = 0f;
    [SerializeField] private float critMultiplier = 2f;

    private int clickLevel = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameEvents.OnClickDamageChanged?.Invoke(GetClickDamage());
    }

    public double GetClickDamage()
    {
        double dmg = baseClickDamage * System.Math.Pow(damageGrowthPerLevel, clickLevel);

        if (PrestigeManager.Instance != null)
        {
            // Умножаем только на "Мощный Клик" (+30% за уровень)
            dmg *= PrestigeManager.Instance.GetClickMultiplier();

            // Умножаем на базовый бонус Пепла (+10% за каждую единицу Пепла)
            double ashBonus = 1.0 + (PrestigeManager.Instance.GetAsh() * 0.10);
            dmg *= ashBonus;
        }

        return dmg;
    }

    public int GetClickLevel() => clickLevel;

    public void SetClickLevel(int level)
    {
        clickLevel = level;
        GameEvents.OnClickDamageChanged?.Invoke(GetClickDamage());
    }

    public double GetUpgradeCost()
    {
        return System.Math.Floor(baseUpgradeCost * System.Math.Pow(costGrowth, clickLevel));
    }

    public bool UpgradeClick()
    {
        double cost = GetUpgradeCost();
        if (!GoldManager.Instance.SpendGold(cost)) return false;

        clickLevel++;
        GameEvents.OnClickDamageChanged?.Invoke(GetClickDamage());
        return true;
    }

    public void PerformClick(Enemy enemy)
    {
        if (enemy == null || enemy.IsDead()) return;

        double damage = GetClickDamage();
        enemy.TakeDamage(damage);
    }

    public void ResetProgress()
    {
        clickLevel = 0;
        GameEvents.OnClickDamageChanged?.Invoke(GetClickDamage());
    }
}