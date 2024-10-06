using System.Collections.Generic;
using UnityEngine;

public class PlayerSlimeManager : MonoBehaviour
{
    private const int kMaxPerLine = 6;
    private const float kOneMassToRadius = 1.54f; // character controller radius on a 1-mass slime is 0.77, so this includes padding
    private const float kSlimeTargetCastDistance = 10.0f;

    [SerializeField]
    private PlayerSlimeRoot slimeprefab;
    [SerializeField]
    private float startingmass = 1.0f;
    [SerializeField]
    private Transform middletarget;
    [SerializeField]
    private float splitlasthittimebonus = 2.0f;
    [SerializeField]
    private new PlayerSlimeCamera camera;
    [SerializeField]
    private LayerMask groundmask;

    private List<PlayerSlimeRoot> slimes;
    private bool isrecalling = false;
    private Vector3 slimetarget;
    private float slimemasstotal;

    public PlayerSlimeCamera Camera => camera;
    public float SlimeMass => slimemasstotal;
    public int SlimeCount => slimes.Count;

    void Awake()
    {
        // -- start with one full mass slime
        slimes = new List<PlayerSlimeRoot>();

        PlayerSlimeRoot rootslime = GameObject.Instantiate(slimeprefab, transform);
        rootslime.OnSpawn(this, startingmass, Time.time);
        rootslime.SetTargetLocalPosition(Vector3.zero);
        slimes.Add(rootslime);
    }

    void Update()
    {
        if (GameManager.gDialogueOpen || GameManager.gPaused)
            return;

        UpdateStats();

        if (!isrecalling)
        {
            bool canrecall = true;
            foreach (PlayerSlimeRoot slimeroot in slimes)
            {
                if (!slimeroot.Slime.Grounded)
                {
                    canrecall = false;
                    break;
                }
            }

            if (canrecall)
            {
                ERecallType recallresult = PlayerInput.Recall();
                if (recallresult != ERecallType.eNone)
                {
                    slimetarget = SlimeTargetRecallPosition();
                    isrecalling = true;

                    Vector3 forward = camera.transform.forward.NoY().normalized;
                    Vector3 right = camera.transform.right.NoY().normalized;

                    if (recallresult == ERecallType.eCircle)
                    {
                        // -- set positions in a circle
                        float angle = 0f;
                        float angleinc = (Mathf.PI * 2.0f) / (float)slimes.Count;
                        foreach (PlayerSlimeRoot slimeroot in slimes)
                        {
                            // -- reset local pos on root
                            slimeroot.SetTargetLocalPosition(Vector3.zero);

                            // -- set angle to target instead
                            Vector3 lpos = right * Mathf.Cos(angle) + forward * Mathf.Sin(angle);
                            slimeroot.Slime.BeginRecall(slimetarget + lpos);
                            angle += angleinc;
                        }
                    }
                    else if(recallresult == ERecallType.eLineForward || recallresult == ERecallType.eLineSideways)
                    {
                        Vector3 linedir = recallresult == ERecallType.eLineForward ? forward : right;

                        // all in one line
                        float estdistance = slimemasstotal * kOneMassToRadius * 2.0f;
                        float curdistance = -estdistance / 2.0f;
                        foreach (PlayerSlimeRoot slimeroot in slimes)
                        {
                            // -- reset local pos on root
                            slimeroot.SetTargetLocalPosition(Vector3.zero);

                            // -- set angle to target instead
                            Vector3 lpos = curdistance * linedir;
                            slimeroot.Slime.BeginRecall(slimetarget + lpos);

                            curdistance += slimeroot.Slime.Mass * kOneMassToRadius * 2.0f;
                        }
                    }
                }
            }
        }
        else
        {
            bool recallfinished = true;
            foreach (PlayerSlimeRoot slimeroot in slimes)
            {
                if(!slimeroot.Slime.ReachedTarget)
                {
                    recallfinished = false;
                    break;
                }
            }

            if(recallfinished)
            {
                isrecalling = false;
                foreach (PlayerSlimeRoot slimeroot in slimes)
                {
                    slimeroot.Slime.EndRecall();
                }
            }
        }
    }

    private void UpdateStats()
    {
        // -- update stats
        slimemasstotal = 0.0f;
        foreach(PlayerSlimeRoot slimeroot in slimes)
        {
            slimemasstotal += slimeroot.Slime.Mass;
        }
    }

    public void SpawnSlime(PlayerSlimeRoot from, float newmass)
    {
        PlayerSlimeRoot newslime = GameObject.Instantiate(slimeprefab, transform);
        newslime.OnSpawn(this, newmass, Time.time + splitlasthittimebonus);
        newslime.Slime.SpawnedFrom(from.Slime);
        slimes.Add(newslime);

        // -- adjust all local positions
        float angle = 0f;
        float angleinc = (Mathf.PI * 2.0f) / (float)slimes.Count;
        for(int i = 0; i < slimes.Count; ++i)
        {
            Vector3 lpos = Vector3.right * Mathf.Cos(angle) + 
                           Vector3.forward * Mathf.Sin(angle);
            slimes[i].SetTargetLocalPosition(lpos);
            angle += angleinc;
        }

        // -- zoom to fit
        camera.ZoomToFit();
    }

    public float LargestMass()
    {
        float largestmass = 0.0f;
        foreach (PlayerSlimeRoot slimeroot in slimes)
        {
            if (slimeroot.Slime.Mass > largestmass)
            {
                largestmass = slimeroot.Slime.Mass;
            }
        }

        return largestmass;
    }

    public void Die(PlayerSlimeRoot root)
    {
        slimes.Remove(root);

        GameObject.Destroy(root.gameObject);
    }

    public void Combine(PlayerSlimeRoot survivor, PlayerSlime other)
    {
        // -- survivor eats other
        survivor.Slime.AddMass(other.Mass);

        slimes.Remove(other.Root);
        GameObject.Destroy(other.Root.gameObject);
    }

    public Vector3 CameraTargetPosition()
    {
        Vector3 mp = Vector3.zero;
        foreach(PlayerSlimeRoot slime in slimes)
        {
            mp += slime.Slime.transform.position;
        }

        mp /= (float)slimes.Count;

        return mp;
    }

    public Vector3 SlimeTargetRecallPosition()
    {
        float highesty = float.MinValue;
        Vector3 mp = Vector3.zero;
        foreach (PlayerSlimeRoot slime in slimes)
        {
            Vector3 spos = slime.Slime.transform.position;
            mp += spos;
            if(spos.y > highesty)
            {
                highesty = spos.y;
            }
        }

        mp /= (float)slimes.Count;
        mp.y = highesty;

        if(Physics.Raycast(mp, Vector3.down, out RaycastHit hit, kSlimeTargetCastDistance, groundmask, QueryTriggerInteraction.Ignore))
        {
            mp = hit.point + Vector3.up * 0.5f;
        }

        return mp;
    }
}
