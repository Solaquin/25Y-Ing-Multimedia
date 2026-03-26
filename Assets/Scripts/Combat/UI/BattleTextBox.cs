using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// View: muestra mensajes en pantalla.
/// Se suscribe a BattleEvents.OnMessageRequested y llama el callback
/// cuando termina de mostrar el texto, desbloqueando al Controller.
/// </summary>
public class BattleTextBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float charDelay = 0.03f;

    void OnEnable()
    {
        BattleEvents.OnMessageRequested += HandleMessageRequested;
    }

    void OnDisable()
    {
        BattleEvents.OnMessageRequested -= HandleMessageRequested;
    }

    void HandleMessageRequested(string message, Action onDone)
    {
        StartCoroutine(ShowMessage(message, onDone));
    }

    IEnumerator ShowMessage(string message, Action onDone)
    {
        messageText.text = "";

        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        // Terminó de escribir - notificar al Controller
        // El Controller decide cuánto esperar después (postDelay en BattleMessenger)
        onDone?.Invoke();
    }
}