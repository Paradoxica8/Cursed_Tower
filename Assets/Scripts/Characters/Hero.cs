using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Attack Visual")]
    [SerializeField] private float attackAnimDuration = 0.15f;
    [SerializeField] private float attackScale = 1.3f;

    private HeroData data;
    private int level = 0;
    private bool isHired = false;

    private float attackTimer;
    private float animTimer;
    private bool isAttacking;

    private Vector3 originalScale;

    public void Init(HeroData heroData)
    {
        data = heroData;
        level = 0;
        isHired = false;
        attackTimer = 0f;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = data.icon;
            spriteRenderer.color = data.heroColor;
            spriteRenderer.sortingOrder = 10;
        }

        originalScale = transform.localScale;
        gameObject.SetActive(false);

        spriteRenderer.color = data.heroColor;
    }

    public void Hire()
    {
        isHired = true;
        level = 1;
        gameObject.SetActive(true);
        attackTimer = 0f;
    }

    public void LevelUp()
    {
        if (!isHired) return;
        level++;
    }

    public void SetLevelFromSave(int savedLevel)
    {
        if (savedLevel <= 0) return;
        level = savedLevel;
        isHired = true;
        gameObject.SetActive(true);
    }

    public HeroData GetData() => data;
    public int GetLevel() => level;
    public bool IsHired() => isHired;

    public double GetDPS()
    {
        if (!isHired || data == null) return 0;

        double dps = data.baseDPS * level;

        if (level >= data.ability1Level)
            dps *= data.ability1Multiplier;

        if (level >= data.ability4Level)
            dps *= data.ability4Multiplier;

        return dps;
    }

    public double GetGlobalDPSMultiplier()
    {
        if (!isHired || data == null) return 1f;

        if (level >= data.ability2Level)
            return data.ability2Multiplier;

        return 1f;
    }

    public double GetGoldMultiplier()
    {
        if (!isHired || data == null) return 1f;

        if (level >= data.ability3Level)
            return data.ability3Multiplier;

        return 1f;
    }

    public double GetNextLevelCost()
    {
        if (data == null) return 0;
        return System.Math.Floor(data.baseCost * System.Math.Pow(data.costGrowth, level));
    }

    public double GetHireCost()
    {
        if (data == null) return 0;
        return data.baseCost;
    }

    private void Update()
    {
        if (!isHired) return;

        UpdateAttack();
        UpdateAnimation();
    }

    private void UpdateAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= 1f)
        {
            attackTimer -= 1f;
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (TowerManager.Instance == null) return;

        Enemy enemy = TowerManager.Instance.GetCurrentEnemy();
        if (enemy == null || enemy.IsDead()) return;

        double dps = GetEffectiveDPS();
        if (dps <= 0) return;

        enemy.TakeDamage(dps);
        GameEvents.OnDamageDealt?.Invoke(dps, false);

        isAttacking = true;
        animTimer = attackAnimDuration;
    }

    private double GetEffectiveDPS()
    {
        double dps = GetDPS();

        if (HeroManager.Instance != null)
            dps *= HeroManager.Instance.GetGlobalMultiplier();

        return dps;
    }

    private void UpdateAnimation()
    {
        if (!isAttacking) return;

        animTimer -= Time.deltaTime;

        if (animTimer <= 0f)
        {
            isAttacking = false;
            transform.localScale = originalScale;
        }
        else
        {
            float t = animTimer / attackAnimDuration;
            float s = Mathf.Lerp(1f, attackScale, t);
            transform.localScale = originalScale * s;
        }
    }

    public void ResetHero()
    {
        level = 0;
        isHired = false;
        attackTimer = 0f;
        isAttacking = false;
        transform.localScale = originalScale;
        gameObject.SetActive(false);
    }

    public void LevelUp(int amount)
    {
        if (!isHired || amount <= 0) return;
        level += amount;
    }

    public double GetCostForLevels(int count)
    {
        if (data == null) return 0;
        return CostCalculator.GetCostForLevels(data.baseCost, data.costGrowth, level, count);
    }

    public int GetMaxAffordableLevels(double currentGold, out double totalCost)
    {
        if (data == null)
        {
            totalCost = 0;
            return 0;
        }
        return CostCalculator.GetMaxAffordableLevels(data.baseCost, data.costGrowth, level, currentGold, out totalCost);
    }
}