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
using Unity.Physics.Authoring;

public enum WeaponMotion
{
    None,
    Started,
    Raised,
    Lowering
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
    private float aimLerp = .03f;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public int playerId; // The Rewired player id of this character
    [SerializeField] private Transform crossHair;
    [SerializeField] private bool cursorTargeting = true;
    [Range(0.0f, 100.0f)]
    [SerializeField] private float cameraZ = 50f;
    public bool weapon2d;
    [SerializeField]
    private Vector3 crossHairOffset;
    [SerializeField] bool simController = false;
    [SerializeField]
    [Range(1.0f, 20.0f)] private float sensitivity = 5;
    private float aimAngle;
    private Animator animator;
    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);
        if (crossHair != null)
        {
            crossHairOffset = crossHair.transform.position;
        }
        Debug.Log("cr " + crossHairOffset);
        animator = GetComponent<Animator>();
        target = crossHair;//default target
    }


    public void SetAim()
    {
        Vector3 aimTarget = crossHair.transform.position;
        float xd = math.abs(transform.position.x - crossHair.transform.position.x);
        float yd = math.abs(transform.position.y - crossHair.transform.position.y);
        bool ik = true;
        if (weapon2d)
        {
         //   Debug.Log("x " + xd + "y " + yd);

            if (xd < 1 && yd < 2)
            {
                ik = false;
            }


            xd = math.sign(crossHair.transform.position.x) * 50;
            yd = math.sign(crossHair.transform.position.y) * 50;



            

            //if (yd < 2)
            //{
            //    yd = math.sign(crossHair.transform.position.y) * 5;
            //}
            //if(xd < 2 && yd < 2)
            //{
            //    ik = false;
            //}

            //Debug.Log("x0 " + xd + "y0 " + yd);
            aimTarget = new Vector3( crossHair.transform.position.x + xd, crossHair.transform.position.y + yd, 
                crossHair.transform.position.z);
            //aimTarget.Normalize();
            //crossHair.transform.position = aimTarget;

        }



        if (ik)
        {
            aim.solver.IKPositionWeight = aimWeight;
            aim.solver.IKPosition = aimTarget;
            aim.solver.Update();
        }

    }

    public void SetIK()
    {
        if (ik == null) return;
        ik.solver.Update();
    }


    public void LateUpdateSystem(WeaponMotion weaponMotion)
    {

        if (aim == null || target == null) return;
        Crosshair();
        aimWeight = 1;
        if (weaponMotion == WeaponMotion.None)
        {
            aimWeight = 0;
        }

        SetAim();
        SetIK();
    }


    void Crosshair()
    {

        if (cursorTargeting == false || crossHair == null) return;

        Controller controller = player.controllers.GetLastActiveController();
        if (controller == null && simController == false) return;

        Vector3 xHairPosition = new Vector3(transform.position.x + 0, transform.position.y + 2, transform.position.z);
        float x = 0;
        float y = 0;
        bool gamePad = false;
        if(controller != null)
        {
            if (controller.type == ControllerType.Joystick) gamePad = true;
        }
        if (simController) gamePad = true;

        if (gamePad)
        {
            Vector3 aim = Vector3.zero;
            crossHair.GetComponent<MeshRenderer>().enabled = true;

            x = player.GetAxis("RightHorizontal");
            y = player.GetAxis("RightVertical");

            aimAngle = transform.rotation.eulerAngles.y;

            aim = new Vector3(
                x * Time.deltaTime,
                y * Time.deltaTime,
                0
                );

            if (weapon2d == false)
            {
                aim = new Vector3(
                    x * Time.deltaTime,
                    0,
                    y * Time.deltaTime
                    );
            }

            crossHairOffset += aim;

        

            crossHair.transform.position = xHairPosition + crossHairOffset * sensitivity;
        }
        else
        {
            Vector3 mousePosition = player.controllers.Mouse.screenPosition;
            mousePosition.z = cameraZ;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 mouseDirection = worldPosition - transform.position;
            x = mouseDirection.x;
            y = mouseDirection.y;



            crossHairOffset = new Vector3(
                x,
                y ,
                0
            );


            crossHair.transform.position = transform.position + crossHairOffset * sensitivity;

            if (weapon2d == false)
            {
                //worldPosition.y = 1;
                crossHair.transform.position = worldPosition;
            }



        }


    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;

        dstManager.AddComponentData(entity,
            new PlayerWeaponAimComponent { weapon2d = weapon2d });
    }
}


