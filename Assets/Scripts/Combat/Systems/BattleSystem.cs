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

    NPCParty enemyParty;

    public CombatUnit playerUnit;
    public CombatUnit enemyUnit;

    public void SetupBattle(NPCParty enemyParty)
    {
        this.enemyParty = enemyParty;

        ProfemonInstance playerInstance =
            PlayerPartyManager.Instance.GetFirstAlive();

        ProfemonInstance enemyInstance =
            enemyParty.GetFirstAlive();

        SpawnUnits(playerInstance, enemyInstance);

        StartBattle();
    }

    void SpawnUnits(ProfemonInstance playerInstance, ProfemonInstance enemyInstance)
    {
        playerUnit.InitializeFromInstance(playerInstance);
        enemyUnit.InitializeFromInstance(enemyInstance);

        allUnits.Clear();

        allUnits.Add(playerUnit);
        allUnits.Add(enemyUnit);
    }

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

        CombatUnit enemy = enemyUnit;
        CombatUnit player = playerUnit;

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
                continue;

            yield return StartCoroutine(ExecuteAction(action));

            // comprobar KO inmediatamente
            if (!playerUnit.IsAlive())
            {
                yield return StartCoroutine(HandlePlayerKO());
                yield break;
            }

            if (!enemyUnit.IsAlive())
            {
                yield return StartCoroutine(HandleEnemyKO());
                yield break;
            }
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

                yield return StartCoroutine(
                    SwitchUnit(action)
                );

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
                $"{user.Instance.data.professorName} usó {move.moveName}"
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

    public IEnumerator SwitchUnit(TurnAction action)
    {
        CombatUnit unit = action.user;

        ProfemonInstance newInstance = action.switchTarget;

        yield return StartCoroutine(
            textBox.ShowMessage($"{unit.Instance.data.professorName} vuelve.")
        );

        yield return new WaitForSeconds(0.5f);

        if (unit == playerUnit)
        {
            playerUnit.InitializeFromInstance(newInstance);
        }
        else
        {
            enemyUnit.InitializeFromInstance(newInstance);
        }

        yield return StartCoroutine(
            textBox.ShowMessage($"{newInstance.data.professorName} entra al combate.")
        );

        BattleEvents.OnActiveUnitChanged?.Invoke();

        yield return new WaitForSeconds(0.5f);
    }

    public void PlayerChooseMove(CombatUnit player, CombatUnit target, MoveSO move)
    {
        if (currentState != BattleState.PlayerInput)
            return;

        playerCommand =
            BattleCommand.CreateMoveCommand(player, target, move);

        playerCommandSelected = true;
    }

    public void PlayerChooseSwitch(ProfemonInstance instance)
    {
        if (currentState != BattleState.PlayerInput &&
            currentState != BattleState.Busy)
            return;

        playerCommand = BattleCommand.CreateSwitchCommand(playerUnit, instance);

        playerCommandSelected = true;
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
        if (!playerUnit.IsAlive() &&
            !PlayerPartyManager.Instance.HasAvailable())
            return true;

        if (!enemyUnit.IsAlive() &&
            !enemyParty.HasAvailable())
            return true;

        return false;
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

    IEnumerator HandlePlayerKO()
    {
        yield return StartCoroutine(
            textBox.ShowMessage($"{playerUnit.Instance.data.professorName} se debilitó")
        );

        BattleEvents.OnPlayerSwitchRequired?.Invoke();
    }

    IEnumerator HandleEnemyKO()
    {
        yield return StartCoroutine(
            textBox.ShowMessage($"{enemyUnit.Instance.data.professorName} se debilitó")
        );

        ProfemonInstance next =
            enemyParty.GetFirstAlive();

        if (next != null)
        {
            enemyUnit.InitializeFromInstance(next);

            yield return StartCoroutine(
                textBox.ShowMessage($"El enemigo envía {next.data.professorName}")
            );

            BattleEvents.OnActiveUnitChanged?.Invoke();
        }
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
}