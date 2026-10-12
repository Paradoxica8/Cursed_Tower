using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string gold = "0";
    public string ash = "0";
    public int currentFloor = 1;
    public int clickLevel = 0;

    public List<int> heroLevels = new List<int>();

    public int talentDamageLevel = 0;
    public int talentGoldLevel = 0;
    public int talentClickLevel = 0;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SAVE_KEY = "CursedTower_SaveData";
    [SerializeField] private float autoSaveInterval = 10f;

    private float autoSaveTimer;

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
        Invoke(nameof(LoadGame), 0.1f);
    }

    private void Update()
    {
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            autoSaveTimer = 0f;
            SaveGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveGame();
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        if (GoldManager.Instance != null)
            data.gold = GoldManager.Instance.GetGold().ToString();

        if (PrestigeManager.Instance != null)
            data.ash = PrestigeManager.Instance.GetAsh().ToString();

        if (TowerManager.Instance != null)
            data.currentFloor = TowerManager.Instance.GetCurrentFloor();

        if (ClickManager.Instance != null)
            data.clickLevel = ClickManager.Instance.GetClickLevel();

        if (HeroManager.Instance != null)
        {
            foreach (var hero in HeroManager.Instance.GetHeroes())
            {
                data.heroLevels.Add(hero.GetLevel());
            }
        }

        if (PrestigeManager.Instance != null)
        {
            data.talentDamageLevel = PrestigeManager.Instance.GetDamageTalentLevel();
            data.talentGoldLevel = PrestigeManager.Instance.GetGoldTalentLevel();
            data.talentClickLevel = PrestigeManager.Instance.GetClickTalentLevel();
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("<color=green>[SaveManager] Игра успешно сохранена!</color>");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[SaveManager] Сохранений не найдено, новая игра.");
            return;
        }

        try
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data == null) return;

            if (double.TryParse(data.gold, out double loadedGold) && GoldManager.Instance != null)
            {
                GoldManager.Instance.ResetGold();
                GoldManager.Instance.AddGold(loadedGold);
            }

            if (PrestigeManager.Instance != null)
            {
                double.TryParse(data.ash, out double loadedAsh);
                PrestigeManager.Instance.SetDataFromSave(
                    loadedAsh,
                    data.talentDamageLevel,
                    data.talentGoldLevel,
                    data.talentClickLevel
                );
            }

            if (TowerManager.Instance != null && data.currentFloor > 1)
            {
                TowerManager.Instance.SetFloor(data.currentFloor);
            }

            if (ClickManager.Instance != null && data.clickLevel > 0)
            {
                ClickManager.Instance.SetClickLevel(data.clickLevel);
            }

            if (HeroManager.Instance != null && data.heroLevels != null)
            {
                var heroes = HeroManager.Instance.GetHeroes();
                for (int i = 0; i < heroes.Count && i < data.heroLevels.Count; i++)
                {
                    int lvl = data.heroLevels[i];
                    if (lvl > 0)
                    {
                        heroes[i].SetLevelFromSave(lvl);
                    }
                }
            }

            Debug.Log("<color=cyan>[SaveManager] Сохранение (включая Пепел и Таланты) успешно загружено!</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Ошибка при загрузке: {e.Message}");
        }
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();

        if (GoldManager.Instance != null) GoldManager.Instance.ResetGold();
        if (PrestigeManager.Instance != null) PrestigeManager.Instance.ResetAllPrestige();
        if (ClickManager.Instance != null) ClickManager.Instance.ResetProgress();
        if (HeroManager.Instance != null) HeroManager.Instance.ResetAllHeroes();
        if (TowerManager.Instance != null) TowerManager.Instance.ResetToFloorOne();

        SaveGame();

        Debug.Log("<color=yellow>[SaveManager] Полный сброс успешно выполнен!</color>");
    }
}