using UnityEngine;
using System.Collections;

public class Profebola : MonoBehaviour
{
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

        // Ocultar profesor (entra a la bola)
        professor.HideProfessor();

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Simular temblor
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

        // Probabilidad de captura
        int roll = Random.Range(0, 100);

        if (roll > professor.data.captureDifficulty)
        {
            professor.ConfirmCapture();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("El profesor escapó!");

            professor.ShowProfessor();
            Destroy(gameObject);
        }

    }

}