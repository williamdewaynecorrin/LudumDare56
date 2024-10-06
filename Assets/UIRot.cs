using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIRot : MonoBehaviour
{
    [SerializeField]
    private float periodsin = 2.0f;
    [SerializeField]
    private float ampsin = 1.0f;
    [Range(0f, 1f)]
    [SerializeField]
    private float slerp = 0.1f;

    void FixedUpdate()
    {
        float roll = Mathf.Sin(periodsin * Time.time) * ampsin;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, 0f, roll), slerp);
    }
}
