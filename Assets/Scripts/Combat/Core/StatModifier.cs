public class StatModifier
{
    public StatType stat;
    public int amount;
    public int remainingTurns;

    public StatModifier(
        StatType stat,
        int amount,
        int duration
    )
    {
        this.stat = stat;
        this.amount = amount;
        this.remainingTurns = duration;
    }

    public void Tick()
    {
        remainingTurns--;
    }

    public bool IsExpired()
    {
        return remainingTurns <= 0;
    }
}