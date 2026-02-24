using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TypeSO", menuName = "Combat/Type")]
public class TypeSO : ScriptableObject
{
    [Header("Logical ID")]
    public ProfessorTypeID ProfessorTypeID;

    [Header("UI")]
    public string fullName;
    public string shortName;
    public Sprite icon;
    public Color typeColor;

    [Header("Offensive Effectiveness")]
    public List<TypeEffectiveness> effectiveness;
}

[System.Serializable]
public struct TypeEffectiveness
{
    public TypeSO targetType;
    public float multiplier;
}