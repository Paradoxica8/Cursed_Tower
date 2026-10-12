using System.Collections.Generic;
using UnityEngine;

public class HeroListUI : MonoBehaviour
{
    [SerializeField] private GameObject heroCardPrefab;
    [SerializeField] private Transform contentContainer;

    private List<HeroCardUI> cards = new List<HeroCardUI>();

    private void OnEnable()
    {
        GameEvents.OnHeroesChanged += RefreshAll;
        GameEvents.OnGoldChanged += HandleGoldChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnHeroesChanged -= RefreshAll;
        GameEvents.OnGoldChanged -= HandleGoldChanged;
    }

    private void Start()
    {
        Invoke(nameof(InitializeList), 0.1f);
    }

    private void InitializeList()
    {
        if (contentContainer == null)
        {
            Debug.LogError("[HeroListUI] Пожалуйста, перетащите объект Content в поле Content Container в Inspector!");
            return;
        }

        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();

        if (HeroManager.Instance == null) return;

        var heroes = HeroManager.Instance.GetHeroes();

        for (int i = 0; i < heroes.Count; i++)
        {
            GameObject obj = Instantiate(heroCardPrefab, contentContainer);
            HeroCardUI cardUI = obj.GetComponent<HeroCardUI>();
            if (cardUI != null)
            {
                cardUI.Setup(heroes[i], i);
                cards.Add(cardUI);
            }
        }

        Debug.Log($"[HeroListUI] Создано карточек: {cards.Count}");
    }

    private void RefreshAll()
    {
        foreach (var card in cards)
        {
            if (card != null) card.Refresh();
        }
    }

    private void HandleGoldChanged(double totalGold)
    {
        RefreshAll();
    }
}