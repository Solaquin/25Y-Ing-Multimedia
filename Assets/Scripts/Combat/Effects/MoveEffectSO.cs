using UnityEngine;

[CreateAssetMenu(fileName = "MoveEffectSO", menuName = "Combat/MoveEffect")]
public abstract class MoveEffectSO : ScriptableObject
{
    public abstract void Execute(CombatUnit user, CombatUnit target, MoveContext context);
}
