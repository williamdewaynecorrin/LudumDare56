using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    private const float kSlimeMassToKG = 100.0f;

    [SerializeField]
    private PlayerSlimeManager playermanager;
    [SerializeField]
    private Text slimecount;
    [SerializeField]
    private Text mass;

    void Update()
    {
        slimecount.text = playermanager.SlimeCount.ToString();
        mass.text = (playermanager.SlimeMass * kSlimeMassToKG).ToString("F1");
    }
}
