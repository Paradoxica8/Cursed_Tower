using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    [Header("Prefabs & Spawn Point")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemySpawnPoint;

    [Header("Floor Settings")]
    public int enemiesPerFloor = 10;
    public float bossTimerDuration = 30f;

    [Header("Balance Scaling")]
    public double baseEnemyHealth = 10;
    public double healthMultiplier = 1.55;
    public double baseGoldReward = 2;
    public double goldMultiplier = 1.45;

    private int currentFloor = 1;
    private int enemiesDefeatedOnFloor = 0;
    private float currentBossTimer;
    private bool isBossActive;
    private Enemy currentEnemy;

    private void Awake() => Instance = this;

    private void OnEnable() => GameEvents.OnEnemyDied += OnEnemyDefeated;
    private void OnDisable() => GameEvents.OnEnemyDied -= OnEnemyDefeated;

    private void Start()
    {
        SpawnNextEnemy();
    }

    private void Update()
    {
        if (isBossActive)
        {
            currentBossTimer -= Time.deltaTime;
            GameEvents.OnBossTimerChanged?.Invoke(currentBossTimer);

            if (currentBossTimer <= 0)
            {
                FailBoss();
            }
        }
    }

    public void RegisterEnemy(Enemy enemy) => currentEnemy = enemy;
    public Enemy GetCurrentEnemy() => currentEnemy;

    private void OnEnemyDefeated()
    {
        enemiesDefeatedOnFloor++;

        if (enemiesDefeatedOnFloor >= (IsBossFloor() ? 1 : enemiesPerFloor))
        {
            if (IsBossFloor() && isBossActive)
            {
                CompleteBossFloor();
            }
            else if (!IsBossFloor())
            {
                StartBossFloor();
            }
        }
        else
        {
            SpawnNextEnemy();
        }

        GameEvents.OnFloorProgressChanged?.Invoke(enemiesDefeatedOnFloor, IsBossFloor() ? 1 : enemiesPerFloor);
    }

    private void StartBossFloor()
    {
        currentFloor++;
        enemiesDefeatedOnFloor = 0;

        if (IsBossFloor())
        {
            isBossActive = true;
            currentBossTimer = bossTimerDuration;
            GameEvents.OnBossStateChanged?.Invoke(true);
        }

        GameEvents.OnFloorChanged?.Invoke(currentFloor);
        SpawnNextEnemy();
    }

    private void CompleteBossFloor()
    {
        isBossActive = false;
        GameEvents.OnBossStateChanged?.Invoke(false);
        StartBossFloor();
    }

    private void FailBoss()
    {
        isBossActive = false;
        GameEvents.OnBossStateChanged?.Invoke(false);

        currentFloor--;
        enemiesDefeatedOnFloor = 0;

        GameEvents.OnFloorChanged?.Invoke(currentFloor);
        GameEvents.OnFloorProgressChanged?.Invoke(0, enemiesPerFloor);
        SpawnNextEnemy();
    }

    private void SpawnNextEnemy()
    {
        if (enemyPrefab == null || enemySpawnPoint == null) return;

        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject);
        }

        bool isBoss = IsBossFloor();

        double health = baseEnemyHealth * System.Math.Pow(healthMultiplier, currentFloor - 1);
        double gold = baseGoldReward * System.Math.Pow(goldMultiplier, currentFloor - 1);

        if (isBoss)
        {
            health *= 10;
            gold *= 5;
        }

        GameObject enemyObj = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity, enemySpawnPoint);
        currentEnemy = enemyObj.GetComponent<Enemy>();

        if (currentEnemy != null)
        {
            currentEnemy.Setup(health, gold, isBoss);
        }

        GameEvents.OnEnemySpawned?.Invoke(isBoss);
    }

    public bool IsBossFloor() => currentFloor % 5 == 0;
    public int GetCurrentFloor() => currentFloor;

    public void SetFloor(int floor)
    {
        currentFloor = floor;
        enemiesDefeatedOnFloor = 0;
        isBossActive = IsBossFloor();
        if (isBossActive) currentBossTimer = bossTimerDuration;

        GameEvents.OnBossStateChanged?.Invoke(isBossActive);
        GameEvents.OnFloorChanged?.Invoke(currentFloor);
        GameEvents.OnFloorProgressChanged?.Invoke(0, IsBossFloor() ? 1 : enemiesPerFloor);
        SpawnNextEnemy();
    }

    public void ResetToFloorOne()
    {
        currentFloor = 1;
        enemiesDefeatedOnFloor = 0;
        isBossActive = false;

        GameEvents.OnBossStateChanged?.Invoke(false);
        GameEvents.OnFloorChanged?.Invoke(currentFloor);
        GameEvents.OnFloorProgressChanged?.Invoke(0, enemiesPerFloor);

        SpawnNextEnemy();
    }
}