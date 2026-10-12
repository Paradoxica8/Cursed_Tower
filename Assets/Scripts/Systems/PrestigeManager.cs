using System;
using UnityEngine;

public class PrestigeManager : MonoBehaviour
{
    public static PrestigeManager Instance { get; private set; }

    [Header("Currency")]
    private double currentAsh = 0;

    [Header("Talent Levels")]
    private int talentDamageLevel = 0;
    private int talentGoldLevel = 0;
    private int talentClickLevel = 0;

    public static Action OnAshChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public double GetAsh() => currentAsh;

    public double CalculateAshReward()
    {
        int floor = TowerManager.Instance != null ? TowerManager.Instance.GetCurrentFloor() : 1;
        if (floor < 15) return 0;

        double baseReward = Math.Pow(floor / 10.0, 1.5);
        return Math.Floor(baseReward);
    }

    public bool PerformPrestige()
    {
        double reward = CalculateAshReward();
        if (reward <= 0) return false;

        currentAsh += reward;
        OnAshChanged?.Invoke();

        if (GoldManager.Instance != null)
            GoldManager.Instance.ResetGold();

        if (ClickManager.Instance != null)
            ClickManager.Instance.ResetProgress();

        if (HeroManager.Instance != null)
            HeroManager.Instance.ResetAllHeroes();

        if (TowerManager.Instance != null)
            TowerManager.Instance.ResetToFloorOne();

        return true;
    }


    public double GetDamageMultiplier()
    {
        double ashBonus = 1.0 + (currentAsh * 0.10);
        double talentBonus = 1.0 + (talentDamageLevel * 0.25);
        return ashBonus * talentBonus;
    }

    public double GetGoldMultiplier()
    {
        return 1.0 + (talentGoldLevel * 0.20);
    }

    public double GetClickMultiplier()
    {
        return 1.0 + (talentClickLevel * 0.30);
    }

    public bool UpgradeDamageTalent()
    {
        double cost = GetTalentCost(talentDamageLevel);
        if (currentAsh >= cost)
        {
            currentAsh -= cost;
            talentDamageLevel++;
            OnAshChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool UpgradeGoldTalent()
    {
        double cost = GetTalentCost(talentGoldLevel);
        if (currentAsh >= cost)
        {
            currentAsh -= cost;
            talentGoldLevel++;
            OnAshChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool UpgradeClickTalent()
    {
        double cost = GetTalentCost(talentClickLevel);
        if (currentAsh >= cost)
        {
            currentAsh -= cost;
            talentClickLevel++;
            OnAshChanged?.Invoke();
            return true;
        }
        return false;
    }

    public double GetTalentCost(int currentLevel)
    {
        return Math.Floor(1 + (currentLevel * 1.5));
    }

    public void ResetAllPrestige()
    {
        currentAsh = 0;
        talentDamageLevel = 0;
        talentGoldLevel = 0;
        talentClickLevel = 0;
        OnAshChanged?.Invoke();
    }

    public void SetDataFromSave(double ash, int damageLvl, int goldLvl, int clickLvl)
    {
        currentAsh = ash;
        talentDamageLevel = damageLvl;
        talentGoldLevel = goldLvl;
        talentClickLevel = clickLvl;

        OnAshChanged?.Invoke();
    }

    public int GetDamageTalentLevel() => talentDamageLevel;
    public int GetGoldTalentLevel() => talentGoldLevel;
    public int GetClickTalentLevel() => talentClickLevel;
}