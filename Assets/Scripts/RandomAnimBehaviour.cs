using System.Collections.Generic;
using UnityEngine;

public class RandomAnimBehaviour : StateMachineBehaviour
{

    [System.Serializable]
    public class WeightedState
    {
        public int stateID;
        public float weight = 1f; // peso relativo
    }

    public string sm_ParameterName;
    public List<WeightedState> sm_States = new List<WeightedState>();

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (sm_States.Count > 0)
        {
            int selectedState = GetWeightedRandomState();
            animator.SetInteger(sm_ParameterName, selectedState);
        }
        else
        {
            Debug.LogWarning("No hay estados configurados");
        }
    }

    int GetWeightedRandomState()
    {
        float totalWeight = 0f;

        foreach (var state in sm_States)
            totalWeight += Mathf.Max(0f, state.weight);

        if (totalWeight <= 0f)
            return sm_States[0].stateID;

        float randomPoint = Random.value * totalWeight;

        float current = 0f;

        foreach (var state in sm_States)
        {
            current += Mathf.Max(0f, state.weight);

            if (randomPoint <= current)
                return state.stateID;
        }

        return sm_States[sm_States.Count - 1].stateID;
    }
}
