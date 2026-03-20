using UnityEngine;

public class StatusInstance
{
    public StatusEffectSO effect;

    public int remainingTurns;

    public StatusInstance(StatusEffectSO effect, int turns)
    {

        // Asegurarse de que effect no sea null al construir
        if (effect == null)
            Debug.LogError("StatusInstance creado con effect null");

        this.effect = effect;
        this.remainingTurns = turns;
    }
}