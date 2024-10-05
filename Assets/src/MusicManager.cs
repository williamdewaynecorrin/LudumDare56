using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance = null;
    private static float gTargetVolume = 1.0f;

    [SerializeField]
    private AudioClip levelmusic;

    private AudioSource source;

    void Awake()
    {
        if(instance != null)
        {
            instance.levelmusic = levelmusic;
            PlayTrack();
            GameObject.Destroy(this.gameObject);
            return;
        }

        instance = this;
        source = GetComponent<AudioSource>();
        PlayTrack();
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    private void PlayTrack()
    {
        StartCoroutine(PlayTrackRoutine());
    }

    private IEnumerator PlayTrackRoutine()
    {
        if (source.isPlaying)
        {
            int fadeoutframes = 50;
            float voldec = source.volume / (float)fadeoutframes;
            for(int i = 0; i < fadeoutframes; ++i)
            {
                source.volume -= voldec;
                yield return new WaitForFixedUpdate();
            }

            source.volume = 0.0f;
            source.Stop();
        }

        source.volume = 0.0f;
        source.clip = levelmusic;
        source.Play();

        int fadeinframes = 50;
        float volinc = gTargetVolume / (float)fadeinframes;
        for (int i = 0; i < fadeinframes; ++i)
        {
            source.volume += volinc;
            yield return new WaitForFixedUpdate();
        }

        source.volume = gTargetVolume;
    }
}
