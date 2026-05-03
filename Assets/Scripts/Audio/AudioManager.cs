using UnityEngine;
using System.Collections.Generic;

public static class AudioManager
{
    // ---------------------------
    // 🔊 SONIDOS NORMALES
    // ---------------------------
    public static void Play(AudioInteractivo audio, Vector3 posicion = default)
    {
        if (audio == null || audio.clips.Length == 0) return;

        AudioClip clip = audio.clips[0];

        // RANDOM
        if (audio.tipoAudio == AudioInteractivo.TipoAudio.Aleatorio ||
            audio.tipoAudio == AudioInteractivo.TipoAudio.AleatorioConPitch)
        {
            clip = audio.clips[Random.Range(0, audio.clips.Length)];
        }

        GameObject temp = new GameObject("AudioTemp");
        temp.transform.position = posicion;

        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;

        // PITCH
        if (audio.tipoAudio == AudioInteractivo.TipoAudio.AleatorioConPitch)
        {
            source.pitch = Random.Range(audio.pitchMin, audio.pitchMax);
        }

        source.Play();

        Object.Destroy(temp, clip.length);
    }

    // ---------------------------
    // 🔁 LOOPS (MEJORADO)
    // ---------------------------
    private static Dictionary<AudioInteractivo, AudioSource> loops =
        new Dictionary<AudioInteractivo, AudioSource>();

    public static void PlayLoop(AudioInteractivo audio)
    {
        if (audio == null || audio.clips.Length == 0) return;

        // Evitar duplicados
        if (loops.ContainsKey(audio) && loops[audio] != null)
            return;

        GameObject obj = new GameObject("Loop_" + audio.name);
        AudioSource source = obj.AddComponent<AudioSource>();

        source.clip = audio.clips[0];
        source.loop = true;
        source.playOnAwake = false;

        source.Play();

        Object.DontDestroyOnLoad(obj);

        loops[audio] = source;
    }

    public static void StopLoop(AudioInteractivo audio)
    {
        if (audio == null) return;

        if (loops.ContainsKey(audio))
        {
            if (loops[audio] != null)
            {
                Object.Destroy(loops[audio].gameObject);
            }

            loops.Remove(audio);
        }
    }

    public static void StopAllLoops()
    {
        foreach (var loop in loops.Values)
        {
            if (loop != null)
                Object.Destroy(loop.gameObject);
        }

        loops.Clear();
    }
}