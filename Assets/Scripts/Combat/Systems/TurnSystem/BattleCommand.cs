using UnityEngine;

public class BattleCommand
{
    public BattleActionType actionType;

    public CombatUnit user;

    // Move
    public CombatUnit target;
    public MoveSO move;

    // Switch
    public ProfemonInstance switchTarget;

    // Item
    public BattleItemSO item;
    public ProfemonInstance itemTarget;

    private BattleCommand() { }

    //Command for execute movements
    public static BattleCommand CreateMoveCommand(
        CombatUnit user,
        CombatUnit target,
        MoveSO move)
    {
        return new BattleCommand
        {
            actionType = BattleActionType.Move,
            user = user,
            target = target,
            move = move
        };
    }

    //Commando for use items
    public static BattleCommand CreateItemCommand(
        CombatUnit user, BattleItemSO item, ProfemonInstance itemTarget)
    {
        return new BattleCommand
        {
            actionType = BattleActionType.Item,
            user = user,
            item = item,
            itemTarget = itemTarget
        };
    }

    //Command for Switch current BattleUnit
    public static BattleCommand CreateSwitchCommand(
        CombatUnit user,
        ProfemonInstance switchTarget)
    {
        return new BattleCommand
        {
            actionType = BattleActionType.Switch,
            user = user,
            switchTarget = switchTarget
        };
    }
}