using System.Collections.Generic;
using UnityEngine;

public enum BuyMode { x1, x10, x100, MAX }

public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance { get; private set; }

    [Header("Hero Data List")]
    [SerializeField] private List<HeroData> availableHeroData;

    [Header("World Hero Spawn")]
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private Transform heroesContainer;

    [Header("World Positioning")]
    [SerializeField] private Vector3 startPosition = new Vector3(-5.5f, -1.5f, 0f);
    [SerializeField] private float spacingX = 1.4f;

    private List<Hero> heroes = new List<Hero>();
    public BuyMode CurrentBuyMode { get; private set; } = BuyMode.x1;

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
        InitializeHeroes();
    }

    private void Update()
    {
        GameEvents.OnTotalDPSChanged?.Invoke(GetTotalDPS());
    }

    private void InitializeHeroes()
    {
        heroes.Clear();

        if (availableHeroData == null || availableHeroData.Count == 0) return;

        for (int i = 0; i < availableHeroData.Count; i++)
        {
            GameObject obj = Instantiate(heroPrefab, heroesContainer);
            obj.name = "Hero_" + availableHeroData[i].heroName;

            obj.transform.localPosition = startPosition + new Vector3(i * spacingX, 0f, 0f);

            Hero hero = obj.GetComponent<Hero>();
            if (hero != null)
            {
                hero.Init(availableHeroData[i]);
                heroes.Add(hero);
            }
        }

        GameEvents.OnHeroesChanged?.Invoke();
    }

    public List<Hero> GetHeroes() => heroes;

    public void ToggleBuyMode()
    {
        switch (CurrentBuyMode)
        {
            case BuyMode.x1: CurrentBuyMode = BuyMode.x10; break;
            case BuyMode.x10: CurrentBuyMode = BuyMode.x100; break;
            case BuyMode.x100: CurrentBuyMode = BuyMode.MAX; break;
            case BuyMode.MAX: CurrentBuyMode = BuyMode.x1; break;
        }

        GameEvents.OnBuyModeChanged?.Invoke();
    }

    public void HireHero(int index)
    {
        if (index < 0 || index >= heroes.Count) return;

        Hero hero = heroes[index];
        if (hero.IsHired()) return;

        double cost = hero.GetHireCost();
        if (GoldManager.Instance != null && GoldManager.Instance.SpendGold(cost))
        {
            hero.Hire();
            GameEvents.OnHeroesChanged?.Invoke();
        }
    }

    public void UpgradeHero(int index)
    {
        if (index < 0 || index >= heroes.Count) return;

        Hero hero = heroes[index];
        if (!hero.IsHired()) return;

        double currentGold = GoldManager.Instance != null ? GoldManager.Instance.GetGold() : 0;
        int levelsToBuy = 1;
        double cost = 0;

        switch (CurrentBuyMode)
        {
            case BuyMode.x1:
                levelsToBuy = 1;
                cost = hero.GetNextLevelCost();
                break;
            case BuyMode.x10:
                levelsToBuy = 10;
                cost = hero.GetCostForLevels(10);
                break;
            case BuyMode.x100:
                levelsToBuy = 100;
                cost = hero.GetCostForLevels(100);
                break;
            case BuyMode.MAX:
                levelsToBuy = hero.GetMaxAffordableLevels(currentGold, out cost);
                break;
        }

        if (levelsToBuy > 0 && GoldManager.Instance != null && GoldManager.Instance.SpendGold(cost))
        {
            hero.LevelUp(levelsToBuy);
            GameEvents.OnHeroesChanged?.Invoke();
        }
    }

    public double GetGlobalMultiplier()
    {
        double multiplier = 1.0;
        foreach (var hero in heroes)
        {
            if (hero.IsHired())
                multiplier *= hero.GetGlobalDPSMultiplier();
        }
        return multiplier;
    }

    public double GetTotalGoldMultiplier()
    {
        double multiplier = 1.0;
        foreach (var hero in heroes)
        {
            if (hero.IsHired())
                multiplier *= hero.GetGoldMultiplier();
        }
        return multiplier;
    }

    public double GetTotalDPS()
    {
        double totalDPS = 0;
        foreach (var hero in heroes)
        {
            if (hero != null)
            {
                totalDPS += hero.GetDPS(); // Урон каждого героя уже умножен на Алтарь и Скиллы!
            }
        }
        return totalDPS;
    }

    public void ResetAllHeroes()
    {
        foreach (var hero in heroes)
        {
            hero.ResetHero();
        }
        GameEvents.OnHeroesChanged?.Invoke();
    }
}