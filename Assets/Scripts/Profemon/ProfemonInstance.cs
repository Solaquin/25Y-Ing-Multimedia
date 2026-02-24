using System;

[System.Serializable]
public class ProfemonInstance
{
    public ProfemonData data;

    public int level;
    public int currentHP;

    public int attack;
    public int defense;
    public int speed;

    public ProfemonInstance(ProfemonData baseData)
    {
        data = baseData;

        level = 1;

        attack = baseData.baseAttack;
        defense = baseData.baseDefense;
        speed = baseData.baseSpeed;

        currentHP = baseData.baseHP;
    }
}