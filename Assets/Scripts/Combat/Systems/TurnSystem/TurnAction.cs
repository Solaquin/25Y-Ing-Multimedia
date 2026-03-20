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
    public BattleItemSO item;
    public ProfemonInstance itemTarget;  // añadido
    public ProfemonInstance switchTarget;

    public int priority;
    public int speed;

    public TurnAction(BattleCommand command)
    {
        user = command.user;
        target = command.target;
        actionType = command.actionType;

        switch (actionType)
        {
            case BattleActionType.Move:
                move = command.move;
                priority = move.priority;
                break;

            case BattleActionType.Item:
                item = command.item;
                itemTarget = command.itemTarget;  // añadido
                priority = 3;
                break;

            case BattleActionType.Switch:
                switchTarget = command.switchTarget;
                priority = 6;
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
                return $"{user.name} usa {item.displayName} en {itemTarget.data.professorName}";

            case BattleActionType.Switch:
                return $"{user.name} cambia a {switchTarget.data.professorName}";

            default:
                return $"{user.name} acción desconocida";
        }
    }
}