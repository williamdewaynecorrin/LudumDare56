using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlimeRoot : MonoBehaviour
{
    [SerializeField]
    private PlayerSlime playerslime;

    private PlayerSlimeManager spawner;

    public PlayerSlime Slime => playerslime;

    public void OnSpawn(PlayerSlimeManager manager, float mass, float lasthittime)
    {
        spawner = manager;
        playerslime.SetCamera(manager.Camera);
        playerslime.SetMass(mass, true);
        playerslime.SetLastHitTime(lasthittime);
    }

    public void SetTargetLocalPosition(Vector3 pos)
    {
        playerslime.Character.enabled = false;
        transform.localPosition = pos;
        playerslime.Character.enabled = true;
    }

    public void Split()
    {
        float newmass = playerslime.Mass / 2.0f;
        playerslime.SetMass(newmass, false);

        spawner.SpawnSlime(this, newmass);
    }

    public void Combine(PlayerSlime other)
    {
        spawner.Combine(this, other);
    }

    public void Die()
    {
        spawner.Die(this);
    }
}
