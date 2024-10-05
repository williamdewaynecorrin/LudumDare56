using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSlime : MonoBehaviour
{
    private const string kAnimatorMoveSpeedTag = "MoveSpeed";
    private const float kSquishAnimationTransitionTime = 0.1f; 
    private const float kInAirAnimationTransitionTime = 0.1f; 
    private const float kJumpAnimationTransitionTime = 0.1f; 
    private const float kLandAnimationTransitionTime = 0.01f; 
    private const float kReachedTargetThresh = 1.0f;
    private static readonly Vector3 kFullMassScale = Vector3.one;

    [Header("Connections")]
    [SerializeField]
    private CharacterController character;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private PlayerSlimeCamera playercam;
    [SerializeField]
    private PlayerSlimeRoot parent;
    [SerializeField]
    private Sync sync;

    [Header("Physics")]
    [SerializeField]
    private float mass = 1.0f;
    [SerializeField]
    private bool usemovecurve = false;
    [SerializeField]
    private MoveCurve movecurve;
    [SerializeField]
    private float standardgluetogrounddist = 0.4f;
    [SerializeField]
    private LayerMask groundmask;
    [SerializeField]
    private float gravity = 0.1f;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float stopfriction = 0.9f;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float stopfrictioninair = 0.96f;
    [SerializeField]
    private float maxspeed = 0.5f;
    [SerializeField]
    private float acceleration = 0.1f;
    [SerializeField]
    private float accelerationinair = 0.01f;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float rotationslerp = 0.1f;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float scalelerp = 0.1f;

    [Header("Abilities")]
    [SerializeField]
    private float standardstepoffset = 0.15f;
    [SerializeField]
    private float minjumpforce = 1.0f;
    [SerializeField]
    private float maxjumpforce = 5.0f;
    [SerializeField]
    private AnimationClip jumpsquishtimeclipref;
    [SerializeField]
    private float jumpsquishmovemultstart = 1.0f;
    [SerializeField]
    private float jumpsquishmovemultend = 0.15f;
    [SerializeField]
    private float invulnerabilitytime = 2.0f;
    [SerializeField]
    private float recallrotatespeed = 1.0f;
    [SerializeField]
    private float maxrecallspeed = 1.0f;

    private Vector2 movementinput;
    private Vector3 currentvelocity;
    private Vector3 currentgravity;
    private Vector3 lookdirection;
    private bool grounded = true;
    private ESlimeMoveMode movemode = ESlimeMoveMode.eFree;

    // -- recall
    private Vector3 randomrecallaxis;
    private Vector3 recalltarget;
    private bool reachedtarget = false;
    private float gluetogrounddist;

    private bool jumped = false;
    private bool issquishing = false;
    private float jumpsquishtime;
    private float jumpsquishtimer;
    private float lasthittime;

    public float Mass => mass;
    public float LastHitTime => lasthittime;
    public CharacterController Character => character;
    public bool Grounded => grounded;
    public bool ReachedTarget => reachedtarget;

    void Awake()
    {
        lookdirection = transform.forward;
        movecurve.Initialize();
        lasthittime = Time.time + invulnerabilitytime;

        jumpsquishtime = jumpsquishtimeclipref.length;
        jumpsquishtimer = jumpsquishtime;
    }

    void Update()
    {
        if(movemode == ESlimeMoveMode.eTarget)
        {
            return;
        }

        bool wasidle = movementinput == Vector2.zero;

        movementinput = PlayerInput.MovementInput();
        if(movementinput == Vector2.zero)
        {
            animator.SetFloat(kAnimatorMoveSpeedTag, 0.0f);
            movecurve.ResetEvaluation();
        }
        else
        {
            animator.SetFloat(kAnimatorMoveSpeedTag, 1.0f);
            movecurve.UpdateEvaluation(Time.deltaTime);

            if(wasidle)
            {
                OnStartMove();
            }
        }

        if (issquishing)
        {
            jumpsquishtimer -= Time.deltaTime;
            if (jumpsquishtimer < 0.0f)
                jumpsquishtimer = 0.0f;

            if(grounded)
            {
                if (PlayerInput.JumpEnd())
                {
                    Jump(GetJumpSquishRatio());
                }
            }
            else
            {
                issquishing = false;
                jumpsquishtimer = jumpsquishtime;
            }
        }
        else if(!jumped && grounded && PlayerInput.JumpStart())
        {
            OnSquishStart();
        }
    }

    void FixedUpdate()
    {
        if(movemode == ESlimeMoveMode.eTarget)
        {
            if (reachedtarget)
                return;

            transform.Rotate(randomrecallaxis * recallrotatespeed);
            Vector3 totarget = recalltarget - transform.position;
            if(totarget.magnitude <= kReachedTargetThresh)
            {
                reachedtarget = true;
                transform.position = recalltarget;
            }
            else
            {
                transform.position += totarget.normalized * maxrecallspeed;
            }
            return;
        }

        GetCapsuleCastData(out Vector3 p1, out Vector3 p2, out float maxdist, out float halfcapsule);
        Vector3 nudge = Vector3.up * Physics.defaultContactOffset;
        if (currentgravity.y <= 0.0f && Physics.CapsuleCast(p1 + nudge, p2 + nudge, character.radius, Vector3.down, out RaycastHit hit, maxdist, groundmask, QueryTriggerInteraction.Ignore))
        {
            if(!grounded)
            {
                OnGroundLand();
            }

            jumped = false;
            grounded = true;
            currentgravity = Vector3.zero;
        }
        else
        {
            if(grounded)
            {
                OnGroundLeave();
            }

            grounded = false;
            currentgravity += Vector3.down * gravity;
        }

        HandleMovement();
        HandleRotation();
        HandleScale();

        Vector3 animy = Vector3.up * movecurve.CurrentEvaluation.y;
        if (!usemovecurve)
            animy = Vector3.zero;

        CollisionFlags fallflags = character.Move(currentgravity + animy);
    }

    private void OnSquishStart()
    {
        issquishing = true;
        jumpsquishtimer = jumpsquishtime;
        animator.CrossFadeInFixedTime(AnimatorExtensions.GetStateName("Squish"), kSquishAnimationTransitionTime, AnimatorExtensions.kBaseLayerValue);
    }

    private float GetJumpSquishRatio()
    {
        float jumpsquishnt = 1.0f - Mathf.Clamp01(jumpsquishtimer / jumpsquishtime);
        return jumpsquishnt;
    }

    private void HandleRotation()
    {
        Quaternion targetrot = Quaternion.LookRotation(lookdirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrot, rotationslerp);
    }

    private void HandleMovement()
    {
        Vector3 rightnoy = playercam.transform.right.NoY().normalized;
        Vector3 forwardnoy = playercam.transform.forward.NoY().normalized;
        Vector3 animeval = new Vector3(movecurve.CurrentEvaluation.x, 0.0f, movecurve.CurrentEvaluation.z);
        if (!grounded)
            animeval = Vector3.zero;
        if (!usemovecurve)
            animeval = Vector3.one;

        float squishmult = 1.0f;
        if (issquishing)
        {
            squishmult = Mathf.Lerp(jumpsquishmovemultstart, jumpsquishmovemultend, GetJumpSquishRatio());
        }

        if (movementinput == Vector2.zero)
        {
            if(grounded)
                currentvelocity *= stopfriction;
            else
                currentvelocity *= stopfrictioninair;
        }
        else
        {
            Vector3 addmove = (forwardnoy * movementinput.y * animeval.z) + (rightnoy * movementinput.x * animeval.x);
            float accel = grounded ? acceleration : accelerationinair;
            currentvelocity += addmove * accel / mass;
            currentvelocity = Vector3.ClampMagnitude(currentvelocity, maxspeed) * squishmult;
        }

        if(currentvelocity != Vector3.zero)
            lookdirection = currentvelocity.normalized;

        CollisionFlags moveflags = character.Move(currentvelocity);
    }

    private void HandleScale()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, kFullMassScale * mass, scalelerp);
    }

    public void BeginRecall(Vector3 point)
    {
        movemode = ESlimeMoveMode.eTarget;
        character.enabled = false;
        recalltarget = point;
        reachedtarget = false;
        randomrecallaxis = RandomExtensions.RandomUnitVector3();
    }

    public void EndRecall()
    {
        movemode = ESlimeMoveMode.eFree;
        character.enabled = true;
        reachedtarget = true;
        lookdirection = playercam.transform.forward.NoY().normalized;
    }

    public void Jump(float squishratio)
    {
        float jumpforce = Mathf.Lerp(minjumpforce, maxjumpforce, squishratio);
        currentgravity = Vector3.up * jumpforce;
        issquishing = false;
        jumpsquishtimer = jumpsquishtime;

        animator.CrossFadeInFixedTime(AnimatorExtensions.GetStateName("Jump"), kJumpAnimationTransitionTime, AnimatorExtensions.kBaseLayerValue);
    }

    public void SpawnedFrom(PlayerSlime slime)
    {
        character.enabled = false;
        transform.position = slime.transform.position;
        transform.rotation = slime.transform.rotation;
        lookdirection = slime.lookdirection;
        character.enabled = true;
    }

    public void SetCamera(PlayerSlimeCamera camera)
    {
        playercam = camera;
    }

    public void SetMass(float newmass, bool immediate)
    {
        mass = newmass;
        character.stepOffset = standardstepoffset * mass;
        gluetogrounddist = standardgluetogrounddist * mass;
        animator.speed = 1.0f / mass;

        if (immediate)
        {
            transform.localScale = kFullMassScale * mass;
        }
    }

    public void SetLastHitTime(float hittime)
    {
        lasthittime = hittime;
    }

    public void Split()
    {
        parent.Split();
    }

    public void OnHit(Vector3 point, Vector3 normal)
    {
        if (lasthittime + invulnerabilitytime > Time.time)
            return;

        lasthittime = Time.time;
        Split();
    }

    private void OnStartMove()
    {
        animator.playbackTime = sync.OffSync;
    }

    private void OnGroundLand()
    {
        if (AnimatorExtensions.AnimatorLayerInState(animator, "Land"))
            return;

        animator.CrossFadeInFixedTime(AnimatorExtensions.GetStateName("Land"), kLandAnimationTransitionTime, AnimatorExtensions.kBaseLayerValue);
    }

    private void OnGroundLeave()
    {
        if (AnimatorExtensions.AnimatorLayerInState(animator, "Land"))
            return;

        animator.CrossFadeInFixedTime(AnimatorExtensions.GetStateName("InAir"), kInAirAnimationTransitionTime, AnimatorExtensions.kBaseLayerValue);
    }

    private void GetCapsuleCastData(out Vector3 p1, out Vector3 p2, out float maxdist, out float centertopoint)
    {
        PhysicsExtensions.GetCapsuleCastData(transform, character, currentgravity.magnitude, currentgravity.magnitude, out p1, out p2, out maxdist, out centertopoint);
        maxdist = Mathf.Max(maxdist, standardgluetogrounddist);
    }

    private void OnGUI()
    {
        GUIExtensions.GlobalHeader("PLAYER");
        GUIExtensions.GlobalLabel(string.Format("Grounded: {0}", grounded));
        GUIExtensions.GlobalLabel(string.Format("Jumped: {0}", jumped));
        GUIExtensions.GlobalLabel(string.Format("Move Speed: {0}", currentvelocity.magnitude.ToString("F2")));
        GUIExtensions.GlobalLabel(string.Format("Current Animation State: {0}", AnimatorExtensions.GetCurrentClipInfo(animator)));
    }
}

public enum ESlimeMoveMode
{
    eFree = 0,
    eTarget = 1
}