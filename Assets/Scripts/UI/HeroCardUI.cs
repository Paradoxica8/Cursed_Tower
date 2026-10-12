using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text dpsText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text abilitiesText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Image iconImage;

    private Hero hero;
    private int heroIndex;

    public void Setup(Hero targetHero, int index)
    {
        hero = targetHero;
        heroIndex = index;

        if (iconImage != null && hero.GetData().icon != null)
            iconImage.sprite = hero.GetData().icon;

        Refresh();
    }

    private void OnEnable()
    {
        GameEvents.OnBuyModeChanged += Refresh;
    }

    private void OnDisable()
    {
        GameEvents.OnBuyModeChanged -= Refresh;
    }

    public void Refresh()
    {
        if (hero == null) return;

        HeroData data = hero.GetData();
        double currentGold = GoldManager.Instance != null ? GoldManager.Instance.GetGold() : 0;

        if (nameText != null) nameText.text = data.heroName;

        if (hero.IsHired())
        {
            if (levelText != null) levelText.text = "Ур. " + hero.GetLevel();
            if (dpsText != null) dpsText.text = "DPS: " + NumberFormatter.Format(hero.GetDPS());

            BuyMode mode = HeroManager.Instance != null ? HeroManager.Instance.CurrentBuyMode : BuyMode.x1;
            int levelsToBuy = 1;
            double cost = 0;

            switch (mode)
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
                    if (levelsToBuy == 0) levelsToBuy = 1;
                    break;
            }

            if (costText != null)
                costText.text = NumberFormatter.Format(cost) + " золота";

            if (buttonText != null)
                buttonText.text = mode == BuyMode.x1 ? "ПРОКАЧАТЬ" : $"ПРОКАЧАТЬ +{levelsToBuy}";

            if (actionButton != null)
                actionButton.interactable = currentGold >= cost && cost > 0;

            if (abilitiesText != null)
                abilitiesText.text = GetAbilitiesString(data, hero.GetLevel());
        }
        else
        {
            if (levelText != null) levelText.text = "Не нанят";
            if (dpsText != null) dpsText.text = "DPS: " + NumberFormatter.Format(data.baseDPS);

            double cost = hero.GetHireCost();
            if (costText != null) costText.text = NumberFormatter.Format(cost) + " золота";
            if (buttonText != null) buttonText.text = "НАНЯТЬ";

            if (actionButton != null)
                actionButton.interactable = currentGold >= cost;

            if (abilitiesText != null) abilitiesText.text = "";
        }
    }

    private string GetAbilitiesString(HeroData data, int level)
    {
        string result = "";


        if (level >= data.ability1Level)
            result += "[v] " + data.ability1Name + "\n";
        else
            result += "[ ] Ур." + data.ability1Level + "\n";

        if (level >= data.ability2Level)
            result += "[v] " + data.ability2Name + "\n";
        else
            result += "[ ] Ур." + data.ability2Level + "\n";

        if (level >= data.ability3Level)
            result += "[v] " + data.ability3Name + "\n";
        else
            result += "[ ] Ур." + data.ability3Level + "\n";

        if (level >= data.ability4Level)
            result += "[v] " + data.ability4Name;
        else
            result += "[ ] Ур." + data.ability4Level;

        return result;
    }

    public void OnActionButtonClick()
    {
        if (hero == null || HeroManager.Instance == null) return;

        if (hero.IsHired())
            HeroManager.Instance.UpgradeHero(heroIndex);
        else
            HeroManager.Instance.HireHero(heroIndex);

        Refresh();
    }
}