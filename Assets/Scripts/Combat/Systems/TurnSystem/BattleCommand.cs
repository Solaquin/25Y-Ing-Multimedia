using UnityEngine;

public class BattleCommand
{
    public CombatUnit user;
    public CombatUnit target;

    public BattleActionType commandType;

    public MoveSO move;
    public ItemSO item;
    public CombatUnit switchTarget;

    private BattleCommand() { }

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

    public static BattleCommand CreateSwitchCommand(
        CombatUnit user,
        CombatUnit switchTarget)
    {
        return new BattleCommand
        {
            user = user,
            switchTarget = switchTarget,
            commandType = BattleActionType.Switch
        };
    }
}