using RootMotion.FinalIK;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.AI;



public class EnemyWeaponAim : MonoBehaviour, IConvertGameObjectToEntity
{
    private NavMeshAgent agent;
    Animator animator;
    [SerializeField] AimIK aim;
    [SerializeField] private CCDIK ccdIK;
    public Transform target;
    public Transform aimTransform;
    public bool weaponRaised = false;
    [Range(0.0f, 1.0f)] [SerializeField] private float aimWeight = 1.0f;
    public CameraTypes weaponCamera;

    Entity entity;
    EntityManager manager;


    //public bool weaponRaisedEndState { get; private set; }
    void Start()
    {
        if (aim == null) 
        {
            Debug.LogWarning("AimIK required");
            return;
        }
        aimTransform = aim.solver.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetLayerWeight(0, 1);
        animator.SetLayerWeight(1, 0);
    }


    public void SetCCD()
    {
        if (ccdIK != null && target != null)
        {
            ccdIK.solver.IKPosition = target.position;
            //Debug.Log("CCD");
        }
    }

    public void SetAim()
    {
        if (aim == null || aimTransform == null || target == null || !agent.enabled) return;
        aim.solver.IKPositionWeight = weaponRaised ? aimWeight : 1;
        aim.solver.transform = aimTransform;
        aim.solver.IKPosition = target.position;
        aim.solver.Update();
    }

    public void SetAnimationLayerWeights()
    {
        if (animator == null) return;
        if (weaponRaised)
        {
            animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(1, 1); //1 is weapon layer
            //Debug.Log("aim");
            animator.SetBool("Aim", true);
        }
        else if (!weaponRaised)
        {
            //weaponRaisedEndState = false;
            if (aim)
            {
                aim.solver.IKPositionWeight = 0;
            }
            animator.SetLayerWeight(0, 1);
            animator.SetLayerWeight(1, 0);
            animator.SetBool("Aim", false);
        }
    }

    public void LateUpdateSystem()
    {

        SetCCD();
        SetAim();
    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        this.entity = entity;
        manager = dstManager;
        dstManager.AddComponentData(entity,
            new ActorWeaponAimComponent { weaponCamera = weaponCamera });


    }
}





