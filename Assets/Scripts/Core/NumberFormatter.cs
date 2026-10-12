using System;

public static class NumberFormatter
{
    private static readonly string[] Suffixes =
    {
        "", "K", "M", "B", "T",
        "Qa", "Qi", "Sx", "Sp", "Oc", "No",
        "Dc", "Ud", "Dd", "Td", "Qad", "Qid",
        "Sxd", "Spd", "Ocd", "Nod", "Vg"
    };

    public static string Format(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return "∞";

        if (value < 0)
            return "-" + Format(-value);

        if (value < 1000)
        {
            if (value < 10 && value % 1 != 0)
                return value.ToString("0.0");

            return Math.Floor(value).ToString("0");
        }

        int tier = (int)(Math.Log10(value) / 3);

        if (tier >= Suffixes.Length)
            return value.ToString("0.00e0");

        double scaled = value / Math.Pow(1000, tier);

        return scaled.ToString("0.00") + Suffixes[tier];
    }

    public static string FormatTime(float seconds)
    {
        if (seconds < 0) seconds = 0;
        int m = (int)(seconds / 60);
        int s = (int)(seconds % 60);
        return m > 0 ? $"{m}:{s:00}" : $"{s}";
    }
}