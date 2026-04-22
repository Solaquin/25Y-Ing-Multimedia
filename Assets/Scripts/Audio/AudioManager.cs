using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton para acceder desde cualquier lugar
    public static AudioManager instance;

    [Header("Clips de Captura")]
    public AudioClip clipCapturado;
    public AudioClip clipEscapado;
    public AudioClip clipTemblor;

    [Range(0f, 1f)]
    public float volumenGeneral = 1f;

    private void Awake()
    {
        // Configuraci�n del Singleton
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

    // M�todo para reproducir sonidos en una posici�n 3D
    public void PlaySound(AudioClip clip, Vector3 posicion)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, posicion, volumenGeneral);
        }
    }
}