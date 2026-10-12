using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    private double currentGold = 0;

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
        GameEvents.OnGoldChanged?.Invoke(currentGold);
    }

    public double GetGold() => currentGold;

    public void AddGold(double amount)
    {
        if (amount <= 0) return;

        double heroGoldMult = HeroManager.Instance != null ? HeroManager.Instance.GetTotalGoldMultiplier() : 1.0;

        double prestigeGoldMult = PrestigeManager.Instance != null ? PrestigeManager.Instance.GetGoldMultiplier() : 1.0;

        currentGold += amount * heroGoldMult * prestigeGoldMult;
        GameEvents.OnGoldChanged?.Invoke(currentGold);
    }

    public bool CanAfford(double amount)
    {
        return currentGold >= amount;
    }

    public bool SpendGold(double amount)
    {
        if (!CanAfford(amount)) return false;

        currentGold -= amount;
        GameEvents.OnGoldChanged?.Invoke(currentGold);
        return true;
    }

    public void ResetGold()
    {
        currentGold = 0;
        GameEvents.OnGoldChanged?.Invoke(currentGold);
    }
}