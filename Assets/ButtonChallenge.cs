using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonChallenge : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;
    [SerializeField]
    private AudioClipWVol sfxcomplete;
    [SerializeField]
    private UnityEvent oncompleted;

    private bool completed = false;

    void Update()
    {
        if (completed)
            return;

        bool finished = true;
        foreach(Button b in buttons)
        {
            if(!b.On)
            {
                finished = false;
                break;
            }
        }

        if(finished)
        {
            completed = true;
            SFXManager.PlayClip2D(sfxcomplete.clip, sfxcomplete.volume);

            foreach (Button b in buttons)
            {
                b.SetCompleted(true);
            }

            oncompleted?.Invoke();
        }
    }
}
