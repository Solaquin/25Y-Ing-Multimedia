using UnityEngine;

[CreateAssetMenu(fileName = "MoveSO", menuName = "Combat/Move")]
public class MoveSO : ScriptableObject
{
    [Header("Basic Info")]
    public string moveName;
    public TypeSO moveType;
    [TextArea]
    public string moveDescription;

    [Header("Combat")]
    public int power;
    public int accuracy;
    public int maxPP;

    [Header("Turn Order")]
    public int priority = 0;

    [Header("Crit")]
    public float baseCritChance = 5f;

    [Header("Behaviour")]
    public MoveEffectSO effect;

    [Header("Stat Effect")]
    public StatType affectedStat;
    public int stageChange;

    [Header("Healing")]
    public int healPercent;

    [Header("Status Effect")]
    public StatusEffectSO statusEffect;
    public int duration;
}
