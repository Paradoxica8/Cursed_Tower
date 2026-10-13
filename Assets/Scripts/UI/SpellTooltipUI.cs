using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellTooltipUI : MonoBehaviour
{
    public static SpellTooltipUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI statusText;

    private RectTransform panelRect;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (tooltipPanel != null)
        {
            panelRect = tooltipPanel.GetComponent<RectTransform>();

            // Отключаем RaycastTarget, чтобы тултип не перехватывал мышь
            var img = tooltipPanel.GetComponent<Image>();
            if (img != null) img.raycastTarget = false;
        }

        HideTooltip();
    }

    public void ShowTooltip(SpellManager.SpellConfig config, Vector3 buttonWorldPosition)
    {
        if (config == null || tooltipPanel == null) return;

        // Заполняем тексты
        if (titleText != null) titleText.text = config.spellName;
        if (descriptionText != null) descriptionText.text = config.description;

        int currentFloor = TowerManager.Instance != null ? TowerManager.Instance.CurrentFloor : 1;
        bool isUnlocked = currentFloor >= config.unlockFloor;

        if (statsText != null)
        {
            string stats = $"КД: {config.cooldown}с";
            if (config.duration > 0) stats += $" | Длительность: {config.duration}с";
            statsText.text = stats;
        }

        if (statusText != null)
        {
            statusText.text = isUnlocked ? "<color=#44FF44>Готово к использованию</color>" :
                                         $"<color=#FF4444>Требуется {config.unlockFloor} этаж</color>";
        }

        tooltipPanel.SetActive(true);

        // Обновляем верстку UI перед позиционированием
        Canvas.ForceUpdateCanvases();

        // Позиционируем ровно над кнопкой
        if (panelRect != null)
        {
            panelRect.position = buttonWorldPosition;
            // Сдвигаем вверх от кнопки на 55 пикселей (так как Pivot Y = 0)
            panelRect.anchoredPosition += new Vector2(0, 55f);
        }
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}