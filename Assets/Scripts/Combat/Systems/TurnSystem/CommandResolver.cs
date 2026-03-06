public static class CommandResolver
{
    public static TurnAction CreateAction(BattleCommand command)
    {
        return new TurnAction(command);
    }
}