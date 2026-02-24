using System.Collections.Generic;

[System.Serializable]
public class StatBlock
{
    private Dictionary<StatType, int> stats =
        new Dictionary<StatType, int>();

    public void SetStat(StatType stat, int value)
    {
        stats[stat] = value;
    }

    public int GetStat(StatType stat)
    {
        if (stats.TryGetValue(stat, out int value))
            return value;

        return 0;
    }

    public void ModifyStat(StatType stat, int amount)
    {
        stats[stat] = GetStat(stat) + amount;
    }

    public StatBlock Clone()
    {
        StatBlock newBlock = new StatBlock();

        foreach (var kvp in stats)
        {
            newBlock.SetStat(kvp.Key, kvp.Value);
        }

        return newBlock;
    }

    public void InitializeFromSO(StatBlockSO so)
    {
        stats.Clear();

        foreach (var entry in so.stats)
        {
            stats[entry.stat] = entry.value;
        }
    }
}