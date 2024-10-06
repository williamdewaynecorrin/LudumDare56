using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIFinish : MonoBehaviour
{
    public static float gFinalMass = 1.0f;

    [SerializeField]
    private UnityEngine.UI.Button backbutton;
    [SerializeField]
    private Text finalmass;
    [SerializeField]
    private AudioClipWVol sfxclick;

    void Start()
    {
        backbutton.onClick.AddListener(() => SceneManager.LoadScene("menu"));
        backbutton.onClick.AddListener(SFXPlayClick);
        finalmass.text = (gFinalMass * UIHUD.kSlimeMassToKG).ToString("F1");
    }

    private void SFXPlayClick()
    {
        SFXManager.PlayClip2D(sfxclick.clip, sfxclick.volume);
    }
}
