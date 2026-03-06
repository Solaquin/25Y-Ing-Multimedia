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
    public StatBlockSO baseStats;

    [Header("Movements")]
    public List<MoveSO> learnableMoves;

    [Header("Captura")]
    [Range(0, 100)]
    public int captureDifficulty = 50;
}