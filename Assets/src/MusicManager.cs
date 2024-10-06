using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance = null;
    private static float gTargetVolume = 0.3f;

    [SerializeField]
    private AudioClip levelmusic;

    private AudioSource source;

    void Awake()
    {
        if(instance != null)
        {
            instance.levelmusic = levelmusic;
            instance.PlayTrack();
            GameObject.Destroy(this.gameObject);
            return;
        }

        instance = this;
        source = GetComponent<AudioSource>();
        instance.source.loop = true;
        PlayTrack();
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    private void PlayTrack()
    {
        StartCoroutine(PlayTrackRoutine());
    }

    private IEnumerator PlayTrackRoutine()
    {
        if (instance.source.isPlaying)
        {
            int fadeoutframes = 50;
            float voldec = instance.source.volume / (float)fadeoutframes;
            for(int i = 0; i < fadeoutframes; ++i)
            {
                instance.source.volume -= voldec;
                yield return new WaitForFixedUpdate();
            }

            instance.source.volume = 0.0f;
            instance.source.Stop();
        }

        instance.source.volume = 0.0f;
        instance.source.clip = instance.levelmusic;
        instance.source.Play();

        int fadeinframes = 50;
        float volinc = gTargetVolume / (float)fadeinframes;
        for (int i = 0; i < fadeinframes; ++i)
        {
            instance.source.volume += volinc;
            yield return new WaitForFixedUpdate();
        }

        instance.source.volume = gTargetVolume;
    }
}
