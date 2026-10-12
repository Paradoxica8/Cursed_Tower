using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickUpgradeButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private void OnEnable()
    {
        GameEvents.OnGoldChanged += OnGoldChanged;
        GameEvents.OnClickDamageChanged += OnClickDamageChanged;
        Refresh();
    }

    private void OnDisable()
    {
        GameEvents.OnGoldChanged -= OnGoldChanged;
        GameEvents.OnClickDamageChanged -= OnClickDamageChanged;
    }

    private void OnGoldChanged(double v) => Refresh();
    private void OnClickDamageChanged(double v) => Refresh();

    public void OnClick()
    {
        if (ClickManager.Instance != null)
            ClickManager.Instance.UpgradeClick();
    }

    private void Refresh()
    {
        if (ClickManager.Instance == null || GoldManager.Instance == null) return;

        double cost = ClickManager.Instance.GetUpgradeCost();

        if (label != null)
        {
            label.text =
                "УРОН КЛИКА  Ур. " + ClickManager.Instance.GetClickLevel() +
                "\n" + NumberFormatter.Format(cost) + " золота";
        }

        if (button != null)
            button.interactable = GoldManager.Instance.CanAfford(cost);
    }
}