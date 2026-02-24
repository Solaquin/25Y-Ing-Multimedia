using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [Header("Setup")]
    public List<TypeSO> types;

    [Header("Stats")]
    public int maxHP;
    public StatBlockSO baseStatsSO;
    public StatBlock baseStats;

    [SerializeField]private int currentHP;

    private List<StatModifier> activeModifiers =
    new List<StatModifier>();

    private void Awake()
    {
        currentHP = maxHP;

        baseStats.InitializeFromSO(baseStatsSO);

        Debug.Log($"{name} Accuracy Base: " + baseStats.GetStat(StatType.Accuracy));

    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log($"{name} recibió {amount} de daño. HP: {currentHP}");
    }

    public bool IsAlive()
    {
        return currentHP > 0;
    }

    public float GetTypeMultiplier(TypeSO attackType)
    {
        float multiplier = 1f;

        foreach (var defenseType in types)
        {
            multiplier *= TypeChart.Instance.GetMultiplier(
                attackType,
                defenseType
            );
        }

        return multiplier;
    }

    public void AddModifier(StatType stat, int amount, int duration)
    {
        StatModifier modifier =
        new StatModifier(stat, amount, duration);

        activeModifiers.Add(modifier);

        Debug.Log($"{name} recibió {stat} {amount} por {duration} turnos, actual stat: {GetStat(stat)}");
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    public int GetStat(StatType stat)
    {
        int baseValue = baseStats.GetStat(stat);

        int modifierSum = 0;

        foreach (var mod in activeModifiers)
        {
            if (mod.stat == stat)
                modifierSum += mod.amount;
        }

        return baseValue + modifierSum;
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
}
