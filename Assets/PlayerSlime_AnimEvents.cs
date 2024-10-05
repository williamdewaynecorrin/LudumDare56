using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlime_AnimEvents : MonoBehaviour
{
    [SerializeField]
    private PlayerSlime slime;
    [SerializeField]
    private AudioClipWVol sfxsquish;
    [SerializeField]
    private AudioClipWVol sfxland;
    [SerializeField]
    private AudioClipWVol sfxbounce;
    [SerializeField]
    private new AudioSource audio;

    public void PlaySquishSFX()
    {
        if (audio.isPlaying)
            audio.Stop();

        float pitch = 1.0f / slime.Mass;
        audio.clip = sfxsquish.clip;
        audio.volume = sfxsquish.volume * slime.Mass;
        audio.pitch = pitch;
        audio.Play();
    }

    public void PlayLandSFX()
    {
        if (audio.isPlaying)
            audio.Stop();

        float pitch = 1.0f / slime.Mass;
        audio.clip = sfxland.clip;
        audio.volume = sfxland.volume * slime.Mass;
        audio.pitch = pitch;
        audio.Play();
    }

    public void PlayBounceSFX()
    {
        if (audio.isPlaying)
            audio.Stop();

        float pitch = 1.0f / slime.Mass;
        audio.clip = sfxbounce.clip;
        audio.volume = sfxbounce.volume * slime.Mass;
        audio.pitch = pitch;
        audio.pitch = pitch;
        audio.Play();
    }
}

[System.Serializable]
public class AudioClipWVol
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1.0f;
}