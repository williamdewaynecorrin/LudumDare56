using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveCurve
{
    [Header("Curves")]
    public AnimationCurve right;
    public float rightscale = 1.0f;
    public AnimationCurve up;
    public float upscale = 1.0f;
    public AnimationCurve forward;
    public float forwardscale = 1.0f;

    [Header("Timing")]
    public bool useanimclip = true;
    public AnimationClip animref;
    public float curvetime = 1.0f;

    private float looptime;
    private float internaltime;
    private Vector3 currentevaluation;

    public Vector3 CurrentEvaluation => currentevaluation;

    public void Initialize()
    {
        // -- wrap modes
        right.preWrapMode = WrapMode.Loop;
        right.postWrapMode = WrapMode.Loop;
        up.preWrapMode = WrapMode.Loop;
        up.postWrapMode = WrapMode.Loop;
        forward.preWrapMode = WrapMode.Loop;
        forward.postWrapMode = WrapMode.Loop;

        if (useanimclip)
        {
            looptime = animref.length;
        }
        else
        {
            looptime = curvetime;
        }

        internaltime = 0.0f;
        currentevaluation = Evaluate();
    }

    public void ResetEvaluation()
    {
        internaltime = 0.0f;
        currentevaluation = Vector3.zero;
    }

    public void UpdateEvaluation(float dt)
    {
        internaltime += dt;
        while (internaltime > looptime)
            internaltime -= looptime;

        currentevaluation = Evaluate();
    }

    private Vector3 Evaluate()
    {
        float x = right.Evaluate(internaltime) * rightscale;
        float y = up.Evaluate(internaltime) * upscale;
        float z = forward.Evaluate(internaltime) * forwardscale;

        return new Vector3(x, y, z);
    }
}
