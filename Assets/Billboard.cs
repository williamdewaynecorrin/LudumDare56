using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField]
    private Camera billboardcam;

    void Start()
    {
        if(billboardcam == null)
            billboardcam = GameObject.FindObjectOfType<Camera>();
    }

    void Update()
    {
        Vector3 tocam = (transform.position - billboardcam.transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(tocam);
    }
}
