using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlimeCamera : MonoBehaviour
{
    [SerializeField]
    private PlayerSlimeManager target;
    [SerializeField]
    private SlimeCameraOffset[] targetoffsets; /* = new Vector3(0f, 2.88f, -5.65f);*/
    [SerializeField]
    private Camera cam;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float positionlerp = 0.1f;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float rotationslerp = 0.1f;
    [SerializeField]
    private float maxpitchangle = 45.0f;
    [SerializeField]
    private float minpitchangle = -20.0f;
    [SerializeField]
    private float pitchsensitivity = 1f;
    [SerializeField]
    private float yawsensitivity = 1f;
    [SerializeField]
    private bool invertpitch = false;
    [SerializeField]
    private bool invertyaw = false;
    [SerializeField]
    private float pitchamount = 0.0f;
    [SerializeField]
    private float yawamount = 0.0f;

    void Update()
    {
        if(PlayerInput.RotateCamera())
        {
            Vector2 mousedelta = PlayerInput.MouseDelta();
            float addpitch = pitchsensitivity * -mousedelta.y;
            float addyaw = yawsensitivity * mousedelta.x;

            pitchamount += invertpitch ? -addpitch : addpitch;
            pitchamount = Mathf.Clamp(pitchamount, minpitchangle, maxpitchangle);
            yawamount += invertyaw ? -addyaw : addyaw;
        }
    }

    void FixedUpdate()
    {
        float smallestmass = target.LargestMass();
        int currentoffset = LargestMassToTargetIndex(smallestmass);
        Vector3 slimemidpointtarget = target.CameraTargetPosition();

        // -- rotate offset by pitch/yaw camera angle controls
        Quaternion offsetrot = Quaternion.Euler(pitchamount, yawamount, 0.0f);
        offsetrot.eulerAngles = offsetrot.eulerAngles.NoZ();
        Vector3 rotatedoffset = offsetrot * targetoffsets[currentoffset].offset;

        Vector3 targetpos = slimemidpointtarget + rotatedoffset;
        transform.position = Vector3.Lerp(transform.position, targetpos, positionlerp);

        Vector3 totargetpos = (slimemidpointtarget - targetpos).normalized;

        Quaternion targetrot = Quaternion.LookRotation(totargetpos);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrot, rotationslerp);
        transform.eulerAngles = transform.eulerAngles.NoZ();
    }

    private int LargestMassToTargetIndex(float largest)
    {
        int index = 0;
        for(int i = 0; i< targetoffsets.Length; ++i)
        {
            if(largest <= targetoffsets[i].massthresh)
            {
                index = i;
            }
        }

        return index;
    }

    private void OnGUI()
    {
        return;

        GUIExtensions.GlobalHeader("CAMERA");
        GUIExtensions.GlobalLabel(string.Format("Current Pitch: {0}", pitchamount.ToString("F3")));
        GUIExtensions.GlobalLabel(string.Format("Current YAW: {0}", yawamount.ToString("F3")));
    }

    private Bounds GetBoundsFor(GameObject obj)
    {
        Bounds fullbounds = new Bounds(obj.transform.position, Vector3.zero);
        Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rends)
        {
            fullbounds.Encapsulate(r.bounds);
        }

        return fullbounds;
    }

    public void ZoomToFit()
    {
        return;

        Bounds targetbounds = GetBoundsFor(target.gameObject);
        Vector3 size = targetbounds.size;
        float cameradiagonal = size.magnitude;

        cam.orthographicSize = cameradiagonal / 2.0f;
        transform.position = targetbounds.center;
    }
}

[System.Serializable]
public class SlimeCameraOffset
{
    public Vector3 offset;
    public float massthresh = 1.0f;
}