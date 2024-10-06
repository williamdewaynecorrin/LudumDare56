using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHappening : MonoBehaviour
{
    [SerializeField]
    private Image happeningimage;
    [Range(0f, 1f)]
    [SerializeField]
    private float scalelerp = 0.1f;
    [SerializeField]
    private float minactivtetime = 1.0f;

    private Vector3 activescale;
    private Vector3 inactivescale;
    private bool activated;
    private float lastactivationtime;

    public bool Activated => activated;

    void Awake()
    {
        activescale = transform.localScale;
        inactivescale = activescale;
        inactivescale.x = 0.0f;
        transform.localScale = inactivescale;

        activated = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(activated || lastactivationtime + minactivtetime > Time.time)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, activescale, scalelerp);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, inactivescale, scalelerp);
        }
    }

    public void Activate(bool active, Sprite sprite = null)
    {
        activated = active;
        lastactivationtime = Time.time;

        if(sprite != null)
            happeningimage.sprite = sprite;
    }
}
