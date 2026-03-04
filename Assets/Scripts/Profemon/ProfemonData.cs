using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Profemon/Professor")]
public class ProfemonData : ScriptableObject
{
    [Header("Información General")]
    public string professorName;
    public string description;
    public Sprite image;

    [Header("Tipos")]
    public List<TypeSO> types;

    [Header("Base Stats")]
    public int baseHP = 100;
    public int baseAttack = 20;
    public int baseDefense = 10;
    public int baseSpeed = 10;

    [Header("Base Combat")]
    public int baseAccuracy = 100;
    public int baseEvasion = 100;

    [Header("Captura")]
    [Range(0, 100)]
    public int captureDifficulty = 50;
}