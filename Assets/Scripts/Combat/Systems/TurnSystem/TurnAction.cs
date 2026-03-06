using UnityEngine;

public class TurnAction
{
    public CombatUnit user;
    public CombatUnit target;
    public MoveSO move;

    public TurnAction(
        CombatUnit user,
        CombatUnit target,
        MoveSO move
    )
    {
        this.user = user;
        this.target = target;
        this.move = move;
    }

    public override string ToString()
    {
        return $"{user.name} -> {move.moveName} (P:{move.priority}, S:{user.GetStat(StatType.Speed)})";
    }
}
