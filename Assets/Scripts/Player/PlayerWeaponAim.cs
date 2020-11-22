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
using  Unity.Physics.Authoring;

public enum WeaponMotion
{
    None,
    Started,
    Raised
}

public struct PlayerWeaponAimComponent : IComponentData
{
    public WeaponMotion weaponRaised;
    public float weaponUpTimer;
    public bool autoTarget;
    public bool dualMode;
    public bool cursorTargeting;
    public bool weapon2d;

}

public class PlayerWeaponAim : MonoBehaviour, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity e;

    [SerializeField] AimIK aim;
    [SerializeField] FullBodyBipedIK ik;
    [HideInInspector]
    public Transform target;
    public Transform aimTransform;
    [Range(0.0f, 1.0f)] [SerializeField] private float aimWeight = 1.0f;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public int playerId; // The Rewired player id of this character
    [SerializeField] private Transform crossHair;
    [SerializeField] private bool cursorTargeting = true;
    [Range(0.0f, 100.0f)]
    [SerializeField] private float cameraZ = 50f;
    public bool weapon2d;


    private float aimAngle;
    private Animator animator;
    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);
        animator = GetComponent<Animator>();
        target = crossHair;//default target
    }


    public void SetAim()
    {

        aim.solver.IKPositionWeight = aimWeight;
        aim.solver.IKPosition = crossHair.transform.position;
        aim.solver.Update();

    }

    public void SetIK()
    {
        if (ik == null) return;
        ik.solver.Update();
    }


    public void LateUpdateSystem()
    {

        if (aim == null || target == null) return;
        Crosshair();
        SetAim();
        SetIK();
    }


    void Crosshair()
    {

        if (cursorTargeting == false || crossHair == null) return;

        Controller controller = player.controllers.GetLastActiveController();
        if (controller == null) return;

        if (controller.type == ControllerType.Joystick)
        {
            crossHair.GetComponent<MeshRenderer>().enabled = true;
            float x = player.GetAxis("RightHorizontal");
            float y = player.GetAxis("RightVertical");
            aimAngle = transform.rotation.eulerAngles.y;

            //if (math.abs(aimAngle) > 22.5 && math.abs(aimAngle) <= 67.5)
            //{
            //    x = 1;
            //    y = 1;
            //}
            //else if (math.abs(aimAngle) > 67.5 && math.abs(aimAngle) <= 112.5)
            //{
            //    x = 1;
            //    y = 0;
            //}
            //else if (math.abs(aimAngle) > 112.5 && math.abs(aimAngle) <= 157.5)
            //{
            //    x = 1;
            //    y = -1;
            //}
            //else if (math.abs(aimAngle) > 157.5 && math.abs(aimAngle) <= 202.5)
            //{
            //    x = 0;
            //    y = -1;
            //}
            //else if (math.abs(aimAngle) > 202.5 && math.abs(aimAngle) <= 247.5)
            //{
            //    x = -1;
            //    y = -1;
            //}
            //else if (math.abs(aimAngle) > 247.5 && math.abs(aimAngle) <= 292.5)
            //{
            //    x = -1;
            //    y = 0;
            //}
            //else if (math.abs(aimAngle) > 292.5 && math.abs(aimAngle) <= 337.5)
            //{
            //    x = -1;
            //    y = 1;
            //}
            //else if (math.abs(aimAngle) > 337.5 && math.abs(aimAngle) <= 22.5)
            //{
            //    x = 0;
            //    y = 1;
            //}

            Debug.Log("ang " + (int)aimAngle);

            //x = math.sign(x);
            //y = math.sign(y);

            Vector3 aim = new Vector3(
                    x,
                    y,
                    0
                );

                crossHair.transform.position = transform.position + aim * 5f;
        }
        else
        {
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
            new PlayerWeaponAimComponent {weapon2d = weapon2d});
    }
}


