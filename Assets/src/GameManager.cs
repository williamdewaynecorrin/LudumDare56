using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(ExecutionOrders.kGameManager)]
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static bool gPaused = false;
    public static bool gDialogueOpen = false;

    [SerializeField]
    private PlayerSlimeManager playermanager;

    void Awake()
    {
        if(instance != null)
        {
            if (instance.playermanager != null)
                instance.playermanager = playermanager;

            GameObject.Destroy(this.gameObject);
            return;
        }

        instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);

        GUIExtensions.ResetGlobals();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUIExtensions.ResetGlobals();
    }

    public static void Pause()
    {
        gPaused = true;
        instance.playermanager.Pause();
    }

    public static void Unpause()
    {
        gPaused = true;
        instance.playermanager.Unpause();
    }

    public static void DialogueActivated(bool activated)
    {
        gDialogueOpen = activated;


        if(activated)
        {
            instance.playermanager.Pause();
        }
        else
        {
            instance.playermanager.Unpause();
        }
    }
}
