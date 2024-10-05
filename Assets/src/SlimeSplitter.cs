using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSplitter : MonoBehaviour
{
    [SerializeField]
    private Transform approximationhelper;

    private Vector3 normalguess = Vector3.forward;
    private Vector3 lastpos = Vector3.zero;

    void FixedUpdate()
    {
        if (lastpos != Vector3.zero)
            normalguess = (approximationhelper.position - lastpos).normalized;

        lastpos = approximationhelper.position;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PlayerSlime slime = hit.gameObject.GetComponent<PlayerSlime>();
        if(slime != null)
        {
            slime.OnHit(hit.point, hit.normal);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        PlayerSlime slime = collider.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            slime.OnHit(slime.transform.position, normalguess);
        }
    }
}
