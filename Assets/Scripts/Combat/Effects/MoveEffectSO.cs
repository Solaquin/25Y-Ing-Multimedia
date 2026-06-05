using System.Collections.Generic;
using UnityEngine;

public enum VisualPhase
{
    BeforeEffect,
    AfterEffect
}

[System.Serializable]
public class VisualEvent
{
    public string animTag;              // "Attack", "Hit", "Cast", etc.
    public AudioClip[] sfx;
    public GameObject[] vfx;

    [Tooltip("Se ejecuta sobre user o target")]
    public bool onTarget;

    public VisualPhase phase;
}

[CreateAssetMenu(fileName = "MoveEffectSO", menuName = "Combat/MoveEffect")]
public abstract class MoveEffectSO : ScriptableObject
{
    [Header("Visual Events (ordenados)")]
    public List<VisualEvent> visualEvents;

    public abstract void Execute(CombatUnit user, CombatUnit target, MoveContext context);
}
