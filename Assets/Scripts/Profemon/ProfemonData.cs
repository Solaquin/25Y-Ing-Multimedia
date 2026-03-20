using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Profemon/Professor")]
public class ProfemonData : ScriptableObject
{
    [Header("Información General")]
    public string professorName;

    [TextArea(2, 4)]
    public string phrase;

    [TextArea(3, 6)]
    public string description;

    [TextArea(3, 6)]
    public string anecdote;

    [TextArea(3, 6)]
    public string profile;

    public string whereToFind;
    public Sprite whereToFindImage;

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