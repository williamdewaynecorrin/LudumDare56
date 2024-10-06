using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField]
    private Material mat;
    [SerializeField]
    private Renderer rend;
    [SerializeField]
    private Transform buttontransform;
    [SerializeField]
    private Transform buttontransformon;
    [SerializeField]
    private Transform buttontransformoff;
    [Range(0f, 1f)]
    [SerializeField]
    private float poslerp = 0.1f;
    [SerializeField]
    private UnityEvent oncompleted;
    [SerializeField]
    private Color oncolor = Color.green;
    [ColorUsage(true, true)]
    [SerializeField]
    private Color oncolorrim = Color.green;
    [SerializeField]
    private Color offcolor =  Color.red;
    [ColorUsage(true, true)]
    [SerializeField]
    private Color offcolorrim = Color.green;
[SerializeField]
    private AudioClipWVol sfxon;
    [SerializeField]
    private AudioClipWVol sfxoff;

    private List<PlayerSlime> slimes = new List<PlayerSlime>();
    private Material matinst;
    private bool on = false;
    private bool completed = false;

    public bool On => on;

    void Awake()
    {
        matinst = new Material(mat);
        rend.material = matinst;

        matinst.color = offcolor;
        on = false;
    }

    void FixedUpdate()
    {
        if(on || completed)
        {
            buttontransform.localPosition = Vector3.Lerp(buttontransform.localPosition, buttontransformon.localPosition, poslerp);
        }
        else
        {
            buttontransform.localPosition = Vector3.Lerp(buttontransform.localPosition, buttontransformoff.localPosition, poslerp);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (completed)
            return;

        PlayerSlime slime = collider.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            if (!slimes.Contains(slime))
                slimes.Add(slime);

            if(slimes.Count > 0)
            {
                if(!on)
                {
                    SFXManager.PlayClip2D(sfxon.clip, sfxon.volume);
                }

                matinst.color = oncolor;
                matinst.SetColor("_RimColor", oncolorrim);
                on = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (completed)
            return;

        PlayerSlime slime = collider.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            if (slimes.Contains(slime))
                slimes.Remove(slime);

            if(slimes.Count == 0)
            {
                if (on)
                {
                    SFXManager.PlayClip3D(sfxoff.clip, transform.position, sfxoff.volume);
                }

                matinst.color = offcolor;
                matinst.SetColor("_RimColor", offcolorrim);
                on = false;
            }
        }
    }

    public void SetCompleted(bool comp)
    {
        completed = comp;
    }
}
