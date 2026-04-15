using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton para acceder desde cualquier lugar
    public static AudioManager instance;

    [Header("Clips de Captura")]
    public AudioClip clipCapturado;
    public AudioClip clipEscapado;
    public AudioClip clipTemblor;

    [Header("Diálogos Retro")]
    public AudioClip clipDialogo8bit;

    private AudioSource voiceSource;

    private void Start()
    {
        // Creamos un AudioSource dinámicamente para la voz
        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.clip = clipDialogo8bit;
        voiceSource.loop = true; // El secreto está aquí
        voiceSource.playOnAwake = false;
    }

    public void StartVoice()
    {
        if (!voiceSource.isPlaying) voiceSource.Play();
    }

    public void StopVoice()
    {
        voiceSource.Stop();
    }

    [Range(0f, 1f)]
    public float volumenGeneral = 1f;

    private void Awake()
    {
        // Configuración del Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: para que no se borre entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para reproducir sonidos en una posición 3D
    public void PlaySound(AudioClip clip, Vector3 posicion)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, posicion, volumenGeneral);
        }
    }

    IEnumerator MostrarDialogo(string textoCompleto)
    {
        string textoActual = "";

        // 1. Iniciamos el loop de sonido
        AudioManager.instance.StartVoice();

        foreach (char letra in textoCompleto.ToCharArray())
        {
            textoActual += letra;
            uiTexto.text = textoActual; // Supongamos que uiTexto es tu componente de UI

            // Pequeña pausa entre letras
            yield return new WaitForSeconds(0.05f);
        }

        // 2. Apagamos el sonido cuando el texto termina de escribirse
        AudioManager.instance.StopVoice();
    }
}

