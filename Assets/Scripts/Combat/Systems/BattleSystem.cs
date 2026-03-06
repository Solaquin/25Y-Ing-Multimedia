using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    StartBattle,
    PlayerInput,
    EnemyInput,
    ResolvingTurn,
    Busy,
    EndBattle
}

public class BattleSystem : MonoBehaviour
{
    public TurnOrderDebugUI debugUI;
    public BattleState currentState;
    public int turnNumber = 0;

    public List<CombatUnit> allUnits = new List<CombatUnit>();
    BattleCommand playerCommand;
    BattleCommand enemyCommand;

    void Start()
    {
        StartBattle();
    }

    void StartBattle()
    {
        Debug.Log("Battle started");

        currentState = BattleState.PlayerInput;
    }

    public void StartTurn()
    {
        turnNumber++;

        Debug.Log($"---- TURN {turnNumber} START ----");

        // aquí luego irán efectos de inicio de turno
    }

    public void ResolveTurn(List<BattleCommand> commands)
    {
        StartTurn();

        List<TurnAction> actions = new List<TurnAction>();

        foreach (var command in commands)
        {
            TurnAction action = CommandResolver.CreateAction(command);

            if (action != null)
                actions.Add(action);
        }

        ExecuteTurn(actions);

        EndTurn();
    }

    public void EndTurn()
    {
        Debug.Log($"---- TURN {turnNumber} END ----");

        foreach (var unit in allUnits)
        {
            unit.TickModifiers();
        }
    }

    void ExecuteAction(TurnAction action)
    {
        switch (action.actionType)
        {
            case BattleActionType.Move:
                UseMove(action.user, action.target, action.move);
                break;

            case BattleActionType.Item:
                UseItem(action);
                break;

            case BattleActionType.Switch:
                SwitchUnit(action);
                break;
        }
    }

    public void ExecuteTurn(List<TurnAction> actions)
    {
        List<TurnAction> ordered =
            TurnOrderResolver.Resolve(actions);

        // Debug turn
        debugUI.ShowTurnOrder(ordered);

        foreach (var action in ordered)
        {
            if (!action.user.IsAlive())
                continue;

            ExecuteAction(action);
        }
    }

    public void UseItem(TurnAction action)
    {

    }

    public void SwitchUnit(TurnAction action)
    {

    }

    void ResolveCommands()
    {
        currentState = BattleState.ResolvingTurn;

        List<BattleCommand> commands =
            new List<BattleCommand>()
            {
            playerCommand,
            enemyCommand
            };

        ResolveTurn(commands);

        currentState = BattleState.PlayerInput;
    }

    public void UseMove(CombatUnit user, CombatUnit target, MoveSO move)
    {
        if (!CheckAccuracy(user, target, move))
        {
            Debug.Log($"{move.moveName} falló");
            return;
        }

        bool isCritical = CheckCritical(user, target, move);

        MoveContext context = new MoveContext
        {
            move = move,
            user = user,
            target = target,
            isCritical = isCritical
        };

        move.effect.Execute(user, target, context);
    }

    private bool CheckAccuracy(CombatUnit user, CombatUnit target, MoveSO move)
    {
        float moveAccuracy = move.accuracy;

        print($"{move} +: Accuracy: {moveAccuracy}");

        float userAccuracy = user.GetStat(StatType.Accuracy) / 100.0f;

        float targetEvasion = target.GetStat(StatType.Evasion) / 100.0f;

        if (targetEvasion <= 0)
            targetEvasion = 1.0f;

        float finalAccuracy = moveAccuracy * (userAccuracy / targetEvasion);

        finalAccuracy = Mathf.Clamp(finalAccuracy, 0f, 100f);

        int roll = Random.Range(0, 101);

        Debug.Log($"Accuracy Roll: {roll} vs {finalAccuracy}");

        return roll <= finalAccuracy;
    }

    private bool CheckCritical(CombatUnit user, CombatUnit target, MoveSO move)
    {
        float userSpeed =
            user.GetStat(StatType.Speed);

        float targetSpeed =
            target.GetStat(StatType.Speed);

        if (targetSpeed <= 0)
            targetSpeed = 1;

        float critChance = move.baseCritChance * (userSpeed / targetSpeed);

        critChance = Mathf.Clamp(critChance, 0f, 100f);

        float roll = Random.Range(0f, 100f);

        Debug.Log($"Crit Roll: {roll} vs {critChance} = Crit: {roll <= critChance}");

        return roll <= critChance;
    }

    public void PlayerChooseMove(CombatUnit player, CombatUnit target, MoveSO move)
    {
        if (currentState != BattleState.PlayerInput)
            return;

        playerCommand =
            BattleCommand.CreateMoveCommand(player, target, move);

        currentState = BattleState.EnemyInput;

        EnemyChooseMove();
    }

    void EnemyChooseMove()
    {
        CombatUnit enemy = allUnits[1];
        CombatUnit player = allUnits[0];

        MoveSO move = enemy.GetRandomMove();

        enemyCommand =
            BattleCommand.CreateMoveCommand(enemy, player, move);

        ResolveCommands();
    }

}