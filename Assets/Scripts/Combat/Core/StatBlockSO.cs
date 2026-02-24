using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Stats/Base Stats")]
public class StatBlockSO : ScriptableObject
{
    public List<StatEntry> stats;
}