using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // Важно для событий мыши
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SpellButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Spell Type")]
    [SerializeField] private SpellType spellType;

    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private GameObject activeGlow;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private TextMeshProUGUI lockFloorText;

    private Button castButton;

    private void Awake()
    {
        castButton = GetComponent<Button>();

        castButton.onClick.RemoveListener(OnCastClicked);
        castButton.onClick.AddListener(OnCastClicked);

        castButton.interactable = false;
    }

    private void Start()
    {
        SetupUI();
        RefreshState();
    }

    private void OnEnable()
    {
        GameEvents.OnSpellCooldownUpdated += UpdateCooldownUI;
        GameEvents.OnSpellActivated += OnSpellActivated;
        GameEvents.OnSpellReady += OnSpellReady;
        GameEvents.OnFloorChanged += OnFloorChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnSpellCooldownUpdated -= UpdateCooldownUI;
        GameEvents.OnSpellActivated -= OnSpellActivated;
        GameEvents.OnSpellReady -= OnSpellReady;
        GameEvents.OnFloorChanged -= OnFloorChanged;
    }

    // --- НАВЕДЕНИЕ МЫШИ (TOOLTIP) ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[SpellButtonUI] Мышь наведена на заклинание: {spellType}");

        if (SpellManager.Instance == null) return;

        var config = SpellManager.Instance.GetConfig(spellType);
        if (config != null && SpellTooltipUI.Instance != null)
        {
            SpellTooltipUI.Instance.ShowTooltip(config, transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SpellTooltipUI.Instance != null)
        {
            SpellTooltipUI.Instance.HideTooltip();
        }
    }

    private void SetupUI()
    {
        if (SpellManager.Instance == null) return;
        var config = SpellManager.Instance.GetConfig(spellType);

        if (config != null)
        {
            if (iconImage != null && config.icon != null)
                iconImage.sprite = config.icon;

            if (lockFloorText != null)
                lockFloorText.text = $"{config.unlockFloor} ЭТ.";
        }

        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        if (cooldownText != null) cooldownText.text = "";
        if (activeGlow != null) activeGlow.SetActive(false);
    }

    private void RefreshState()
    {
        if (SpellManager.Instance == null)
        {
            castButton.interactable = false;
            return;
        }

        var config = SpellManager.Instance.GetConfig(spellType);
        if (config == null)
        {
            castButton.interactable = false;
            return;
        }

        bool isUnlocked = SpellManager.Instance.IsUnlocked(spellType);
        bool canCast = SpellManager.Instance.CanCast(spellType);

        if (lockOverlay != null)
            lockOverlay.SetActive(!isUnlocked);

        if (lockFloorText != null)
            lockFloorText.text = $"{config.unlockFloor} ЭТ.";

        castButton.interactable = canCast;

        if (!isUnlocked)
        {
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            if (cooldownText != null) cooldownText.text = "";
        }
    }

    private void OnCastClicked()
    {
        if (SpellManager.Instance == null) return;

        bool success = SpellManager.Instance.CastSpell(spellType);
        RefreshState();

        if (SpellTooltipUI.Instance != null)
        {
            SpellTooltipUI.Instance.HideTooltip();
        }
    }

    private void UpdateCooldownUI(SpellType type, float currentCD, float maxCD)
    {
        if (type != spellType || SpellManager.Instance == null) return;

        bool isUnlocked = SpellManager.Instance.IsUnlocked(spellType);
        if (!isUnlocked)
        {
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            if (cooldownText != null) cooldownText.text = "";
            castButton.interactable = false;
            return;
        }

        if (currentCD > 0f)
        {
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = currentCD / maxCD;
            if (cooldownText != null) cooldownText.text = Mathf.CeilToInt(currentCD).ToString();
            castButton.interactable = false;
        }
        else
        {
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            if (cooldownText != null) cooldownText.text = "";
            castButton.interactable = SpellManager.Instance.CanCast(spellType);
        }
    }

    private void OnSpellActivated(SpellType type, float duration)
    {
        if (type != spellType) return;
        RefreshState();

        if (activeGlow != null && duration > 0f)
        {
            activeGlow.SetActive(true);
            CancelInvoke(nameof(HideActiveGlow));
            Invoke(nameof(HideActiveGlow), duration);
        }
    }

    private void OnSpellReady(SpellType type)
    {
        if (type != spellType) return;
        RefreshState();
    }

    private void OnFloorChanged(int newFloor)
    {
        RefreshState();
    }

    private void HideActiveGlow()
    {
        if (activeGlow != null)
            activeGlow.SetActive(false);
    }
}