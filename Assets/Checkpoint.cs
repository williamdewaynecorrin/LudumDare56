using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private Transform xf;

    public Vector3 Pos => xf.position;
    public Quaternion Rot => xf.rotation;

    private void OnTriggerEnter(Collider other)
    {
        PlayerSlime slime = other.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            slime.Root.Spawner.SetLastCheckpoint(this);
        }
    }
}
