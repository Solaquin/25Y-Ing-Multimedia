using UnityEngine;

public class BattleCommand
{
    public CombatUnit user;
    public CombatUnit target;

    public BattleActionType commandType;

    public MoveSO move;
    public ItemSO item;
    public ProfemonInstance switchTarget;

    private BattleCommand() { }

    //Command for execute movements
    public static BattleCommand CreateMoveCommand(
        CombatUnit user,
        CombatUnit target,
        MoveSO move)
    {
        return new BattleCommand
        {
            user = user,
            target = target,
            move = move,
            commandType = BattleActionType.Move
        };
    }

    //Commando for use items
    public static BattleCommand CreateItemCommand(
        CombatUnit user,
        CombatUnit target,
        ItemSO item)
    {
        return new BattleCommand
        {
            user = user,
            target = target,
            item = item,
            commandType = BattleActionType.Item
        };
    }

    //Command for Switch current BattleUnit
    //Command for Switch current BattleUnit
    public static BattleCommand CreateSwitchCommand(
        CombatUnit user,
        ProfemonInstance switchTarget)
    {
        return new BattleCommand
        {
            user = user,
            switchTarget = switchTarget,
            commandType = BattleActionType.Switch
        };
    }
}