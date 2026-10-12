using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrestigeUI : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject prestigePanel;
    [SerializeField] private Button openPanelButton;
    [SerializeField] private Button closePanelButton;

    [Header("Prestige Info")]
    [SerializeField] private TMP_Text currentAshText;
    [SerializeField] private TMP_Text pendingAshText;
    [SerializeField] private Button prestigeButton;
    [SerializeField] private TMP_Text prestigeButtonText;

    [Header("Talent 1: Damage")]
    [SerializeField] private TMP_Text damageTalentLevelText;
    [SerializeField] private TMP_Text damageTalentCostText;
    [SerializeField] private Button damageTalentButton;

    [Header("Talent 2: Gold")]
    [SerializeField] private TMP_Text goldTalentLevelText;
    [SerializeField] private TMP_Text goldTalentCostText;
    [SerializeField] private Button goldTalentButton;

    [Header("Talent 3: Click")]
    [SerializeField] private TMP_Text clickTalentLevelText;
    [SerializeField] private TMP_Text clickTalentCostText;
    [SerializeField] private Button clickTalentButton;

    private void OnEnable()
    {
        PrestigeManager.OnAshChanged += RefreshUI;
        GameEvents.OnFloorChanged += OnFloorChanged;
    }

    private void OnDisable()
    {
        PrestigeManager.OnAshChanged -= RefreshUI;
        GameEvents.OnFloorChanged -= OnFloorChanged;
    }

    private void Start()
    {
        if (openPanelButton != null)
            openPanelButton.onClick.AddListener(OpenPanel);

        if (closePanelButton != null)
            closePanelButton.onClick.AddListener(ClosePanel);

        if (prestigeButton != null)
            prestigeButton.onClick.AddListener(OnPrestigeClick);

        if (damageTalentButton != null)
            damageTalentButton.onClick.AddListener(OnUpgradeDamage);

        if (goldTalentButton != null)
            goldTalentButton.onClick.AddListener(OnUpgradeGold);

        if (clickTalentButton != null)
            clickTalentButton.onClick.AddListener(OnUpgradeClick);

        if (prestigePanel != null)
            prestigePanel.SetActive(false);

        RefreshUI();
    }

    public void OpenPanel()
    {
        if (prestigePanel != null) prestigePanel.SetActive(true);
        RefreshUI();
    }

    public void ClosePanel()
    {
        if (prestigePanel != null) prestigePanel.SetActive(false);
    }

    private void OnFloorChanged(int floor) => RefreshUI();

    public void RefreshUI()
    {
        if (PrestigeManager.Instance == null) return;

        double ash = PrestigeManager.Instance.GetAsh();
        double pendingAsh = PrestigeManager.Instance.CalculateAshReward();

        if (currentAshText != null)
            currentAshText.text = $"Пепел: {NumberFormatter.Format(ash)}";

        if (pendingAshText != null)
            pendingAshText.text = $"Получите при сбросе: +{NumberFormatter.Format(pendingAsh)} Пепла";

        if (prestigeButton != null)
        {
            bool canPrestige = pendingAsh > 0;
            prestigeButton.interactable = canPrestige;

            if (prestigeButtonText != null)
                prestigeButtonText.text = canPrestige ? "ПЕРЕРОДИТЬСЯ" : "Нужен 15+ этаж";
        }

        int dmgLvl = PrestigeManager.Instance.GetDamageTalentLevel();
        double dmgCost = PrestigeManager.Instance.GetTalentCost(dmgLvl);
        if (damageTalentLevelText != null)
            damageTalentLevelText.text = $"Сила Предков (Ур. {dmgLvl})\n+{(dmgLvl * 25)}% Урона";
        if (damageTalentCostText != null)
            damageTalentCostText.text = $"{NumberFormatter.Format(dmgCost)} Пепла";
        if (damageTalentButton != null)
            damageTalentButton.interactable = ash >= dmgCost;

        int goldLvl = PrestigeManager.Instance.GetGoldTalentLevel();
        double goldCost = PrestigeManager.Instance.GetTalentCost(goldLvl);
        if (goldTalentLevelText != null)
            goldTalentLevelText.text = $"Богатство Башни (Ур. {goldLvl})\n+{(goldLvl * 20)}% Золота";
        if (goldTalentCostText != null)
            goldTalentCostText.text = $"{NumberFormatter.Format(goldCost)} Пепла";
        if (goldTalentButton != null)
            goldTalentButton.interactable = ash >= goldCost;

        int clickLvl = PrestigeManager.Instance.GetClickTalentLevel();
        double clickCost = PrestigeManager.Instance.GetTalentCost(clickLvl);
        if (clickTalentLevelText != null)
            clickTalentLevelText.text = $"Мощный Клик (Ур. {clickLvl})\n+{(clickLvl * 30)}% Клика";
        if (clickTalentCostText != null)
            clickTalentCostText.text = $"{NumberFormatter.Format(clickCost)} Пепла";
        if (clickTalentButton != null)
            clickTalentButton.interactable = ash >= clickCost;
    }

    private void OnPrestigeClick()
    {
        if (PrestigeManager.Instance != null && PrestigeManager.Instance.PerformPrestige())
        {
            RefreshUI();
            ClosePanel();
        }
    }

    private void OnUpgradeDamage()
    {
        if (PrestigeManager.Instance != null && PrestigeManager.Instance.UpgradeDamageTalent())
            RefreshUI();
    }

    private void OnUpgradeGold()
    {
        if (PrestigeManager.Instance != null && PrestigeManager.Instance.UpgradeGoldTalent())
            RefreshUI();
    }

    private void OnUpgradeClick()
    {
        if (PrestigeManager.Instance != null && PrestigeManager.Instance.UpgradeClickTalent())
            RefreshUI();
    }
}