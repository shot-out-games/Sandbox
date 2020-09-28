using System;
using System.Collections.Generic;
using RootMotion.FinalIK;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Rewired;
using Rewired.ComponentControls;
using Unity.Mathematics;
using Unity.Transforms;



public struct PlayerWeaponAimComponent : IComponentData
{
    public bool weaponRaised;
    public float weaponUpTimer;
    public bool autoTarget;
    public bool dualMode;
    public bool cursorTargeting;

}

public class PlayerWeaponAim : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] AimIK aim;
    [SerializeField] FullBodyBipedIK ik;
    [HideInInspector]
    public Transform target;
    public Transform aimTransform;
    public bool weaponRaised = false;
    [Range(0.0f, 1.0f)] [SerializeField] private float aimWeight = 1.0f;
    private EntityManager manager;
    private Entity e;
    //[SerializeField]
    private bool dualMode;
    //[SerializeField]
    public Player player;
    public int playerId; // The Rewired player id of this character
    [SerializeField] private bool cursorTargeting = true;
    [SerializeField] private GameObject crossHair;
    //[SerializeField] private float crossHairRange = 18.0f;
    [Range(0.0f, 100.0f)]
    [SerializeField] private float cameraZ = 50f;

    private float aimAngle;

    //[SerializeField] private TouchJoystick touchJoystick;

    private Animator animator;
    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);
        animator = GetComponent<Animator>();
    }


    public void SetAim()
    {
        //float autoWeight = aimWeight;
        //var playerWeaponAim = manager.GetComponentData<PlayerWeaponAimComponent>(e);
        //if (playerWeaponAim.autoTarget == false)
        //{
        //    autoWeight = 0;
        //}


        //aim.solver.IKPositionWeight = weaponRaised ? autoWeight : 0;


        //aim.solver.transform = aimTransform;

        //if (cursorTargeting == true && crossHair != null)
        //{
        //    target = crossHair.transform;
        //    target.position = crossHair.transform.position;
        //}

        //aim.solver.IKPosition = target.position;
        //aim.solver.Update();

        aim.solver.IKPositionWeight = aimWeight;
        aim.solver.IKPosition = crossHair.transform.position;
        //aim.solver.IKPosition = target.position;
        //aim.solver.IKPosition = transform.forward;
        aim.solver.Update();



    }

    public void SetIK()
    {
        if (ik == null) return;
        ik.solver.Update();
    }


    public void LateUpdateSystem()
    {

        if (manager == default) return;
        if (manager.HasComponent<PlayerWeaponAimComponent>(e) == false) return;
        if (manager.HasComponent<ApplyImpulseComponent>(e) == false) return;
        if (aim == null || target == null) return;
        Crosshair();
        SetAim();
        SetIK();

        //Crosshair();
    }

    public void UpdateSystem()
    {

        if (manager == default) return;
        if (manager.HasComponent<PlayerWeaponAimComponent>(e) == false) return;
        if (aim == null || target == null) return;

        //Crosshair();

        //crossHair.transform.position = new Vector3(crossHair.transform.position.x, crossHair.transform.position.y, 0);

    }


    void Crosshair()
    {
        //Transform targetTransform = animator.GetBoneTransform(HumanBodyBones.Neck);

        if (cursorTargeting == false || crossHair == null) return;

        Controller controller = player.controllers.GetLastActiveController();
        if (controller == null) return;

        if (controller.type == ControllerType.Joystick)
        {
            crossHair.GetComponent<MeshRenderer>().enabled = true;

            //Quaternion entityRotation = manager.GetComponentData<LocalToWorld>(e).Rotation;
            //transform.rotation = entityRotation; 
            var applyImpulse = manager.GetComponentData<ApplyImpulseComponent>(e);


            //float x = player.GetAxis("Move Horizontal");
            //float y = player.GetAxis("Move Vertical");
            float x = applyImpulse.stickX;
            float y = applyImpulse.stickY;
            //Debug.Log("x " + Math.Ceiling(x * 10) + " y " + Math.Ceiling(y * 10));
            //Debug.Log("x " + x + " y " + y);

            aimAngle = transform.rotation.eulerAngles.y;

            if (math.abs(aimAngle) > 22.5 && math.abs(aimAngle) <= 67.5)
            {
                x = 1;
                y = 1;
            }
            else if (math.abs(aimAngle) > 67.5 && math.abs(aimAngle) <= 112.5)
            {
                x = 1;
                y = 0;
            }
            else if (math.abs(aimAngle) > 112.5 && math.abs(aimAngle) <= 157.5)
            {
                x = 1;
                y = -1;
            }
            else if (math.abs(aimAngle) > 157.5 && math.abs(aimAngle) <= 202.5)
            {
                x = 0;
                y = -1;
            }
            else if (math.abs(aimAngle) > 202.5 && math.abs(aimAngle) <= 247.5)
            {
                x = -1;
                y = -1;
            }
            else if (math.abs(aimAngle) > 247.5 && math.abs(aimAngle) <= 292.5)
            {
                x = -1;
                y = 0;
            }
            else if (math.abs(aimAngle) > 292.5 && math.abs(aimAngle) <= 337.5)
            {
                x = -1;
                y = 1;
            }
            else if (math.abs(aimAngle) > 337.5 && math.abs(aimAngle) <= 22.5)
            {
                x = 0;
                y = 1;
            }


            //if (math.abs(aimAngle) < 22.5f || math.abs(aimAngle) > 315) x = 0;
            //else if (math.abs(aimAngle) > 157.5 && math.abs(aimAngle) < 202.5) x = 0;
            //else if (aimAngle > 180) x = -1;
            //else if (aimAngle < 180) x = 1;

            //x = math.sign(aimAngle);



            Vector3 aim = new Vector3(
                    x,
                    y,
                    //y + .15f * math.sign(y),
                    0
                );


            //Debug.Log("x " + x + " y " + y);

            //if (aim.sqrMagnitude > 0)
            //{

                crossHair.transform.position = transform.position + aim * 5f;
            //}
        }
        else
        {
            //Debug.Log("tr " + targetTransform.position);
            //Debug.Log("tr");


            Vector3 mousePosition = player.controllers.Mouse.screenPosition;
            mousePosition.z = cameraZ;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            crossHair.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
        }


    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;

        dstManager.AddComponentData(entity,
            new PlayerWeaponAimComponent { weaponRaised = false, weaponUpTimer = 0, autoTarget = false, dualMode = dualMode });
    }
}


