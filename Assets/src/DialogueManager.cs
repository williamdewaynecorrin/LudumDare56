using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DialogueManager : MonoBehaviour
{
    private const float kAnimateFrames = 26f;

    public static int gLastPlayedDialogue = -1;
    private static DialogueManager gDialogueManager = null;
    public static DialogueManager GDialogueManager => gDialogueManager;

    // -- going do the UI together with this cause game jam
    // -- also going to have the entries all live on this object 
    [SerializeField]
    private DialogueEntry[] allentires;
    [SerializeField]
    private float advancetime = 0.075f;
    [SerializeField]
    private int sfxscrollmod = 3;
    [SerializeField]
    private Transform uipanel;
    [SerializeField]
    private Transform uipanelonscreen;
    [SerializeField]
    private Transform uipaneloffscreen;
    [SerializeField]
    private Text dialoguedisplay;
    [SerializeField]
    private AudioClipWVolPitch sfxtextscroll;
    [SerializeField]
    private AudioClipWVol sfxadvance;
    [SerializeField]
    private AudioClipWVol sfxclose;
    [SerializeField]
    private AudioClipWVol sfxopen;
    [SerializeField]
    private AudioSource sfxsource;

    private DialogueEntry currententry;
    private string currentdialoguestring;
    private int currentdialoguestringindex;
    private float advancetimer;
    private bool finished = false;
    private bool open = false;
    private int queuedindex = -1;

    void Awake()
    {
        gDialogueManager = this;

        foreach(DialogueEntry entry in allentires)
        {
            entry.Initialize();
        }

        advancetimer = advancetime;
        currentdialoguestring = "";
        currentdialoguestringindex = 0;
        finished = false;
        open = false;
        uipanel.localPosition = uipaneloffscreen.localPosition;
    }

    void Start()
    {
        QueueDialogue(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (currententry == null || !open)
            return;

        if(finished)
        {
            // -- wait for press
            if(PlayerInput.DialogueAdvance())
            {
                currententry.onentryfinished?.Invoke();
                SFXManager.PlayClip2D(sfxadvance.clip, sfxadvance.volume);

                if (currententry.nextentry != -1)
                {
                    QueueDialogue(currententry.nextentry);
                }
                else
                {
                    Close();
                }
            }
        }
        else
        {
            bool wantstoskip = PlayerInput.DialogueAdvance();
            if (wantstoskip)
            {
                finished = true;
                advancetimer = advancetime;
                currentdialoguestring = currententry.entry;
                currentdialoguestringindex = currententry.entry.Length;
                dialoguedisplay.text = currentdialoguestring;
                return;
            }

            // -- animate
            advancetimer -= Time.deltaTime;
            if(advancetimer <= 0.0f)
            {
                currentdialoguestring += currententry.entry[currentdialoguestringindex];
                ++currentdialoguestringindex;
                advancetimer = advancetime;
                if(currentdialoguestringindex == currententry.entry.Length)
                {
                    finished = true;
                }

                dialoguedisplay.text = currentdialoguestring;

                if (currentdialoguestringindex % sfxscrollmod == 0)
                {
                    PlaySFXScroll();
                }
            }
        }
    }

    public void Close()
    {
        SFXManager.PlayClip2D(sfxclose.clip, sfxclose.volume);
        StartCoroutine(CloseRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        Vector3 tooff = uipaneloffscreen.localPosition - uipanel.localPosition;
        Vector3 inc = tooff / kAnimateFrames;

        for(int i = 0; i < kAnimateFrames; ++i)
        {
            uipanel.localPosition += inc;
            yield return new WaitForFixedUpdate();
        }

        uipanel.localPosition = uipaneloffscreen.localPosition;
        open = false;
        finished = false;
        queuedindex = -1;
        GameManager.DialogueActivated(false);
    }

    public void Open()
    {
        GameManager.DialogueActivated(true);
        dialoguedisplay.text = "";
        SFXManager.PlayClip2D(sfxopen.clip, sfxopen.volume);
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        Vector3 tooff = uipanelonscreen.localPosition - uipanel.localPosition;
        Vector3 inc = tooff / kAnimateFrames;

        for (int i = 0; i < kAnimateFrames; ++i)
        {
            uipanel.localPosition += inc;
            yield return new WaitForFixedUpdate();
        }

        uipanel.localPosition = uipanelonscreen.localPosition;
        open = true;
        finished = false;

        Assert.IsTrue(queuedindex != -1, "Should never be opening the dialogue system with no queued index");
        PlayDialogue(queuedindex);
    }

    private void PlaySFXScroll()
    {
        // -- play text scroll
        sfxsource.clip = sfxtextscroll.clip;
        sfxsource.volume = sfxtextscroll.volume;
        sfxsource.pitch = sfxtextscroll.pitch.PickValue();
        sfxsource.Play();
    }

    public void QueueDialogue(int dialogueindex)
    { 
        if(!open)
        {
            queuedindex = dialogueindex;
            Open();
            return;
        }

        PlayDialogue(dialogueindex);
    }

    private void PlayDialogue(int dialogueindex)
    {
        Assert.IsTrue(dialogueindex < allentires.Length && dialogueindex >= 0, string.Format("Invalid dialogue index provided: {0}", dialogueindex));
        currententry = allentires[dialogueindex];
        currentdialoguestring = "";
        currentdialoguestringindex = 0;
        finished = false;
        gLastPlayedDialogue = dialogueindex;
        queuedindex = -1;
    }
}

[System.Serializable]
public class DialogueEntry
{
    [TextArea(1, 4)]
    public string entry = "Dialogue entry goes here";
    public Color color = Color.white;
    public int nextentry = -1;
    public UnityEvent onentryfinished; 

    public void Initialize()
    {

    }
}
