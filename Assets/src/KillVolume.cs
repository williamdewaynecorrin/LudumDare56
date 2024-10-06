using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        PlayerSlime slime = collider.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            slime.Die();
        }
    }
}
