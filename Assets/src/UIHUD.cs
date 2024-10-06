using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    public const float kSlimeMassToKG = 100.0f;

    [SerializeField]
    private PlayerSlimeManager playermanager;
    [SerializeField]
    private Text slimecount;
    [SerializeField]
    private Text mass;
    [SerializeField]
    private Color panelusecolor = Color.yellow;
    [SerializeField]
    private UIButtonPanel[] buttonpanels;
    [SerializeField]
    private Text deathtimetext;
    [SerializeField]
    private GameObject deathpanel;
    [SerializeField]
    private AudioClipWVol sfxclick;
    [SerializeField]
    private AudioClipWVol sfxletgo;
    [SerializeField]
    private AudioClipWVol sfxdeathtimer;

    void Awake()
    {
        HideDeathScreen();
    }

    void Update()
    {
        slimecount.text = playermanager.SlimeCount.ToString();
        mass.text = (playermanager.SlimeMass * kSlimeMassToKG).ToString("F1");

        foreach (UIButtonPanel panel in buttonpanels)
        {
            if (Input.GetKeyDown(panel.Key))
            {
                panel.Shade(panelusecolor);
                SFXManager.PlayClip2D(sfxclick.clip, sfxclick.volume);
            }
            else if (Input.GetKeyUp(panel.Key))
            {
                panel.ResetTints();
                SFXManager.PlayClip2D(sfxletgo.clip, sfxletgo.volume);
            }
        }
    }

    public void SetDeathScreen(int time)
    {
        deathpanel.SetActive(true);
        deathtimetext.text = time.ToString();
        SFXManager.PlayClip2D(sfxdeathtimer.clip, sfxdeathtimer.volume);
    }

    public void HideDeathScreen()
    {
        deathpanel.SetActive(false);
    }
}
