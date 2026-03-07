public class StatusInstance
{
    public StatusEffectSO effect;

    public int remainingTurns;

    public StatusInstance(StatusEffectSO effect, int turns)
    {
        this.effect = effect;
        this.remainingTurns = turns;
    }
}