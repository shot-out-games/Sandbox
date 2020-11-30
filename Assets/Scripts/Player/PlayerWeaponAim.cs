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
using UnityEngine.UI;

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
    [SerializeField] private Image crosshairImage;
    [SerializeField] private bool cursorTargeting = true;
    [Range(0.0f, 100.0f)]
    [SerializeField] private float cameraZ = 50f;
    public CameraType weaponCamera;
    //[SerializeField]
    //private Vector3 crossHairOffset;
    [SerializeField] bool simController = false;
    [SerializeField]
    [Range(1.0f, 100.0f)] private float mouseSensitivity = 5;
    [SerializeField]
    [Range(1.0f, 100.0f)] private float gamePadSensitivity = 5;

    [SerializeField] private float topDownY = 1;
    [Range(80f, 100.0f)]
    [SerializeField] private float viewportPct = 90;
    [SerializeField]
    private Vector3 mousePosition;

    private float aimAngle;
    private Animator animator;
    [SerializeField] private float calc;
    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);
        //if (crossHair != null)
        //{
        // crossHairOffset = crossHair.transform.position;
        //}

        mousePosition = new Vector2(Screen.width / 2f, Screen.height / 2f);

        //Debug.Log("cr " + crossHairOffset);
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

            aimTarget = new Vector3(crossHair.transform.position.x + xd, crossHair.transform.position.y + yd,
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
        Vector3 worldPosition = Vector3.zero;
        float x = 0;
        float y = 0;
        float z = 0;
        bool gamePad = false;
        if (controller != null)
        {
            if (controller.type == ControllerType.Joystick) gamePad = true;
        }
        if (simController) gamePad = true;

        if (gamePad)
        {
            Vector3 aim = Vector3.zero;
            //crossHair.GetComponent<MeshRenderer>().enabled = true;

            x = player.GetAxis("RightHorizontal");
            if (math.abs(x) < .19) x = 0;
            y = player.GetAxis("RightVertical");
            if (math.abs(y) < .19) y = 0;

            aim = new Vector3(
                x * Time.deltaTime,
                y * Time.deltaTime,
                0
            );

            aim.Normalize();

            if (weaponCamera == CameraType.ThreeD)
            {
                aim = new Vector3(
                    x * Time.deltaTime,
                    y * Time.deltaTime,
                    0
                );


                aim.Normalize();
                mousePosition += new Vector3(aim.x * gamePadSensitivity, aim.y * gamePadSensitivity, 0);
                mousePosition.z = cameraZ;

            }
            else if (weaponCamera == CameraType.ThirdPerson || weaponCamera == CameraType.TwoD)
            {
                aim = new Vector3(
                    x * Time.deltaTime,
                    y * Time.deltaTime,
                    0
                );

                aim.Normalize();
                mousePosition += new Vector3(aim.x * gamePadSensitivity, aim.y * gamePadSensitivity, 0);
                mousePosition.z = cameraZ;


            }


            worldPosition = cam.ScreenToWorldPoint(mousePosition);
            x = worldPosition.x;
            y = worldPosition.y;
            z = worldPosition.z;


            Vector3 targetPosition = new Vector3(
                x,
                y,
                z
            );


            crossHair.transform.position = targetPosition;
            crosshairImage.transform.position = mousePosition;



            //crossHair.transform.position = xHairPosition + crossHairOffset * gamePadSensitivity / 10;
            //if (weaponCamera == CameraType.ThirdPerson)
            //{

            //crossHair.transform.position += cam.transform.forward * cameraZ;
            //}

        }
        else
        {
            mousePosition = player.controllers.Mouse.screenPosition;

            if (weaponCamera == CameraType.ThirdPerson)
            {


                mousePosition.z = cameraZ;
                worldPosition = cam.ScreenToWorldPoint(mousePosition);
                x = worldPosition.x;
                y = worldPosition.y;
                z = worldPosition.z;
            }

            if (weaponCamera == CameraType.ThreeD)
            {
                mousePosition.z = cameraZ;
                worldPosition = cam.ScreenToWorldPoint(mousePosition);
                x = worldPosition.x;
                z = worldPosition.z;
                y = transform.position.y +  topDownY;
                //mousePosition = cam.WorldToScreenPoint(new Vector3(x, z, mousePosition.z));
            }



            Vector3 targetPosition = new Vector3(
                x,
                y,
                z
            );

            if (weaponCamera == CameraType.ThreeD)
            {
                mousePosition = cam.WorldToScreenPoint(targetPosition);
            }


            crossHair.transform.position = targetPosition;
            crosshairImage.transform.position = mousePosition;
        }

        var position = crossHair.transform.position;
        crossHair.transform.position = position;


    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;

        dstManager.AddComponentData(entity,
            new PlayerWeaponAimComponent { weaponCamera = weaponCamera });
    }
}


