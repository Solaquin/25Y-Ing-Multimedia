using UnityEngine;
using System.Collections.Generic;

public static class AudioManager
{
    // ---------------------------
    // SONIDOS NORMALES (FX)
    // ---------------------------
    public static void Play(AudioInteractivo audio, Vector3 posicion = default)
    {
        if (audio == null || audio.clips.Length == 0)
            return;

        AudioClip clip = audio.clips[0];

        if (audio.tipoAudio == AudioInteractivo.TipoAudio.Aleatorio ||
            audio.tipoAudio == AudioInteractivo.TipoAudio.AleatorioConPitch)
        {
            clip = audio.clips[Random.Range(0, audio.clips.Length)];
        }

        PlayClip(clip, posicion, audio);
    }

    // ---------------------------
    // REPRODUCIR CLIP
    // ---------------------------
    public static void PlayClip(
        AudioClip clip,
        Vector3 posicion,
        AudioInteractivo config = null)
    {
        if (clip == null)
            return;

        GameObject temp = new GameObject("AudioTemp");
        temp.transform.position = posicion;

        AudioSource source = temp.AddComponent<AudioSource>();

        source.clip = clip;
        source.spatialBlend = 0f;

        if (config != null)
        {
            source.volume =
                config.volumen *
                AudioInteractivo.GlobalFXVolume;
        }
        else
        {
            source.volume =
                AudioInteractivo.GlobalFXVolume;
        }

        if (config != null &&
            config.tipoAudio ==
            AudioInteractivo.TipoAudio.AleatorioConPitch)
        {
            source.pitch =
                Random.Range(config.pitchMin,
                             config.pitchMax);
        }

        source.Play();

        Object.Destroy(temp, clip.length);
    }

    // ---------------------------
    // LOOPS
    // ---------------------------
    private static Dictionary<AudioInteractivo, AudioSource> loops =
        new Dictionary<AudioInteractivo, AudioSource>();

    public static void PlayLoop(AudioInteractivo audio)
    {
        if (audio == null || audio.clips.Length == 0)
            return;

        if (loops.ContainsKey(audio))
            return;

        GameObject obj =
            new GameObject("Loop_" + audio.name);

        AudioSource source =
            obj.AddComponent<AudioSource>();

        source.clip = audio.clips[0];
        source.loop = true;
        source.spatialBlend = 0f;

        source.volume =
            audio.volumen *
            AudioInteractivo.GlobalMusicVolume;

        source.Play();

        Object.DontDestroyOnLoad(obj);

        loops[audio] = source;
    }

    public static void StopLoop(AudioInteractivo audio)
    {
        if (audio == null)
            return;

        if (loops.ContainsKey(audio))
        {
            if (loops[audio] != null)
            {
                Object.Destroy(
                    loops[audio].gameObject);
            }

            loops.Remove(audio);
        }
    }

    public static void StopAllLoops()
    {
        foreach (var loop in loops.Values)
        {
            if (loop != null)
            {
                Object.Destroy(loop.gameObject);
            }
        }

        loops.Clear();
    }

    // ---------------------------
    // ACTUALIZAR VOLÚMENES
    // ---------------------------
    public static void RefreshVolumes()
    {
        foreach (var loop in loops)
        {
            if (loop.Key == null || loop.Value == null)
                continue;

            loop.Value.volume =
                loop.Key.volumen *
                AudioInteractivo.GlobalMusicVolume;
        }
    }
}