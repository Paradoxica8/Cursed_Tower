using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyModeToggleUI : MonoBehaviour
{
    [SerializeField] private Button toggleButton;
    [SerializeField] private TMP_Text buttonText;

    private void OnEnable()
    {
        GameEvents.OnBuyModeChanged += UpdateText;
    }

    private void OnDisable()
    {
        GameEvents.OnBuyModeChanged -= UpdateText;
    }

    private void Start()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(OnToggleClick);

        UpdateText();
    }

    private void OnToggleClick()
    {
        if (HeroManager.Instance != null)
            HeroManager.Instance.ToggleBuyMode();
    }

    private void UpdateText()
    {
        if (buttonText == null || HeroManager.Instance == null) return;

        BuyMode mode = HeroManager.Instance.CurrentBuyMode;
        buttonText.text = $"КУПИТЬ: {mode}";
    }
}