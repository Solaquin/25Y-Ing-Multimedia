using UnityEngine;

[CreateAssetMenu(fileName = "MoveSO", menuName = "Combat/Move")]
public class MoveSO : ScriptableObject
{
    [Header("Basic Info")]
    public string moveName;
    public TypeSO moveType;

    [Header("Combat")]
    public int power;
    public int accuracy;
    public int maxPP;

    [Header("Crit")]
    public float baseCritChance = 5f;

    [Header("Behaviour")]
    public MoveEffectSO effect;

    [Header("Effect Parameters")]
    public StatType affectedStat;
    public int amount;
    public int duration;
    public int healAmount;
}
