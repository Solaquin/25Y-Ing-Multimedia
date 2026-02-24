using System.Collections.Generic;
using UnityEngine;

public class TypeChart : MonoBehaviour
{
    public static TypeChart Instance;

    private Dictionary<(TypeSO, TypeSO), float> chart = new Dictionary<(TypeSO, TypeSO), float>();

    [SerializeField] private List<TypeSO> allTypes;

    private void Awake()
    {
        Instance = this;
        BuildChart();
    }

    private void BuildChart()
    {
        chart.Clear();

        foreach (var attackType in allTypes)
        {
            foreach (var eff in attackType.effectiveness)
            {
                chart[(attackType, eff.targetType)] =
                    eff.multiplier;
            }
        }
    }    

    public float GetMultiplier(TypeSO attack, TypeSO target)
    {
        if (chart.TryGetValue((attack, target), out float value))
        {
            return value;
        }

        return 1f;
    }
}
