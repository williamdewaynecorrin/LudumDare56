using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBath : MonoBehaviour
{
    private List<PlayerSlime> slimes = new List<PlayerSlime>();

    private void OnTriggerEnter(Collider collider)
    {
        PlayerSlime slime = collider.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            if (!slimes.Contains(slime))
                slimes.Add(slime);

            if(slimes.Count > 1)
            {
                PlayerSlime s1 = slimes[1];
                slimes.Remove(s1);
                slimes[0].Combine(s1);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        PlayerSlime slime = collider.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            if (slimes.Contains(slime))
                slimes.Remove(slime);
        }
    }
}
