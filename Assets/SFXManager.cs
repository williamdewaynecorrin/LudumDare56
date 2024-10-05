using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private const float kMinDist3D = 10.0f;
    private const float kMaxDist3D = 100.0f;
    private static SFXManager instance = null;
    private static float gTargetVolume = 1.0f;

    void Awake()
    {
        if (instance != null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    public static AudioSource PlayClip2D(AudioClip clip, float volumemult = 1.0f, float pitch = 1.0f)
    {
        GameObject newaudio = new GameObject(string.Format("sfx_audio_{0}_2d", clip.name));
        AudioSource source = newaudio.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0.0f;
        source.volume = gTargetVolume * volumemult;
        source.pitch = pitch;
        source.Play();

        GameObject.Destroy(newaudio, clip.length);
        return source;
    }

    public static AudioSource PlayClip3D(AudioClip clip, Vector3 position, float volumemult = 1.0f, float pitch = 1.0f)
    {
        GameObject newaudio = new GameObject(string.Format("sfx_audio_{0}_3d", clip.name));
        newaudio.transform.position = position;

        AudioSource source = newaudio.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 1.0f;
        source.volume = gTargetVolume * volumemult;
        source.pitch = pitch;
        source.minDistance = kMinDist3D;
        source.maxDistance = kMaxDist3D;
        source.Play();

        GameObject.Destroy(newaudio, clip.length);
        return source;
    }
}
