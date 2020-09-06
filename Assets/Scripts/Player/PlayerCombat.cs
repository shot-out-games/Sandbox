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
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ik = GetComponent<FullBodyBipedIK>();

        for (int i = 0; i < movesInspector.Moves.Count; i++)
        {
            Moves move = movesInspector.Moves[i];
            if (move.playerMove && move.active)
            {
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
        }



        if(aim != null) aim.enabled = false;
        if(ik != null) ik.enabled = false;

    }

    public void SelectMove(int combatAction)
    {
        if (moveList.Count < combatAction) return;

        moveUsing = moveList[combatAction - 1];
        aim = moveUsing.aimIk;
        AimTransform = moveUsing.aimTransform;
        if (aim != null)
        {
            if (AimTransform == null)
            {
                int boneCount = aim.solver.bones.Length;
                AimTransform = aim.solver.bones[boneCount - 1].transform;
            }
            moveUsing.aimTransform = AimTransform;
            //Debug.Log("at1 " + AimTransform);
        }

    }

    public void StartMove(int combatAction)
    {
        animator.SetInteger("CombatAction", combatAction);
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
            aim.solver.Update();
        }

        if (ik == null || moveUsing.target == null) return;

        if (moveUsing.usingFbb)
        {
            ik.solver.GetEffector(moveUsing.effector).position = moveUsing.target.position;
            ik.solver.GetEffector(moveUsing.effector).positionWeight = hitWeight * hitWeight;
            ik.solver.Update();
        }

    }

    private void LateUpdate()
    {
        if (moveList.Count == 0) return;
        Aim();

    }

    public void EndAttack()
    {
        //Debug.Log("attack stage end");
        AttackStage = AttackStages.End;//
        animator.SetInteger("CombatAction", 0);

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        _entity = entity;
        _manager = dstManager;

        _manager.AddComponentData(_entity, new MeleeComponent
        {
            Available = active, hitPower = hitPower,
            gameHitPower = hitPower
        });


    }
}

