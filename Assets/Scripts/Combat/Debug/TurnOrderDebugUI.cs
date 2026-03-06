using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnOrderDebugUI : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    public void ShowTurnOrder(List<TurnAction> actions)
    {
        string text = "TURN ORDER\n";

        for (int i = 0; i < actions.Count; i++)
        {
            var action = actions[i];

            text +=
            $"{i + 1}. {action}\n";
        }

        debugText.text = text;
    }
}