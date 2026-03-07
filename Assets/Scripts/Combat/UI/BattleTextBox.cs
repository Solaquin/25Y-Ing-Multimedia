using UnityEngine;
using TMPro;
using System.Collections;

public class BattleTextBox : MonoBehaviour
{
    public TextMeshProUGUI battleText;

    public IEnumerator ShowMessage(string message)
    {
        battleText.text = message;

        yield return new WaitForSeconds(1.5f);
    }
}