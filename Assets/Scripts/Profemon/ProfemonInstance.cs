using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProfemonInstance
{
    public ProfemonData data;

    public int level;

    // Vida
    public int maxHP;
    public int currentHP;

    // Stats reales
    public int attack;
    public int defense;
    public int speed;
    public int accuracy;
    public int evasion;

    // Tipos
    public List<TypeSO> types;

    // Movimientos actuales
    public List<MoveSO> currentMoves = new List<MoveSO>();

    // Status persistente
    public StatusInstance activeStatus;

    public int MaxHP => maxHP;

    public ProfemonInstance(ProfemonData baseData, int level)
    {
        data = baseData;
        this.level = level;

        GenerateStats();

        currentHP = maxHP;

        types = new List<TypeSO>(baseData.types);

        InitializeMoves();
    }

    private void GenerateStats()
    {
        maxHP = data.baseStats.baseHealth.value + level * 5;

        attack = data.baseStats.baseAttack.value + level * 2;
        defense = data.baseStats.baseDefense.value + level * 2;
        speed = data.baseStats.baseSpeed.value + level * 2;

        accuracy = data.baseStats.baseAccuracy.value;
        evasion = data.baseStats.baseEvasion.value;
    }

    private void InitializeMoves()
    {
        int maxMoves = Mathf.Min(4, data.learnableMoves.Count);

        for (int i = 0; i < maxMoves; i++)
        {
            currentMoves.Add(data.learnableMoves[i]);
        }
    }

    public bool IsAlive()
    {
        return currentHP > 0;
    }

    //Items

    public int HealHP(int amount)
    {
        int before = currentHP;
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        return currentHP - before;
    }

    public void Revive(int hpAmount)
    {
        if (IsAlive()) return;
        currentHP = Mathf.Clamp(hpAmount, 1, maxHP);
    }

    public bool HasStatusCondition()
    {
        return activeStatus != null;
    }

    public void CureStatusCondition()
    {
        activeStatus = null;
    }
}