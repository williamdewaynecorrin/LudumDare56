using System.Collections.Generic;
using UnityEngine;

public class PlayerSlimeManager : MonoBehaviour
{
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
                if (PlayerInput.Recall())
                {
                    slimetarget = CameraTargetPosition();
                    isrecalling = true;

                    // -- set positions in a circle
                    float angle = 0f;
                    float angleinc = (Mathf.PI * 2.0f) / (float)slimes.Count;
                    foreach (PlayerSlimeRoot slimeroot in slimes)
                    {
                        // -- reset local pos on root
                        slimeroot.SetTargetLocalPosition(Vector3.zero);

                        // -- set angle to target instead
                        Vector3 lpos = Vector3.right * Mathf.Cos(angle) + Vector3.forward * Mathf.Sin(angle);
                        slimeroot.Slime.BeginRecall(slimetarget + lpos);
                        angle += angleinc;
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

    public void Combine()
    {

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
}
