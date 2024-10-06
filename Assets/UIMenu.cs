using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Button startbutton;
    [SerializeField]
    private UnityEngine.UI.Button controlsbutton;
    [SerializeField]
    private UnityEngine.UI.Button quitbutton;
    [SerializeField]
    private UnityEngine.UI.Button backbutton;
    [SerializeField]
    private GameObject controlsstuff;
    [SerializeField]
    private AudioClipWVol sfxclick;

    void Start()
    {
        startbutton.onClick.AddListener(() => SceneManager.LoadScene("test_level_basic"));
        controlsbutton.onClick.AddListener(ActivateControls);
        quitbutton.onClick.AddListener(() => Application.Quit());
        backbutton.onClick.AddListener(BackFromControls);

        startbutton.onClick.AddListener(SFXPlayClick);
        controlsbutton.onClick.AddListener(SFXPlayClick);
        quitbutton.onClick.AddListener(SFXPlayClick);
        backbutton.onClick.AddListener(SFXPlayClick);

        backbutton.gameObject.SetActive(false);
    }

    private void ActivateControls()
    {
        startbutton.gameObject.SetActive(false);
        controlsbutton.gameObject.SetActive(false);
        quitbutton.gameObject.SetActive(false);

        backbutton.gameObject.SetActive(true);
        controlsstuff.SetActive(true);
    }

    private void BackFromControls()
    {
        startbutton.gameObject.SetActive(true);
        controlsbutton.gameObject.SetActive(true);
        quitbutton.gameObject.SetActive(true);

        backbutton.gameObject.SetActive(false);
        controlsstuff.SetActive(false);
    }

    private void SFXPlayClick()
    {
        SFXManager.PlayClip2D(sfxclick.clip, sfxclick.volume);
    }
}
