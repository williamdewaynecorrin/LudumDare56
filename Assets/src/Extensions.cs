using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector3 NoY(this Vector3 v)
    {
        return new Vector3(v.x, 0.0f, v.z);
    }
    public static Vector3 NoZ(this Vector3 v)
    {
        return new Vector3(v.x, v.y, 0.0f);
    }

    public static Color Multiply(this Color c, Color other)
    {
        return new Color(c.r * other.r, c.g * other.g, c.b * other.b, c.a * other.a);
    }
}

public static class PhysicsExtensions
{
    public static void GetCapsuleCastData(Transform transform, CharacterController character, float currentgravitymag, float gravityforce,
                                          out Vector3 p1, out Vector3 p2, out float maxdist, out float centertopoint)
    {
        centertopoint = (character.height / 2.0f - character.radius);
        p1 = transform.position + character.center + Vector3.down * centertopoint;
        p2 = transform.position + character.center + Vector3.up * centertopoint;

        maxdist = currentgravitymag + gravityforce + character.skinWidth + Physics.defaultContactOffset;
    }
}

public static class GUIExtensions
{
    private static Rect gDebugRect;
    private const float kLineHeight = 23f;

    public static void Label(ref Rect rect, float lineheight, string content)
    {
        GUI.Label(rect, content);
        rect.y += lineheight;
    }

    public static void GlobalLabel(string content)
    {
        GUI.Label(gDebugRect, content);
        gDebugRect.y += kLineHeight;
    }

    public static void GlobalHeader(string header)
    {
        GlobalLabel(string.Format("===== {0} =====", header));
    }

    public static void ResetGlobals()
    {
        gDebugRect = new Rect(5, 5, 600, 25);
    }
}

public static class RandomExtensions
{
    public static Vector3 RandomUnitVector3()
    {
        return Random.onUnitSphere;
    }
}

public static class AnimatorExtensions
{
    // -- these are always the default animator layer values
    public const string kBaseLayerName = "Base Layer";
    public const int kBaseLayerValue = 0;

    public static string GetStateName(string animname, string layername = kBaseLayerName)
    {
        return string.Format("{0}.{1}", layername, animname);
    }

    public static string GetCurrentClipInfo(Animator animator, int layer = kBaseLayerValue)
    {
        string clipstr = "";
        AnimatorClipInfo[] clipinfos = animator.GetCurrentAnimatorClipInfo(layer);
        if (clipinfos.Length == 0)
            return "";

        foreach (AnimatorClipInfo info in clipinfos)
        {
            clipstr += info.clip.name + ", ";
        }

        clipstr = clipstr.Remove(clipstr.Length - 2);

        return clipstr;
    }

    public static AnimatorStateInfo GetAnimatorState(Animator anim, int layer = kBaseLayerValue)
    {
        return anim.GetCurrentAnimatorStateInfo(layer);
    }

    public static bool AnimatorLayerInState(Animator anim, string state, int layer = kBaseLayerValue)
    {
        return GetAnimatorState(anim, layer).IsName(state);
    }
}