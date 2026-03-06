public static class BattleActionValidator
{
    public static bool IsActionValid(TurnAction action)
    {
        if (action == null)
            return false;

        if (action.user == null)
            return false;

        if (!action.user.IsAlive())
            return false;

        switch (action.actionType)
        {
            case BattleActionType.Move:

                if (action.move == null)
                    return false;

                if (action.target == null)
                    return false;

                if (!action.target.IsAlive())
                    return false;

                break;

            case BattleActionType.Item:

                if (action.item == null)
                    return false;

                break;

            case BattleActionType.Switch:

                if (action.switchTarget == null)
                    return false;

                break;
        }

        return true;
    }
}