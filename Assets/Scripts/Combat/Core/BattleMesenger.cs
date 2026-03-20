using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Lado Controller del sistema de mensajes.
/// Emite OnMessageRequested y suspende la coroutine hasta que la View confirma.
/// BattleSystem, BattleItemUsageHandler y cualquier otro Controller lo usan
/// en lugar de llamar textBox directamente.
/// </summary>
public static class BattleMessenger
{
    public static IEnumerator Show(string message, float postDelay = 0.5f)
    {
        if (BattleEvents.OnMessageRequested == null)
        {
            Debug.LogWarning($"[BattleMessenger] Sin suscriptores. Mensaje ignorado: {message}");
            yield break;
        }

        bool done = false;
        BattleEvents.OnMessageRequested.Invoke(message, () => done = true);
        yield return new WaitUntil(() => done);

        if (postDelay > 0f)
            yield return new WaitForSeconds(postDelay);
    }
}