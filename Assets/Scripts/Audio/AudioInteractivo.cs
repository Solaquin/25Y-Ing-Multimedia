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

    public enum CategoriaAudio
    {
        FX,
        Musica
    }

    [Header("Configuración")]
    public TipoAudio tipoAudio;

    [Header("Categoría")]
    public CategoriaAudio categoria = CategoriaAudio.FX;

    [Header("Volumen")]
    [Range(0f, 1f)]
    public float volumen = 1f;

    public static float GlobalFXVolume = 1f;
    public static float GlobalMusicVolume = 1f;

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
        ActualizarVolumen();

        if (tipoAudio == TipoAudio.Loop)
            audioSource.loop = true;
    }

    private void Update()
    {
        ActualizarVolumen();
    }

    private void ActualizarVolumen()
    {
        if (audioSource == null)
            return;

        if (categoria == CategoriaAudio.Musica)
        {
            audioSource.volume =
                volumen * GlobalMusicVolume;
        }
        else
        {
            audioSource.volume =
                volumen * GlobalFXVolume;
        }
    }

    public void ActivarAudio()
    {
        if (clips == null || clips.Length == 0)
            return;

        AudioClip clipSeleccionado = clips[0];

        if (tipoAudio == TipoAudio.Aleatorio ||
            tipoAudio == TipoAudio.AleatorioConPitch)
        {
            clipSeleccionado =
                clips[Random.Range(0, clips.Length)];
        }

        audioSource.clip = clipSeleccionado;

        if (tipoAudio == TipoAudio.AleatorioConPitch)
        {
            audioSource.pitch =
                Random.Range(pitchMin, pitchMax);
        }
        else
        {
            audioSource.pitch = 1f;
        }

        if (!audioSource.isPlaying ||
            tipoAudio != TipoAudio.Loop)
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