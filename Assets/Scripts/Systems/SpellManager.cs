using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance { get; private set; }

    [System.Serializable]
    public class SpellConfig
    {
        public SpellType spellType;
        public string spellName;
        public string description;
        public float cooldown = 30f;
        public float duration = 10f;
        public int unlockFloor = 1;
        public Sprite icon;
    }

    [Header("Spell Configurations")]
    [SerializeField] private List<SpellConfig> spellConfigs = new List<SpellConfig>();

    public float HeroDPSMultiplier { get; private set; } = 1f;
    public float GoldMultiplier { get; private set; } = 1f;

    private Dictionary<SpellType, float> cooldownTimers = new Dictionary<SpellType, float>();
    private Dictionary<SpellType, float> activeDurationTimers = new Dictionary<SpellType, float>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var config in spellConfigs)
        {
            cooldownTimers[config.spellType] = 0f;
            activeDurationTimers[config.spellType] = 0f;
        }
    }

    private void Update()
    {
        List<SpellType> keys = new List<SpellType>(cooldownTimers.Keys);
        foreach (var type in keys)
        {
            if (cooldownTimers[type] > 0)
            {
                cooldownTimers[type] -= Time.deltaTime;
                SpellConfig config = GetConfig(type);
                float maxCD = config != null ? config.cooldown : 1f;

                GameEvents.OnSpellCooldownUpdated?.Invoke(type, Mathf.Max(0, cooldownTimers[type]), maxCD);

                if (cooldownTimers[type] <= 0)
                {
                    cooldownTimers[type] = 0;
                    GameEvents.OnSpellReady?.Invoke(type);
                }
            }

            if (activeDurationTimers[type] > 0)
            {
                activeDurationTimers[type] -= Time.deltaTime;
                if (activeDurationTimers[type] <= 0)
                {
                    activeDurationTimers[type] = 0;
                    EndSpellEffect(type);
                }
            }
        }
    }

    public SpellConfig GetConfig(SpellType type)
    {
        return spellConfigs.Find(s => s.spellType == type);
    }

    private int GetCurrentFloor()
    {
        if (TowerManager.Instance == null)
            return 1;

        return TowerManager.Instance.CurrentFloor;
    }

    public bool IsUnlocked(SpellType type)
    {
        SpellConfig config = GetConfig(type);

        if (config == null)
        {
            Debug.LogWarning($"[SpellManager] Нет конфига для заклинания: {type}");
            return false;
        }

        int currentFloor = GetCurrentFloor();
        return currentFloor >= config.unlockFloor;
    }

    public bool CanCast(SpellType type)
    {
        SpellConfig config = GetConfig(type);

        if (config == null)
            return false;

        if (!cooldownTimers.ContainsKey(type))
            cooldownTimers[type] = 0f;

        int currentFloor = GetCurrentFloor();

        if (currentFloor < config.unlockFloor)
        {
            return false;
        }

        return cooldownTimers[type] <= 0f;
    }

    public bool CastSpell(SpellType type)
    {
        SpellConfig config = GetConfig(type);

        if (config == null)
        {
            Debug.LogWarning($"[SpellManager] Попытка использовать заклинание без конфига: {type}");
            return false;
        }

        int currentFloor = GetCurrentFloor();

        Debug.Log($"[SPELL TRY] {type} | Current Floor: {currentFloor} | Unlock Floor: {config.unlockFloor}");

        if (currentFloor < config.unlockFloor)
        {
            Debug.LogWarning($"[SPELL LOCKED] {type} откроется на этаже {config.unlockFloor}. Сейчас этаж {currentFloor}.");
            return false;
        }

        if (!cooldownTimers.ContainsKey(type))
            cooldownTimers[type] = 0f;

        if (cooldownTimers[type] > 0f)
        {
            Debug.LogWarning($"[SPELL COOLDOWN] {type} ещё на перезарядке.");
            return false;
        }

        cooldownTimers[type] = config.cooldown;

        ExecuteSpellEffect(config);

        GameEvents.OnSpellActivated?.Invoke(type, config.duration);

        return true;
    }
    private void ExecuteSpellEffect(SpellConfig config)
    {
        switch (config.spellType)
        {
            case SpellType.CursedStrike:
                double clickDamage = ClickManager.Instance != null ? ClickManager.Instance.GetClickDamage() : 10;
                double spellDamage = clickDamage * 20;

                if (TowerManager.Instance != null && TowerManager.Instance.CurrentEnemy != null)
                {
                    TowerManager.Instance.CurrentEnemy.TakeDamage(spellDamage);
                }
                break;

            case SpellType.HeroicFrenzy:
                HeroDPSMultiplier = 3f;
                activeDurationTimers[config.spellType] = config.duration;
                break;

            case SpellType.GoldSurge:
                double currentDPS = HeroManager.Instance != null ? HeroManager.Instance.GetTotalDPS() : 0;
                double instantGold = currentDPS * 15;
                if (instantGold < 10) instantGold = 10;

                if (GoldManager.Instance != null)
                {
                    GoldManager.Instance.AddGold(instantGold);
                }

                GoldMultiplier = 2f;
                activeDurationTimers[config.spellType] = config.duration;
                break;
        }
    }

    private void EndSpellEffect(SpellType type)
    {
        switch (type)
        {
            case SpellType.HeroicFrenzy:
                HeroDPSMultiplier = 1f;
                break;

            case SpellType.GoldSurge:
                GoldMultiplier = 1f;
                break;
        }
    }
}