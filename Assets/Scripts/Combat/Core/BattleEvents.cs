using System;

public static class BattleEvents
{
    public static Action OnBattleStarted;

    public static Action OnBattleEnded;

    public static Action OnTurnStart;

    public static Action OnTurnEnd;

    public static Action OnHPChanged;

    public static Action<BattleState> OnBattleStateChanged;

    public static Action<string, Action> OnMessageRequested;

    public static Action OnPlayerSwitchRequired;

    public static Action<BattleUnitSide> OnActiveUnitChanged;
}