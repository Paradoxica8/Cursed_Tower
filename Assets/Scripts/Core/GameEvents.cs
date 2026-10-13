using System;

public static class GameEvents
{
    public static Action<double> OnGoldChanged;

    public static Action<double> OnClickDamageChanged;
    public static Action<double, bool> OnDamageDealt;

    public static Action<int> OnFloorChanged;
    public static Action<int, int> OnFloorProgressChanged;

    public static Action<double, double> OnEnemyHealthChanged;
    public static Action<bool> OnEnemySpawned;
    public static Action OnEnemyDied;

    public static Action<bool> OnBossStateChanged;
    public static Action<float> OnBossTimerChanged;
    public static Action OnBossFailed;

    public static Action OnHeroesChanged;
    public static Action<double> OnTotalDPSChanged;

    public static System.Action OnBuyModeChanged;

    public static Action<SpellType, float> OnSpellActivated;
    public static Action<SpellType, float, float> OnSpellCooldownUpdated;
    public static Action<SpellType> OnSpellReady;
}