using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Profedex/Database")]
public class ProfedexDatabase : ScriptableObject
{
    public List<ProfemonData> allProfemons;
}