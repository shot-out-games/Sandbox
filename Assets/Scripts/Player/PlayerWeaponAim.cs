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

public enum CameraType
{
    ThreeD,
    TwoD,
    ThirdPerson
}

public struct PlayerWeaponAimComponent : IComponentData
{
    public WeaponMotion weaponRaised;
    public float weaponUpTimer;
    public bool autoTarget;
    public bool dualMode;
    public bool cursorTargeting;
    public CameraType weaponCamera;
    
}

public class PlayerWeaponAim : MonoBehaviour, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity e;
    private Camera cam;

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
    public CameraType weaponCamera;
    [SerializeField]
    private Vector3 crossHairOffset;
    [SerializeField] bool simController = false;
    [SerializeField]
    [Range(1.0f, 20.0f)] private float mouseSensitivity = 5;
    [SerializeField]
    [Range(1.0f, 20.0f)] private float gamePadSensitivity = 5;

    [SerializeField] private float minY = 1;
    [Range(80f, 100.0f)]
    [SerializeField] private float viewportPct =  90;

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

        cam = Camera.main;
    }


    public void SetAim()
    {
        Vector3 aimTarget = crossHair.transform.position;
        float xd = math.abs(transform.position.x - crossHair.transform.position.x);
        float yd = math.abs(transform.position.y - crossHair.transform.position.y);
        bool ik = true;
        if (weaponCamera == CameraType.TwoD)
        {
         //   Debug.Log("x " + xd + "y " + yd);

            if (xd < 1 && yd < 2)
            {
                ik = false;
            }


            xd = math.sign(crossHair.transform.position.x) * 50;
            yd = math.sign(crossHair.transform.position.y) * 50;

            aimTarget = new Vector3( crossHair.transform.position.x + xd, crossHair.transform.position.y + yd, 
                crossHair.transform.position.z);

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

        Vector3 xHairPosition = new Vector3(cam.transform.position.x, cam.transform.position.y + 2, cam.transform.position.z);
        Vector3 startCrossHairPosition = crossHair.position;
        Vector3 startCrossHairOffset = crossHairOffset;
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

            x = player.GetAxis("RightHorizontal") ;
            if (math.abs(x) < .19) x = 0;
            y = player.GetAxis("RightVertical");
            if (math.abs(y) < .19) y = 0;


            //aimAngle = transform.rotation.eulerAngles.y;

            aim = new Vector3(
                x * Time.deltaTime,
                y * Time.deltaTime,
                0
                );

            if (weaponCamera == CameraType.ThreeD)
            {
                aim = new Vector3(
                    x * Time.deltaTime,
                    0,
                    y * Time.deltaTime
                );


                
                aim =  cam.transform.TransformDirection(aim);
                aim.y = 0;

            }
            else if (weaponCamera == CameraType.ThirdPerson)
            {
                aim = new Vector3(
                    x * Time.deltaTime,
                    y * Time.deltaTime,
                    //x,
                    //y,
                    0
                );


                //aim =  cam.transform.TransformDirection(aim);
                aim.z = 0;
                aim.Normalize();

            }

            crossHairOffset += aim;

            //crossHairOffset = cam.transform.TransformDirection(crossHairOffset);
            //crossHairOffset.z = 0;


            var right = cam.transform.right;
            var up = cam.transform.up;
            var fwd = cam.transform.forward;

            Vector3 add = (crossHairOffset.x * right * gamePadSensitivity  + crossHairOffset.y * up * gamePadSensitivity  + fwd * cameraZ);

            var camPos = cam.transform.position;
            add = new Vector3(camPos.x + crossHairOffset.x, camPos.y + crossHairOffset.y, cameraZ);
            crossHair.transform.position = add;


            //crossHair.transform.position = xHairPosition + crossHairOffset * gamePadSensitivity / 10;
            if (weaponCamera == CameraType.ThirdPerson)
            {
                
                //crossHair.transform.position += cam.transform.forward * cameraZ;
            }

        }
        else
        {
            Vector3 mousePosition = player.controllers.Mouse.screenPosition;
            mousePosition.z = cameraZ;
            Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);
            //Vector3 mouseDirection = worldPosition - cam.transform.position;
            //x = mouseDirection.x;
            //y = mouseDirection.y;
            x = worldPosition.x;
            y = worldPosition.y;


            crossHairOffset = new Vector3(
                x,
                y ,
                0
            );


            //crossHair.transform.position = cam.transform.position + crossHairOffset * mouseSensitivity;

            var add = cam.transform.TransformDirection(Vector3.forward);
            add.Normalize();
            //Debug.Log("add " + add.z * cameraZ);
            crossHairOffset.z = cam.transform.position.z + add.z * cameraZ;
            crossHair.transform.position = crossHairOffset;
            crossHair.transform.position += cam.transform.forward;


            //if (weaponCamera != CameraType.TwoD)
            //{
            //worldPosition.y = 1;
            // crossHair.transform.position = worldPosition;
            //}



        }

        var position = crossHair.transform.position;

        //Vector3 viewPos = cam.WorldToViewportPoint(position);
        //Debug.Log("vp ");

        //float f = viewportPct / 100;
        //if (viewPos.y > f)
        //{
        //    position.y = startCrossHairPosition.y;
        //}
        //if (viewPos.x > f)
        //{
        //    position.x = startCrossHairPosition.x;
        //}
        //else if (viewPos.x < (1 - f))
        //{
        //    position.x = startCrossHairPosition.x;
        //}

        position.y = position.y < minY ? minY : position.y;
        crossHair.transform.position = position;


    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;

        dstManager.AddComponentData(entity,
            new PlayerWeaponAimComponent { weaponCamera = weaponCamera});
    }
}


