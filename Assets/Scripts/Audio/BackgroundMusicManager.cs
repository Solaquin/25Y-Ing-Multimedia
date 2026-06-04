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

        DontDestroyOnLoad(gameObject);

        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Start()
    {
        StartCoroutine(PlayMusicRoutine());
    }

    void Update()
    {
        if (audioSource != null)
        {
            audioSource.volume = AudioInteractivo.GlobalMusicVolume;
        }
    }

    IEnumerator PlayMusicRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            if (musicTracks == null || musicTracks.Length == 0)
                yield break;

            AudioClip clip =
                musicTracks[Random.Range(0, musicTracks.Length)];

            audioSource.clip = clip;
            audioSource.volume = AudioInteractivo.GlobalMusicVolume;
            audioSource.Play();

            yield return new WaitForSeconds(clip.length);
        }
    }
}
