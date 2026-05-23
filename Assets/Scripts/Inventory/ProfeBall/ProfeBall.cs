using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Profebola : MonoBehaviour
{
    [Tooltip("SO que define este tipo de bola. Debe coincidir con la que se consumió del inventario.")]
    public ProfeBallSO datos;

    [Header("Audio")]
    public AudioInteractivo sonidoTemblor;
    public AudioInteractivo sonidoCaptura;
    public AudioInteractivo sonidoFallo;

    [Header("Auto-destrucción")]
    public float tiempoEnSuelo = 5f;

    private bool isProcessing = false;
    private bool tocóSuelo = false;

    [SerializeField]
    private NotificationSO captureNotification;

    private void OnCollisionEnter(Collision collision)
    {
        if (isProcessing) return;

        // Intento de captura de Profemon
        Profemon professor = collision.collider.GetComponentInParent<Profemon>();
        if (professor != null && !professor.isCaptured)
        {
            StartCoroutine(CaptureSequence(professor));
            return;
        }

        // Tocó el suelo (cualquier cosa que no sea un Profemon)
        if (!tocóSuelo)
        {
            tocóSuelo = true;
            StartCoroutine(AutoDestruir());
        }
    }

    IEnumerator AutoDestruir()
    {
        yield return new WaitForSeconds(tiempoEnSuelo);
        if (!isProcessing)
            Destroy(gameObject);
    }

    IEnumerator CaptureSequence(Profemon professor)
    {
        isProcessing = true;

        if (datos == null)
        {
            Debug.LogWarning("[Profebola] Sin ProfeBallSO asignado.");
            Destroy(gameObject);
            yield break;
        }

        professor.HideProfessor();

        if (sonidoTemblor != null)
            AudioManager.Play(sonidoTemblor, transform.position);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        float shakeTime = 2f;
        float elapsed = 0f;
        Vector3 originalPos = transform.position;

        while (elapsed < shakeTime)
        {
            transform.position = originalPos + Random.insideUnitSphere * 0.05f;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPos;

        int dificultad = datos != null
            ? Mathf.Max(0, professor.data.captureDifficulty - datos.captureBonus)
            : professor.data.captureDifficulty;

        int roll = Random.Range(0, 100);
        if (roll > dificultad)
        {
            if (sonidoCaptura != null)
                AudioManager.Play(sonidoCaptura, transform.position);

            professor.ConfirmCapture();
            Destroy(gameObject);
        }
        else
        {

            NotificationManager.Send("¡El profesor escapó!");
            if (sonidoFallo != null)
                AudioManager.Play(sonidoFallo, transform.position);

            professor.ShowProfessor();
            Destroy(gameObject);
        }
    }
}