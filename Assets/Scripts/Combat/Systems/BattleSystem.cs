using System.Collections;
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
    bool playerCommandSelected = false;
    BattleCommand playerCommand;
    BattleCommand enemyCommand;

    public BattleTextBox textBox;

    public void StartBattle()
    {
        if (allUnits.Count < 2)
        {
            Debug.LogError("BattleSystem necesita al menos 2 unidades.");
            return;
        }

        Debug.Log("Battle started");

        BattleEvents.OnBattleStarted?.Invoke();

        currentState = BattleState.StartBattle;

        StartCoroutine(BattleLoop());
    }
    IEnumerator BattleLoop()
    {
        while (currentState != BattleState.EndBattle)
        {
            yield return StartCoroutine(PlayerInputPhase());

            yield return StartCoroutine(EnemyInputPhase());

            yield return StartCoroutine(ResolveTurnPhase());

            yield return StartCoroutine(EndTurnPhase());

            if (CheckBattleEnd())
            {
                EndBattle();
                yield break;
            }
        }
    }

    IEnumerator PlayerInputPhase()
    {
        currentState = BattleState.PlayerInput;

        playerCommandSelected = false;

        // Esperar hasta que el jugador elija movimiento
        yield return new WaitUntil(() => playerCommandSelected);
    }

    IEnumerator EnemyInputPhase()
    {
        currentState = BattleState.EnemyInput;

        CombatUnit enemy = allUnits[1];
        CombatUnit player = allUnits[0];

        MoveSO move = enemy.GetRandomMove();

        enemyCommand =
            BattleCommand.CreateMoveCommand(enemy, player, move);

        yield return null;
    }

    IEnumerator ResolveTurnPhase()
    {
        currentState = BattleState.ResolvingTurn;

        List<BattleCommand> commands =
            new List<BattleCommand>()
            {
                playerCommand,
                enemyCommand
            };

        yield return StartCoroutine(ResolveTurn(commands));
    }

    IEnumerator EndTurnPhase()
    {
        EndTurn();

        yield return new WaitForSeconds(0.5f);
    }

    public void StartTurn()
    {
        turnNumber++;

        Debug.Log($"---- TURN {turnNumber} START ----");

        BattleEvents.OnTurnStart?.Invoke();

        // aquí luego irán efectos de inicio de turno
    }

    IEnumerator ResolveTurn(List<BattleCommand> commands)
    {
        StartTurn();

        List<TurnAction> actions = new List<TurnAction>();

        foreach (var command in commands)
        {
            TurnAction action = CommandResolver.CreateAction(command);

            if (action != null)
                actions.Add(action);
        }

        yield return StartCoroutine(
            ExecuteTurn(actions)
        );
    }

    public void EndTurn()
    {
        Debug.Log($"---- TURN {turnNumber} END ----");

        foreach (var unit in allUnits)
        {
            unit.TickStatus();
        }

        BattleEvents.OnTurnEnd?.Invoke();
    }

    IEnumerator ExecuteTurn(List<TurnAction> actions)
    {
        List<TurnAction> ordered =
            TurnOrderResolver.Resolve(actions);

        debugUI.ShowTurnOrder(ordered);

        foreach (var action in ordered)
        {
            if (!BattleActionValidator.IsActionValid(action))
            {
                Debug.Log("Action inválida, se omite.");
                continue;
            }

            yield return StartCoroutine(ExecuteAction(action));
        }
    }

    IEnumerator ExecuteAction(TurnAction action)
    {
        string statusMessage;

        if (action.user.TryPreventAction(action.actionType, out statusMessage))
        {
            yield return StartCoroutine(
                textBox.ShowMessage(statusMessage)
            );

            yield break;
        }

        switch (action.actionType)
        {
            case BattleActionType.Move:

                yield return StartCoroutine(
                    ExecuteMove(action)
                );

                break;

            case BattleActionType.Item:

                UseItem(action);
                break;

            case BattleActionType.Switch:

                SwitchUnit(action);
                break;
        }
    }

    IEnumerator ExecuteMove(TurnAction action)
    {
        CombatUnit user = action.user;
        CombatUnit target = action.target;
        MoveSO move = action.move;

        yield return StartCoroutine(
            textBox.ShowMessage(
                $"{user.name} usó {move.moveName}"
            )
        );

        if (!target.IsAlive())
            yield break;

        if (!CheckAccuracy(user, target, move))
        {
            yield return StartCoroutine(
                textBox.ShowMessage("El ataque falló")
            );

            yield break;
        }

        bool isCritical = CheckCritical(user, target, move);

        float typeMultiplier = target.GetTypeMultiplier(move.moveType);

        MoveContext context = new MoveContext
        {
            move = move,
            user = user,
            target = target,
            isCritical = isCritical
        };

        move.effect.Execute(user, target, context);

        if (isCritical)
        {
            yield return StartCoroutine(
                textBox.ShowMessage("¡Golpe crítico!")
            );
        }

        yield return StartCoroutine(ShowEffectivenessMessage(typeMultiplier));

        yield return new WaitForSeconds(0.5f);
    }

    public void UseItem(TurnAction action)
    {

    }

    public void SwitchUnit(TurnAction action)
    {

    }

    public void PlayerChooseMove(CombatUnit player, CombatUnit target, MoveSO move)
    {
        if (currentState != BattleState.PlayerInput)
            return;

        playerCommand =
            BattleCommand.CreateMoveCommand(player, target, move);

        playerCommandSelected = true;
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

    private bool CheckAccuracy(CombatUnit user, CombatUnit target, MoveSO move)
    {
        float moveAccuracy = move.accuracy;

        print($"{move} +: Accuracy: {moveAccuracy}");

        int accuracyStage = user.GetStage(StatType.Accuracy);
        int evasionStage = target.GetStage(StatType.Evasion);

        float accuracyMultiplier = StatStageUtility.GetMultiplier(accuracyStage);

        float evasionMultiplier = StatStageUtility.GetMultiplier(evasionStage);

        float finalAccuracy = moveAccuracy * (accuracyMultiplier / evasionMultiplier);

        finalAccuracy = Mathf.Clamp(finalAccuracy, 0f, 100f);

        int roll = Random.Range(0, 101);

        Debug.Log(
            $"Accuracy {moveAccuracy} | AStage:{accuracyStage} " +
            $"EStage:{evasionStage} | Final:{finalAccuracy} VS Roll:{roll}" 
        );

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

    bool CheckBattleEnd()
    {
        bool playerAlive = allUnits[0].IsAlive();
        bool enemyAlive = allUnits[1].IsAlive();

        return !playerAlive || !enemyAlive;
    }

    void EndBattle()
    {
        currentState = BattleState.EndBattle;

        foreach (var unit in allUnits)
        {
            unit.ResetStages();
        }

        CombatUnit player = allUnits[0];
        CombatUnit enemy = allUnits[1];

        if (player.IsAlive())
            Debug.Log("Player wins!");
        else
            Debug.Log("Enemy wins!");
    }

    IEnumerator ShowEffectivenessMessage(float multiplier)
    {
        if (multiplier >= 1.5f)
        {
            yield return StartCoroutine(
                textBox.ShowMessage("¡Es super efectivo!")
            );
        }
        else if (multiplier > 0f && multiplier < 1f)
        {
            yield return StartCoroutine(
                textBox.ShowMessage("No es muy efectivo...")
            );
        }
        else if (multiplier == 0f)
        {
            yield return StartCoroutine(
                textBox.ShowMessage("No tuvo efecto...")
            );
        }
    }

    // TEST FUNCTIONS

    public void RunBattleTest()
    {
        StartBattle();
    }

    //DEPRECATED
    void SimulatePlayerInput()
    {
        CombatUnit player = allUnits[0];
        CombatUnit enemy = allUnits[1];

        MoveSO move = player.GetRandomMove();

        playerCommand =
            BattleCommand.CreateMoveCommand(player, enemy, move);
    }
    void SimulateEnemyInput()
    {
        CombatUnit enemy = allUnits[1];
        CombatUnit player = allUnits[0];

        MoveSO move = enemy.GetRandomMove();

        enemyCommand =
            BattleCommand.CreateMoveCommand(enemy, player, move);
    }

    public void RunBattleLoop() //Deprecated test function without corutines
    {
        StartBattle();

        while (currentState != BattleState.EndBattle)
        {
            SimulatePlayerInput();
            SimulateEnemyInput();

            ResolveCommands();

            if (CheckBattleEnd())
            {
                EndBattle();
                break;
            }
        }
    }

    public void UseMove(CombatUnit user, CombatUnit target, MoveSO move)
    {
        StartCoroutine(textBox.ShowMessage($"{user.name} usó {move.moveName}"));


        if (!target.IsAlive())
        {
            Debug.Log($"{user.name} no tiene objetivo válido.");
            return;
        }

        if (!CheckAccuracy(user, target, move))
        {
            Debug.Log($"{move.moveName} falló");
            StartCoroutine(textBox.ShowMessage("El ataque falló"));
            return;
        }

        bool isCritical = CheckCritical(user, target, move);

        if (isCritical)
        {
            StartCoroutine(textBox.ShowMessage("¡Golpe crítico!"));
        }

        MoveContext context = new MoveContext
        {
            move = move,
            user = user,
            target = target,
            isCritical = isCritical
        };

        move.effect.Execute(user, target, context);
    }

}