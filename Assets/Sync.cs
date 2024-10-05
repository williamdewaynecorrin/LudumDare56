using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sync : MonoBehaviour
{
    [SerializeField]
    private MinMaxFloat offsync;

    public float OffSync => offsync.LastPickedValue;

    void Awake()
    {
        offsync.PickValue();
    }
}

[System.Serializable]
public class MinMaxFloat
{
    public float min = 0.0f;
    public float max = 1.0f;

    private float lastpickedvalue;
    public float LastPickedValue => lastpickedvalue;

    public float PickValue()
    {
        lastpickedvalue = Random.Range(min, max);
        return lastpickedvalue;
    }
}