using System;

public static class BattleEvents
{
    public static Action OnBattleStarted;

    public static Action OnTurnStart;

    public static Action OnTurnEnd;

    public static Action OnHPChanged;

    public static Action<string> OnBattleMessage;

    public static Action OnPlayerSwitchRequired;
}