using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [Header("Setup")]
    public ProfemonData data;
    public int level = 1;

    private ProfemonInstance instance;

    private List<StatModifier> activeModifiers =
        new List<StatModifier>();

    [SerializeField] int currentHPDebug;

    private void Awake()
    {
        instance = new ProfemonInstance(data, level);
        currentHPDebug = instance.currentHP;

        Debug.Log($"{name} Attack Base: {instance.attack}");
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

    public int GetStat(StatType stat)
    {
        int baseValue = 0;

        switch (stat)
        {
            case StatType.Attack:
                baseValue = instance.attack;
                break;

            case StatType.Defense:
                baseValue = instance.defense;
                break;

            case StatType.Speed:
                baseValue = instance.speed;
                break;

            case StatType.Accuracy:
                baseValue = instance.accuracy;
                break;

            case StatType.Evasion:
                baseValue = instance.evasion;
                break;
        }

        int modifierSum = 0;

        foreach (var mod in activeModifiers)
        {
            if (mod.stat == stat)
                modifierSum += mod.amount;
        }

        return baseValue + modifierSum;
    }

    public void AddModifier(StatType stat, int amount, int duration)
    {
        StatModifier modifier =
            new StatModifier(stat, amount, duration);

        activeModifiers.Add(modifier);

        Debug.Log($"{name} recibió {stat} {amount} por {duration} turnos.");
    }

    public void TickModifiers()
    {
        for (int i = activeModifiers.Count - 1; i >= 0; i--)
        {
            activeModifiers[i].remainingTurns--;

            if (activeModifiers[i].remainingTurns <= 0)
                activeModifiers.RemoveAt(i);
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
}