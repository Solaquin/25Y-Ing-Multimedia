using UnityEngine;

public enum BattleActionType
{
    Move,
    Item,
    Switch
}

public class TurnAction
{
    public CombatUnit user;
    public CombatUnit target;

    public BattleActionType actionType;

    public MoveSO move;
    public ItemSO item;
    public CombatUnit switchTarget;

    public int priority;
    public int speed;

    public TurnAction(BattleCommand command)
    {
        user = command.user;
        target = command.target;
        actionType = command.commandType;

        switch (actionType)
        {
            case BattleActionType.Move:
                move = command.move;
                priority = move.priority;
                break;

            case BattleActionType.Item:
                item = command.item;
                priority = 3; // ejemplo: objetos rápidos
                break;

            case BattleActionType.Switch:
                switchTarget = command.switchTarget;
                priority = 6; // cambiar es muy rápido
                break;
        }

        speed = user.GetStat(StatType.Speed);
    }

    public override string ToString()
    {
        switch (actionType)
        {
            case BattleActionType.Move:
                return $"{user.name} -> {move.moveName} (P:{priority}, S:{speed})";

            case BattleActionType.Item:
                return $"{user.name} uses item {item.name}";

            case BattleActionType.Switch:
                return $"{user.name} switches to {switchTarget.name}";

            default:
                return $"{user.name} unknown action";
        }
    }
}
