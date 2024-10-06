using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlimeMassChunk : MonoBehaviour
{
    [SerializeField]
    [Range(0.001f, 0.1f)]
    private float mass = 0.01f;
    [SerializeField]
    private AudioClipWVolPitch sfxcollect;
    [SerializeField]
    private GameObject fxcollectprefab;
    [SerializeField]
    private float fxlifetime = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerSlime slime = other.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            slime.AddMass(mass);

            SFXManager.PlayClip3D(sfxcollect.clip, transform.position, sfxcollect.volume, sfxcollect.pitch.PickValue());

            GameObject fxinstance = GameObject.Instantiate(fxcollectprefab);
            fxinstance.transform.position = transform.position;
            fxinstance.transform.rotation = transform.rotation;
            GameObject.Destroy(fxinstance, fxlifetime);

            GameManager.Destroy(this.gameObject);
        }
    }
}
