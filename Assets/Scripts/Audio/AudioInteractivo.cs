using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioInteractivo : MonoBehaviour
{
    public enum TipoAudio
    {
        UnaVez,
        Loop,
        Aleatorio,
        AleatorioConPitch
    }

    [Header("Configuración")]
    public TipoAudio tipoAudio;

    [Header("Clips de audio")]
    public AudioClip[] clips;

    private AudioSource audioSource;

    [Header("Pitch aleatorio")]
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (tipoAudio == TipoAudio.Loop)
        {
            audioSource.loop = true;
        }
    }

    public void ActivarAudio()
    {
        if (clips.Length == 0) return;

        AudioClip clipSeleccionado = clips[0];

        if (tipoAudio == TipoAudio.Aleatorio || tipoAudio == TipoAudio.AleatorioConPitch)
        {
            int index = Random.Range(0, clips.Length);
            clipSeleccionado = clips[index];
        }

        audioSource.clip = clipSeleccionado;

        if (tipoAudio == TipoAudio.AleatorioConPitch)
        {
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
        }
        else
        {
            audioSource.pitch = 1f;
        }

        if (!audioSource.isPlaying || tipoAudio != TipoAudio.Loop)
        {
            audioSource.Play();
        }
    }

    public void DesactivarAudio()
    {
        if (tipoAudio == TipoAudio.Loop)
        {
            audioSource.Stop();
        }
    }
}
