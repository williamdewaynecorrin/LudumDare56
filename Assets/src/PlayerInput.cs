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

    public static bool DialogueAdvance()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
    }

    public static Vector2 MouseDelta()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
    
    public static bool RotateCamera()
    {
        return Input.GetMouseButton(0) || Input.GetMouseButton(1);
    }

    public static ERecallType Recall()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            return ERecallType.eCircle;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            return ERecallType.eLineForward;
        else if(Input.GetKeyDown(KeyCode.Alpha3))
            return ERecallType.eLineSideways;

        return ERecallType.eNone;
    }

    public static bool Pause()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}

public enum ERecallType
{
    eNone = 0,
    eCircle = 1,
    eLineForward = 2,
    eLineSideways = 3
}
