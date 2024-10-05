using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInput
{
    public static Vector2 MovementInput()
    {
        float x = Input.GetKey(KeyCode.D) ? 1.0f : 0.0f;
        x += Input.GetKey(KeyCode.A) ? -1.0f : 0.0f;

        float y = Input.GetKey(KeyCode.W) ? 1.0f : 0.0f;
        y += Input.GetKey(KeyCode.S) ? -1.0f : 0.0f;

        return new Vector2(x, y);
    }

    public static bool JumpStart()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public static bool JumpEnd()
    {
        return Input.GetKeyUp(KeyCode.Space);
    }

    public static Vector2 MouseDelta()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public static bool Recall()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    public static bool Pause()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
