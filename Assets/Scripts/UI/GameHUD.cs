using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("Top Bar")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text clickDamageText;
    [SerializeField] private TMP_Text totalDPSText;

    [Header("Floor")]
    [SerializeField] private TMP_Text floorText;
    [SerializeField] private TMP_Text progressText;

    [Header("Enemy")]
    [SerializeField] private Slider enemyHealthBar;
    [SerializeField] private TMP_Text enemyHealthText;

    [Header("Boss")]
    [SerializeField] private GameObject bossTimerPanel;
    [SerializeField] private TMP_Text bossTimerText;

    private void OnEnable()
    {
        GameEvents.OnGoldChanged += UpdateGold;
        GameEvents.OnClickDamageChanged += UpdateClickDamage;
        GameEvents.OnFloorChanged += UpdateFloor;
        GameEvents.OnFloorProgressChanged += UpdateProgress;
        GameEvents.OnEnemyHealthChanged += UpdateEnemyHealth;
        GameEvents.OnBossStateChanged += UpdateBossState;
        GameEvents.OnBossTimerChanged += UpdateBossTimer;
        GameEvents.OnTotalDPSChanged += UpdateTotalDPS;
    }

    private void OnDisable()
    {
        GameEvents.OnGoldChanged -= UpdateGold;
        GameEvents.OnClickDamageChanged -= UpdateClickDamage;
        GameEvents.OnFloorChanged -= UpdateFloor;
        GameEvents.OnFloorProgressChanged -= UpdateProgress;
        GameEvents.OnEnemyHealthChanged -= UpdateEnemyHealth;
        GameEvents.OnBossStateChanged -= UpdateBossState;
        GameEvents.OnBossTimerChanged -= UpdateBossTimer;
        GameEvents.OnTotalDPSChanged -= UpdateTotalDPS;
    }

    private void UpdateGold(double value)
    {
        if (goldText != null)
            goldText.text = NumberFormatter.Format(value);
    }

    private void UpdateClickDamage(double value)
    {
        if (clickDamageText != null)
            clickDamageText.text = "Урон клика: " + NumberFormatter.Format(value);
    }

    private void UpdateFloor(int floor)
    {
        if (floorText != null)
            floorText.text = "ЭТАЖ " + floor;
    }

    private void UpdateProgress(int killed, int total)
    {
        if (progressText != null)
            progressText.text = killed + " / " + total;
    }

    private void UpdateEnemyHealth(double current, double max)
    {
        if (enemyHealthBar != null)
            enemyHealthBar.value = max > 0 ? (float)(current / max) : 0f;

        if (enemyHealthText != null)
            enemyHealthText.text = NumberFormatter.Format(current) + " / " + NumberFormatter.Format(max);
    }

    private void UpdateBossState(bool active)
    {
        if (bossTimerPanel != null)
            bossTimerPanel.SetActive(active);
    }

    private void UpdateBossTimer(float remaining)
    {
        if (bossTimerText != null)
            bossTimerText.text = $"ВРЕМЯ: {NumberFormatter.FormatTime(remaining)}с";
    }

    private void UpdateTotalDPS(double dps)
    {
        if (totalDPSText != null)
            totalDPSText.text = "DPS: " + NumberFormatter.Format(dps);
    }
}