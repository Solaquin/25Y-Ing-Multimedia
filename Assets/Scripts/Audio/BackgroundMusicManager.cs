using System.Collections;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Lista de canciones")]
    public AudioClip[] musicTracks;

    [Header("Tiempo entre canciones")]
    public float minDelay = 30f;
    public float maxDelay = 90f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        //  evitar que se destruya entre escenas
        DontDestroyOnLoad(gameObject);

        //  asegurar que sea audio 2D (global)
        audioSource.spatialBlend = 0f;

        // opcional pero recomendado
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Start()
    {
        StartCoroutine(PlayMusicRoutine());
    }

    IEnumerator PlayMusicRoutine()
    {
        while (true)
        {
            // Espera tiempo random
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            if (musicTracks == null || musicTracks.Length == 0)
                yield break;

            // Elegir canción random
            AudioClip clip = musicTracks[Random.Range(0, musicTracks.Length)];

            audioSource.clip = clip;
            audioSource.Play();

            // Esperar a que termine
            yield return new WaitForSeconds(clip.length);
        }
    }
}
