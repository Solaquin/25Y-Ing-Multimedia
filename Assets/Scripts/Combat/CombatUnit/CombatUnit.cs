using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [Header("Setup")]
    public ProfemonData data;
    public int level = 1;

    private ProfemonInstance instance;
    public ProfemonInstance Instance => instance;

    Dictionary<StatType, int> statStages = new Dictionary<StatType, int>();
    StatusInstance currentStatus;

    [SerializeField] int currentHPDebug;

    private void Awake()
    {
        instance = new ProfemonInstance(data, level);

        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            statStages[stat] = 0;
        }

        currentHPDebug = instance.currentHP;

        Debug.Log($"{name} Attack Base: {instance.attack}");
        Debug.Log($"{name} Defense Base: {instance.defense}");
        Debug.Log($"{name} Speed Base: {instance.speed}");
        Debug.Log($"{name} Accuracy Base: {instance.accuracy}");
        Debug.Log($"{name} Evasion Base: {instance.evasion}");
    }

    // ================================
    // VIDA
    // ================================

    public void TakeDamage(int amount)
    {
        instance.currentHP -= amount;
        instance.currentHP = Mathf.Clamp(
            instance.currentHP,
            0,
            instance.maxHP
        );

        BattleEvents.OnHPChanged?.Invoke();

        Debug.Log($"{name} recibió {amount} de daño. HP: {instance.currentHP}");
        currentHPDebug = instance.currentHP;
    }

    public void Heal(int amount)
    {
        instance.currentHP += amount;
        instance.currentHP = Mathf.Clamp(
            instance.currentHP,
            0,
            instance.maxHP
        );

        BattleEvents.OnHPChanged?.Invoke();

        Debug.Log($"{name} se curó {amount}. New HP:{instance.currentHP}");
        currentHPDebug = instance.currentHP;
    }

    public bool IsAlive()
    {
        return instance.currentHP > 0;
    }

    public int GetCurrentHP()
    {
        return instance.currentHP;
    }

    public int GetMaxHP()
    {
        return instance.maxHP;
    }

    // ================================
    // STATS
    // ================================

    int GetBaseStat(StatType stat)
    {
        switch (stat)
        {
            case StatType.Attack: return instance.attack;
            case StatType.Defense: return instance.defense;
            case StatType.Speed: return instance.speed;
            case StatType.Accuracy: return instance.accuracy;
            case StatType.Evasion: return instance.evasion;
        }

        return 0;
    }

    public int GetStat(StatType stat)
    {
        int baseValue = GetBaseStat(stat);

        int stage = statStages[stat];

        float multiplier = GetStageMultiplier(stage);

        return Mathf.RoundToInt(baseValue * multiplier);
    }

    public void AddStageModifier(StatType stat, int amount)
    {
        int currentStage = statStages[stat];

        currentStage += amount;

        currentStage = Mathf.Clamp(currentStage, -6, 6);

        statStages[stat] = currentStage;

        Debug.Log($"{name} {stat} stage ahora es {currentStage}");
    }

    public int GetStage(StatType stat)
    {
        return statStages[stat];
    }

    float GetStageMultiplier(int stage)
    {
        if (stage >= 0)
            return (2f + stage) / 2f;

        return 2f / (2f - stage);
    }

    public void ResetStages()
    {
        statStages.Clear();

        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            statStages[stat] = 0;
        }
    }

    // ================================
    // ESTADOS
    // ================================
    public void ApplyStatus(StatusEffectSO status, int duration)
    {
        if (currentStatus != null)
        {
            Debug.Log($"{name} ya tiene un estado.");
            return;
        }

        currentStatus = new StatusInstance(status, duration);

        status.OnApply(this);

        Debug.Log($"{name} ahora tiene {status.statusType}");
    }

    public bool TryPreventAction(BattleActionType actionType, out string message)
    {
        message = "";

        if (currentStatus == null)
            return false;

        if (currentStatus.effect.PreventAction(actionType))
        {
            message =
                $"{name} está {currentStatus.effect.statusType} y no puede moverse.";

            return true;
        }

        return false;
    }

    public void TickStatus()
    {
        if (currentStatus == null)
            return;

        currentStatus.effect.OnTurnEnd(this);

        currentStatus.remainingTurns--;

        if (currentStatus.remainingTurns <= 0)
        {
            Debug.Log($"{name} ya no está {currentStatus.effect.statusType}");
            currentStatus = null;
        }
    }

    // ================================
    // TIPOS
    // ================================

    public float GetTypeMultiplier(TypeSO attackType)
    {
        float multiplier = 1f;

        foreach (var defenseType in instance.types)
        {
            multiplier *= TypeChart.Instance.GetMultiplier(
                attackType,
                defenseType
            );
        }

        return multiplier;
    }

    // ================================
    // MOVIMIENTOS
    // ================================
    public List<MoveSO> GetMoves()
    {
        return instance.currentMoves;
    }

    public MoveSO GetRandomMove()
    {
        var moves = instance.currentMoves;

        if (moves == null || moves.Count == 0)
            return null;

        return moves[Random.Range(0, moves.Count)];
    }

}