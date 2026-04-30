using UnityEngine;


public static class AudioSystem
{
    private static AudioSource sfxSource;
    private static AudioSource musicSource;
    private static AudioSource ambientSource;

    public static void Initialize(AudioSource sfx, AudioSource music, AudioSource ambient)
    {
        sfxSource = sfx;
        musicSource = music;
        ambientSource = ambient;
    }

    // SONIDO SIMPLE
    public static void Play(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    //  RANDOM
    public static void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0 || sfxSource == null) return;

        int index = Random.Range(0, clips.Length);
        sfxSource.PlayOneShot(clips[index]);
    }

    // RANDOM + PITCH
    public static void PlayRandomPitch(AudioClip[] clips, float minPitch, float maxPitch)
    {
        if (clips == null || clips.Length == 0 || sfxSource == null) return;

        int index = Random.Range(0, clips.Length);

        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(clips[index]);
        sfxSource.pitch = 1f;
    }

    // MÚSICA
    public static void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public static void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    // AMBIENTE
    public static void PlayAmbient(AudioClip clip)
    {
        if (ambientSource == null || clip == null) return;

        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public static void StopAmbient()
    {
        if (ambientSource != null)
            ambientSource.Stop();
    }
}


// AUDIO INITIALIZER

public class AudioInitializer : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioSource ambientSource;

    void Awake()
    {
        AudioSystem.Initialize(sfxSource, musicSource, ambientSource);
    }
}

// AMBIENT ZONE (ZONAS)

public class AmbientZone : MonoBehaviour
{
    public AudioClip sonidoAmbiente;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSystem.PlayAmbient(sonidoAmbiente);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSystem.StopAmbient();
        }
    }
}
