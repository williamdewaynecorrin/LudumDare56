using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private bool oneshot = true;
    [SerializeField]
    private int dialogueidx = 0;

    private bool played = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerSlime slime = other.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            if (oneshot && played)
                return;
            else
            {
                DialogueManager.GDialogueManager.QueueDialogue(dialogueidx);
                played = true;
            }
        }
    }
}
