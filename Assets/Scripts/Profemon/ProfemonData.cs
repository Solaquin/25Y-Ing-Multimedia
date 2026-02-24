using UnityEngine;

[CreateAssetMenu(menuName = "Profemon/Professor")]
public class ProfemonData : ScriptableObject
{
    [Header("Información General")]
    public string professorName;

    [TextArea(3, 5)]
    public string description;

    public Sprite image;

    [Range(0, 100)]
    public int captureDifficulty = 50;

    [Header("Base Stats")]
    public int baseHP = 100;
    public int baseAttack = 20;
    public int baseDefense = 10;
    public int baseSpeed = 10;
}