using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "CursedTower/Hero Data")]
public class HeroData : ScriptableObject
{
    [Header("Identity")]
    public string heroName = "Hero";
    public string heroId = "hero_01";
    public Sprite icon;

    [Header("Stats")]
    public double baseDPS = 1;
    public double baseCost = 10;
    public float costGrowth = 1.07f;

    [Header("Abilities (unlock at levels)")]
    public int ability1Level = 10;
    public string ability1Name = "Power Strike";
    [Tooltip("Множитель к собственному DPS")]
    public float ability1Multiplier = 2f;

    public int ability2Level = 25;
    public string ability2Name = "Team Boost";
    [Tooltip("Множитель к DPS всех героев")]
    public float ability2Multiplier = 1.5f;

    public int ability3Level = 50;
    public string ability3Name = "Gold Rush";
    [Tooltip("Множитель к получаемому золоту")]
    public float ability3Multiplier = 1.2f;

    public int ability4Level = 100;
    public string ability4Name = "Mastery";
    [Tooltip("Множитель к собственному DPS")]
    public float ability4Multiplier = 3f;
    public Color heroColor = Color.white;
}