using System;

public static class CostCalculator
{
    public static double GetCostForLevels(double baseCost, float costGrowth, int currentLevel, int count)
    {
        if (count <= 0) return 0;
        if (count == 1) return Math.Floor(baseCost * Math.Pow(costGrowth, currentLevel));

        double firstTerm = baseCost * Math.Pow(costGrowth, currentLevel);
        double sum = firstTerm * (Math.Pow(costGrowth, count) - 1.0) / (costGrowth - 1.0);
        return Math.Floor(sum);
    }

    public static int GetMaxAffordableLevels(double baseCost, float costGrowth, int currentLevel, double currentGold, out double totalCost)
    {
        double nextLevelCost = Math.Floor(baseCost * Math.Pow(costGrowth, currentLevel));
        if (currentGold < nextLevelCost)
        {
            totalCost = nextLevelCost;
            return 0;
        }

        double g = costGrowth;
        double inner = (currentGold * (g - 1.0) / nextLevelCost) + 1.0;
        int count = (int)Math.Floor(Math.Log(inner, g));

        if (count < 1) count = 1;

        totalCost = GetCostForLevels(baseCost, costGrowth, currentLevel, count);
        return count;
    }
}