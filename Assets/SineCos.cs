using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineCos : MonoBehaviour
{
    [SerializeField]
    private float periodsin = 2.0f;
    [SerializeField]
    private float ampsin = 1.0f;
    [SerializeField]
    private float periodcos = 1.0f;
    [SerializeField]
    private float ampcos = 1.0f;
    [Range(0f, 1f)]
    [SerializeField]
    private float slerp = 0.1f;

    void FixedUpdate()
    {
        float pitch = Mathf.Sin(periodsin * Time.time) * ampsin;
        float yaw = Mathf.Sin(periodcos * Time.time) * ampcos;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(pitch, yaw, 0.0f), slerp);
    }
}
