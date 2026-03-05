using System;
using System.Collections.Generic;

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

    public ProfemonInstance(ProfemonData baseData, int level)
    {
        data = baseData;
        this.level = level;

        GenerateStats();

        currentHP = maxHP;

        // Copiamos los tipos del Data
        types = new List<TypeSO>(baseData.types);
    }

    private void GenerateStats()
    {
        maxHP = data.baseHP + level * 5;

        attack = data.baseAttack + level * 2;
        defense = data.baseDefense + level * 2;
        speed = data.baseSpeed + level * 2;

        accuracy = data.baseAccuracy;
        evasion = data.baseEvasion;
    }
}