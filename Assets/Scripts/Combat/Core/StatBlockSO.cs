using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Stats/Base Stats")]
public class StatBlockSO : ScriptableObject
{
    public StatEntry baseHealth;
    public StatEntry baseAttack;
    public StatEntry baseDefense;
    public StatEntry baseSpeed;
    public StatEntry baseAccuracy;
    public StatEntry baseEvasion;
}