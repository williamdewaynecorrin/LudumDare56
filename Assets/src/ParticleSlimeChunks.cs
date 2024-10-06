using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSlimeChunks : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem chunks;
    [SerializeField]
    private ParticleSystem hit;

    public void Initialize(float mass)
    {
        ParticleSystem.MainModule chunksmain = chunks.main;

        // -- speed
        chunksmain.startSpeedMultiplier *= mass;

        // -- size
        chunksmain.startSizeMultiplier *= mass;

        // -- gravity
        ParticleSystem.MinMaxCurve chunksgravity = chunksmain.gravityModifier;
        chunksgravity.constant /= mass;

        hit.transform.localScale *= mass;

        GameObject.Destroy(this.gameObject, chunksmain.startLifetime.constant);
    }
}
