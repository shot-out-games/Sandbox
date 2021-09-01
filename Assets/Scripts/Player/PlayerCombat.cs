using System.Collections;
using Unity.Entities;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.AI;


public class PlayerCombat : MonoBehaviour, IConvertGameObjectToEntity, ICombat
{
    public MovesManager movesInspector;
    public Transform AimTransform;
    private Animator animator;
    private Rigidbody rb;
    private List<Moves> moveList = new List<Moves>();
    public Moves moveUsing = new Moves();
    [HideInInspector]
    public FullBodyBipedIK ik;
    [HideInInspector]
    public AimIK aim;
    private Entity _entity;
    private EntityManager _manager;
    public AttackStages AttackStage { get; set; }

    [SerializeField]
    private bool active = true;
    [SerializeField]
    private float hitPower = 100;

    [SerializeField] private float fbbIKUseDistance = 8;
    //public bool haraKiri = false;
    //public Transform haraKiriTarget;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ik = GetComponent<FullBodyBipedIK>();

        for (int i = 0; i < movesInspector.Moves.Count; i++)
        {
            Moves move = movesInspector.Moves[i];
            if (move.aimIk != null)
            {
                aim = move.aimIk;
                AimTransform = move.aimTransform;
                if (AimTransform == null)
                {
                    int boneCount = aim.solver.bones.Length;
                    AimTransform = aim.solver.bones[boneCount - 1].transform;
                }
            }
            move.target = moveUsing.target;//default target assigned in system
            moveList.Add(move);

        }



        if (aim != null) aim.enabled = false;
        if (ik != null) ik.enabled = false;

    }

    public void SelectMove(int combatAction)
    {
        if (moveList.Count <= 0) return;

        int animationIndex = -1;

        for (int i = 0; i < moveList.Count; i++)
        {
            if ((int)moveList[i].animationType == combatAction)
            {
                moveUsing = moveList[i];
                animationIndex = (int)moveUsing.animationType;
            }
        }

        if (animationIndex <= 0) return;//0 is none on enum


        aim = moveUsing.aimIk;
        AimTransform = moveUsing.aimTransform;
        if (aim != null)
        {
            if (AimTransform == null)
            {
                int boneCount = aim.solver.bones.Length;
                AimTransform = aim.solver.bones[boneCount - 1].transform;
                Debug.Log("aim ik auto bone ");
            }
            moveUsing.aimTransform = AimTransform;
        }

        if (moveUsing.moveAudioSource && moveUsing.moveAudioClip && moveUsing.moveAudioSource.isPlaying == false)
        {
            moveUsing.moveAudioSource.clip = moveUsing.moveAudioClip;
            moveUsing.moveAudioSource.Play();
        }
        if (moveUsing.moveParticleSystem)
        {
            moveUsing.moveParticleSystem.Play(true);
        }

        StartMove(animationIndex);

        //if (combatAction == 3)
        //{
        // moveUsing.target = transform;
        //}
        //Debug.Log("at1 " + AimTransform);


    }

    public void StartMove(int animationIndex)
    {
        if (moveList.Count <= 0) return;


        animator.SetInteger("CombatAction", animationIndex);
        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(2, 0);

        StartIKPlayer();
    }

    public void StartIKPlayer()
    {
        if (moveUsing.usingAim)
        {
            aim.enabled = true;
        }
        if (moveUsing.usingFbb)
        {
            ik.enabled = true;
        }

    }

    public void StopIKPlayer()
    {
        animator.SetFloat("HitWeight", 0);

        if (moveUsing.usingFbb)
        {
            ik.solver.GetEffector(moveUsing.effector).positionWeight = 0;
        }

        if (moveUsing.moveParticleSystem)
        {
            moveUsing.moveParticleSystem.Stop(true);
        }

        //haraKiri = false;
        if (moveUsing.usingAim)
        {
            aim.enabled = false;
        }
        if (moveUsing.usingFbb)
        {
            ik.enabled = false;
        }

    }

    float AdjustWeightToDistanceFromTarget(float weight)
    {
        float distance = Vector3.Distance(transform.position, moveUsing.target.position);
        //Debug.Log("dist " + distance);
        float adjustedWeight = weight;
        if (distance > fbbIKUseDistance) //change to member for fbbik start using distance
        {
            adjustedWeight = 0;
        }

        return adjustedWeight;

    }

    public void Aim()
    {

        float hitWeight = animator.GetFloat("HitWeight");
        int combatAction = animator.GetInteger("CombatAction");
        if (aim == null || combatAction == 0 || moveUsing.target == null) return;

        if (moveUsing.usingAim)
        {
            aim.solver.transform = moveUsing.aimTransform;
            aim.solver.transform.LookAt(moveUsing.pin.position);
            aim.solver.IKPosition = moveUsing.target.position;
            aim.solver.IKPositionWeight = hitWeight * moveUsing.weight;
            //if (haraKiri == true)
            //{
            // aim.solver.IKPosition = haraKiriTarget.position;
            //    //aim.solver.IKPositionWeight = 1;
            //}

            aim.solver.Update();
        }

        if (ik == null || moveUsing.target == null) return;

        if (moveUsing.usingFbb && ik.enabled)
        {
            float adjHitWeight = AdjustWeightToDistanceFromTarget(hitWeight);
            //Debug.Log("ik " + " hw " + hitWeight + " move weight " + moveUsing.weight);
            ik.solver.GetEffector(moveUsing.effector).position = moveUsing.target.position;
            ik.solver.GetEffector(moveUsing.effector).positionWeight = adjHitWeight * moveUsing.weight;

            //if (haraKiri == true)
            //{
            //    ik.solver.GetEffector(moveUsing.effector).position = haraKiriTarget.position;
            //    ik.solver.GetEffector(moveUsing.effector).positionWeight = hitWeight * moveUsing.weight;
            //}


            ik.solver.Update();
        }

    }

    public void LateUpdateSystem()
    {
        if (moveList.Count == 0) return;
        Aim();

    }

    public void EndAttack()
    {
        Debug.Log("attack stage end");
        AttackStage = AttackStages.End;//
        //animator.SetInteger("CombatAction", 0);
        StopIKPlayer();
        if (_manager.HasComponent<CheckedComponent>(_entity))
        {
            var checkedComponent = _manager.GetComponentData<CheckedComponent>(_entity);
            checkedComponent.collisionChecked = false;
            _manager.SetComponentData(_entity, checkedComponent);
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        _entity = entity;
        _manager = dstManager;

        _manager.AddComponentData(_entity, new MeleeComponent
        {
            Available = active,
            hitPower = hitPower,
            gameHitPower = hitPower
        });


    }
}

