using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerSlime slime = other.gameObject.GetComponent<PlayerSlime>();
        if (slime != null)
        {
            UIFinish.gFinalMass = slime.Root.Spawner.SlimeMass;
            SceneManager.LoadScene("finish");
        }
    }
}
