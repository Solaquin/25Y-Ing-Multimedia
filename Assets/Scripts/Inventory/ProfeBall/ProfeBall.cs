using UnityEngine;
using System.Collections;

public class Profebola : MonoBehaviour
{
    private bool isProcessing = false;

    public BallType ballType;

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
        professor.HideProfessor();

        // --- SONIDO DE TEMBLOR ---
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySound(AudioManager.instance.clipTemblor, transform.position);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        float shakeTime = 2f;
        float timer = 0f;
        Vector3 originalPos = transform.position;

        while (timer < shakeTime)
        {
            transform.position = originalPos + Random.insideUnitSphere * 0.05f;
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        int roll = Random.Range(0, 100);

        if (roll > professor.data.captureDifficulty)
        {
            // --- SONIDO DE ÉXITO ---
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound(AudioManager.instance.clipCapturado, transform.position);

            professor.ConfirmCapture();
            Destroy(gameObject);
        }
        else
        {
            // --- SONIDO DE FALLO ---
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound(AudioManager.instance.clipEscapado, transform.position);

            Debug.Log("El profesor escapó!");
            professor.ShowProfessor();
            Destroy(gameObject);
        }
    }
}