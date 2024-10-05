using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private float rotatespeed = 1.0f;
    [SerializeField]
    private Vector3 rotateaxis = Vector3.up;
    [SerializeField]
    private ParticleSystem whooshparticles;
    [SerializeField]
    private AudioClipWVol sfxwhoosh;

    private float previousr;
    private bool lastdir;

    void Awake()
    {
        rotateaxis.Normalize();
        previousr = transform.localRotation.y;
        lastdir = true;
        whooshparticles.Stop();
    }

    void FixedUpdate()
    {
        previousr = transform.localRotation.y;
        transform.localRotation *= Quaternion.AngleAxis(rotatespeed, rotateaxis);
        float curr = transform.localRotation.y;

        if(curr - previousr < 0)
        {
            if(lastdir)
            {
                PlayWhoosh();
            }
            lastdir = false;
        }
        else
        {
            if(!lastdir)
            {
                PlayWhoosh();
            }
            lastdir = true;
        }
    }

    private void PlayWhoosh()
    {
        SFXManager.PlayClip3D(sfxwhoosh.clip, transform.position, sfxwhoosh.volume);
        whooshparticles.Play();
    }
}
