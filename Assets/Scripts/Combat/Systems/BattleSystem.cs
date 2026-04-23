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
    [Header("Audio Combate")]
    public AudioInteractivo musicaCombate;
    public AudioInteractivo audioGolpe;
    public TurnOrderDebugUI debugUI;
    public BattleState currentState;
    public int turnNumber = 0;

    public List<CombatUnit> allUnits = new List<CombatUnit>();
    bool playerCommandSelected = false;
    BattleCommand playerCommand;
    BattleCommand enemyCommand;


    NPCParty enemyParty;

    public CombatUnit playerUnit;
    public CombatUnit enemyUnit;



    // Handler items
    [SerializeField] private BattleItemUsageHandler itemUsageHandler;

    public void SetupBattle(NPCParty enemyParty)
    {
        this.enemyParty = enemyParty;

        turnNumber = 0;
        ProfemonInstance playerInstance =
            PlayerPartyManager.Instance.GetFirstAlive();

        ProfemonInstance enemyInstance =
            enemyParty.GetFirstAlive();

        SpawnUnitsEmpty();

        StartCoroutine(StartBattleRoutine(playerInstance, enemyInstance));
    }

    void SpawnUnitsEmpty()
    {
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
        if (musicaCombate != null) musicaCombate.ActivarAudio();

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
                yield return StartCoroutine(EndBattle());
                yield break;
            }
        }
    }

    IEnumerator PlayerInputPhase()
    {
        currentState = BattleState.PlayerInput;

        playerCommandSelected = false;

        yield return StartCoroutine(
                BattleMessenger.Show($"Selecciona una acción")
        );

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

        if (!playerUnit.IsAlive() && !playerUnit.HasBeenKO)
        {
            yield return StartCoroutine(HandleUnitKO(playerUnit));
            yield break;
        }

        if (!enemyUnit.IsAlive() && !enemyUnit.HasBeenKO)
        {
            yield return StartCoroutine(HandleUnitKO(enemyUnit));
            yield break;
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void StartTurn()
    {
        turnNumber++;

        Debug.Log($"---- TURN {turnNumber} START ----");

        BattleEvents.OnTurnStart?.Invoke();
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

            if (!playerUnit.IsAlive() && !playerUnit.HasBeenKO)
            {
                yield return StartCoroutine(HandleUnitKO(playerUnit));
                yield break;
            }

            if (!enemyUnit.IsAlive() && !enemyUnit.HasBeenKO)
            {
                yield return StartCoroutine(HandleUnitKO(enemyUnit));
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
                BattleMessenger.Show(statusMessage)
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

                yield return StartCoroutine(
                    itemUsageHandler.ExecuteItemAction(action)
                );

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
            BattleMessenger.Show(
                $"{user.Instance.data.professorName} usó {move.moveName}"
            )
        );

        if (!target.IsAlive())
            yield break;

        if (!CheckAccuracy(user, target, move))
        {
            yield return StartCoroutine(
                BattleMessenger.Show("El ataque falló")
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

        // BEFORE (animación ataque / cast)
        yield return StartCoroutine(
            user.PlayVisualPhase(
                move.effect.visualEvents,
                VisualPhase.BeforeEffect,
                user,
                target
            )
        );

        // LÓGICA
        move.effect.Execute(user, target, context);

        if (audioGolpe != null)
            audioGolpe.ActivarAudio();

        // AFTER (impacto / hit / efectos)
        yield return StartCoroutine(
            user.PlayVisualPhase(
                move.effect.visualEvents,
                VisualPhase.AfterEffect,
                user,
                target
            )
        );



        if (isCritical)
        {
            yield return StartCoroutine(
                BattleMessenger.Show("¡Golpe crítico!")
            );
        }

        yield return StartCoroutine(ShowEffectivenessMessage(typeMultiplier));

        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator SwitchUnit(TurnAction action)
    {
        CombatUnit unit = action.user;
        ProfemonInstance newInstance = action.switchTarget;

        yield return StartCoroutine(
            BattleMessenger.Show($"{unit.Instance.data.professorName} vuelve.")
        );

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(unit.SwapProfemon(newInstance));

        yield return StartCoroutine(
            BattleMessenger.Show($"{newInstance.data.professorName} entra al combate.")
        );

        BattleEvents.OnActiveUnitChanged?.Invoke();

        yield return new WaitForSeconds(0.5f);
    }

    //Comandos jugador

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
        if (currentState != BattleState.PlayerInput)
            return;

        playerCommand =
            BattleCommand.CreateSwitchCommand(playerUnit, instance);

        playerCommandSelected = true;
    }

    /// <summary>
    /// El jugador elige usar un item sobre una instancia de su equipo.
    /// itemTarget: la ProfemonInstance a la que se aplica el item.
    /// </summary>
    public void PlayerChooseItem(BattleItemSO item, ProfemonInstance itemTarget)
    {
        if (currentState != BattleState.PlayerInput)
            return;

        playerCommand =
            BattleCommand.CreateItemCommand(playerUnit, item, itemTarget);

        playerCommandSelected = true;
    }

    // Utilidades

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

    IEnumerator EndBattle()
    {
        currentState = BattleState.EndBattle;

        bool playerWon = allUnits[0].IsAlive();


        if (musicaCombate != null) musicaCombate.DesactivarAudio();

        yield return StartCoroutine(
            BattleMessenger.Show(playerWon ? "Player wins" : "Enemy wins")
        );


        StopAllCoroutines();

        if (BattleTransitionManager.Instance != null)
        {
            yield return StartCoroutine(
                BattleTransitionManager.Instance.EndBattleTransition(playerWon)
            );
        }
    }

    IEnumerator HandleUnitKO(CombatUnit unit)
    {
        currentState = BattleState.Busy;

        unit.MarkAsKO();

        // Mensaje KO
        yield return StartCoroutine(
            BattleMessenger.Show($"{unit.Instance.data.professorName} se debilitó")
        );

        // Animación de derrota
        yield return StartCoroutine(unit.PlayFaint());

        // =========================
        // PLAYER
        // =========================
        if (unit == playerUnit)
        {
            if (!PlayerPartyManager.Instance.HasAvailable())
            {
                EndBattle();
                yield break;
            }

            BattleEvents.OnPlayerSwitchRequired?.Invoke();

            currentState = BattleState.PlayerInput;

            playerCommandSelected = false;

            yield return new WaitUntil(() => playerCommandSelected);

            currentState = BattleState.Busy;

            TurnAction switchAction = CommandResolver.CreateAction(playerCommand);

            // mensaje entrada
            yield return StartCoroutine(
                BattleMessenger.Show($"{switchAction.switchTarget.data.professorName} entra al combate")
            );

            // SIEMPRE SwapProfemon
            yield return StartCoroutine(
                playerUnit.SwapProfemon(switchAction.switchTarget)
            );

            BattleEvents.OnActiveUnitChanged?.Invoke();
        }

        // =========================
        // ENEMY
        // =========================
        else if (unit == enemyUnit)
        {
            if (!enemyParty.HasAvailable())
            {
                EndBattle();
                yield break;
            }

            ProfemonInstance next = enemyParty.GetFirstAlive();

            // mensaje entrada
            yield return StartCoroutine(
                BattleMessenger.Show($"El enemigo envía a {next.data.professorName}")
            );

            // CAMBIO IMPORTANTE (antes era InitializeFromInstance)
            yield return StartCoroutine(
                enemyUnit.SwapProfemon(next)
            );

            BattleEvents.OnActiveUnitChanged?.Invoke();
        }
    }

    IEnumerator ShowEffectivenessMessage(float multiplier)
    {
        if (multiplier >= 1.5f)
        {
            yield return StartCoroutine(
                BattleMessenger.Show("¡Es super efectivo!")
            );
        }
        else if (multiplier > 0f && multiplier < 1f)
        {
            yield return StartCoroutine(
                BattleMessenger.Show("No es muy efectivo...")
            );
        }
        else if (multiplier == 0f)
        {
            yield return StartCoroutine(
                BattleMessenger.Show("No tuvo efecto...")
            );
        }
    }

    IEnumerator StartBattleRoutine(ProfemonInstance playerInstance, ProfemonInstance enemyInstance)
    {
        yield return StartCoroutine(
            BattleIntro(playerInstance, enemyInstance)
        );

        StartBattle();
    }

    IEnumerator BattleIntro(ProfemonInstance playerInstance, ProfemonInstance enemyInstance)
    {
        // Mensaje inicial (opcional)
        yield return StartCoroutine(
            BattleMessenger.Show("¡Un combate comienza!")
        );

        yield return new WaitForSeconds(0.3f);

        // Enemy entra primero (estilo Pokémon)
        yield return StartCoroutine(
            BattleMessenger.Show($"{enemyInstance.data.professorName} aparece.")
        );

        yield return StartCoroutine(
            enemyUnit.SwapProfemon(enemyInstance, true)
        );

        yield return new WaitForSeconds(0.3f);

        // Player entra
        yield return StartCoroutine(
            BattleMessenger.Show($"Adelante {playerInstance.data.professorName}!")
        );

        yield return StartCoroutine(
            playerUnit.SwapProfemon(playerInstance, true)
        );

        yield return new WaitForSeconds(0.5f);
    }

    public void RunBattleTest()
    {
        StartBattle();
    }

    public void CleanupBattle()
    {
        foreach (var unit in allUnits)
        {
            unit.ResetStages();
            unit.ClearVisual();
        }
    }
}