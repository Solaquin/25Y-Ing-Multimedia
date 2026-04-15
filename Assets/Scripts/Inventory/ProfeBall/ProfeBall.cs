using System.Collections;
using UnityEngine;

public class Profebola : MonoBehaviour
{
    [Tooltip("SO que define este tipo de bola. Debe coincidir con la que se consumio del inventario.")]
    public ProfeBallSO datos;

    private bool isProcessing = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isProcessing) return;

        Profemon professor = collision.collider.GetComponentInParent<Profemon>();

        if (professor != null && !professor.isCaptured)
        {
            StartCoroutine(CaptureSequence(professor));
        }
    }

    IEnumerator CaptureSequence(Profemon professor)
    {
        isProcessing = true;

        // Verificar stock antes de proceder
        if (datos == null || !ItemInventory.Instance.HasItem(datos.id))
        {
            Debug.LogWarning("[Profebola] Sin stock o sin ProfeBallSO asignado.");
            Destroy(gameObject);
            yield break;
        }

        // Consumir del inventario
        ItemInventory.Instance.ConsumeItem(datos.id);

        // Ocultar Profemon (entra a la bola)
        professor.HideProfessor();

        // --- SONIDO DE TEMBLOR ---
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySound(AudioManager.instance.clipTemblor, transform.position);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Simular temblor
        float shakeTime = 2f;
        float elapsed = 0f;
        float timer = 0f;
        Vector3 originalPos = transform.position;

        while (elapsed < shakeTime)
            while (timer < shakeTime)
            {
                transform.position = originalPos + Random.insideUnitSphere * 0.05f;
                elapsed += Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }

        transform.position = originalPos;

        // Probabilidad de captura la dificultad base se reduce con el captureBonus del SO
        int dificultad = datos != null
            ? Mathf.Max(0, professor.data.captureDifficulty - datos.captureBonus)
            : professor.data.captureDifficulty;

        int roll = Random.Range(0, 100);

        if (roll > dificultad)
            if (roll > professor.data.captureDifficulty)
            {
                // --- SONIDO DE �XITO ---
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySound(AudioManager.instance.clipCapturado, transform.position);

                professor.ConfirmCapture();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("[Profebola] El profesor escap�!");
                // --- SONIDO DE FALLO ---
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySound(AudioManager.instance.clipEscapado, transform.position);

                Debug.Log("El profesor escap�!");
                professor.ShowProfessor();
                Destroy(gameObject);
            }
    }
}