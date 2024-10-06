using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonPanel : MonoBehaviour
{
    private const float kPressSize = 0.925f;

    [SerializeField]
    private MaskableGraphic[] tintgraphics;
    [SerializeField]
    private KeyCode key;

    private Color[] normalcolors;
    private Vector3 normalsize;

    public KeyCode Key => key;

    void Awake()
    {
        normalcolors = new Color[tintgraphics.Length];
        for (int i = 0; i < tintgraphics.Length; ++i)
        {
            normalcolors[i] = tintgraphics[i].color;
        }

        normalsize = transform.localScale;
    }

    public void Shade(Color color)
    {
        for (int i = 0; i < tintgraphics.Length; ++i)
        {
            tintgraphics[i].color = tintgraphics[i].color.Multiply(color);
        }

        transform.localScale = normalsize * kPressSize;
    }

    public void ResetTints()
    {
        for (int i = 0; i < tintgraphics.Length; ++i)
        {
            tintgraphics[i].color = normalcolors[i];
        }

        transform.localScale = normalsize;
    }
}
