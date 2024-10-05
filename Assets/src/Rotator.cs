using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private float rotatespeed = 1.0f;
    [SerializeField]
    private Vector3 rotateaxis = Vector3.up;

    void Awake()
    {
        rotateaxis.Normalize();
    }

    void FixedUpdate()
    {
        transform.localRotation *= Quaternion.AngleAxis(rotatespeed, rotateaxis);
    }
}
