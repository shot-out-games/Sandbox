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

public enum CameraTypes
{
    TopDown,
    TwoD,
    ThirdPerson
}

public struct ActorWeaponAimComponent : IComponentData
{
    public WeaponMotion weaponRaised;
    public float weaponUpTimer;
    public bool autoTarget;
    public bool dualMode;
    public bool cursorTargeting;
    public CameraTypes weaponCamera;


}

public class PlayerWeaponAim : MonoBehaviour, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity e;
    private Camera cam;



    [SerializeField] AimIK aim;
    [SerializeField] FullBodyBipedIK ik;
    [SerializeField] private LookAtIK lookAtIk;
    [HideInInspector]
    public Transform target;
    public Transform aimTransform;
    [Range(0.0f, 1.0f)] [SerializeField] private float aimWeight = 1.0f;
    [Range(0.0f, 1.0f)] [SerializeField] private float clampWeight = 0.1f;
    [Range(0.0f, 1.0f)] [SerializeField] private float lookWeight = 1f;
    [SerializeField]
    float lerpSpeed = 3;
    private float startAimWeight;
    [SerializeField]
    private float currentAimWeight;
    [SerializeField]
    float targetAimWeight;
    float startClampWeight;
    private float startLookWeight;
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
    public CameraTypes weaponCamera;
    [SerializeField] bool simController = false;
    [SerializeField]
    [Range(1.0f, 100.0f)] private float mouseSensitivity = 5;
    [SerializeField]
    [Range(0.0f, 100.0f)] private float gamePadSensitivity = 5;

    [SerializeField] private float topDownY = 1;
    [Range(80f, 100.0f)]
    [SerializeField] private float viewportPct = 90;
    [SerializeField]
    private Vector3 mousePosition;
    private float aimAngle;
    private Animator animator;
    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    Vector3 targetPosition = Vector3.zero;
    Vector3 worldPosition = Vector3.zero;
    public Vector3 closetEnemyWeaponTargetPosition;
    
    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);
        animator = GetComponent<Animator>();
        target = crossHair;//default target

        cam = Camera.main;
        startAimWeight = aimWeight;
        startClampWeight = clampWeight;
        startLookWeight = lookWeight;
        currentAimWeight = lookWeight;
        targetAimWeight = startClampWeight;

        SetCursorBounds();

    }

    void SetCursorBounds()
    {
        mousePosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
        xMin = Screen.width * (1 - viewportPct / 100);
        xMax = Screen.width * viewportPct / 100;
        yMin = Screen.height * (1 - viewportPct / 100);
        yMax = Screen.height * viewportPct / 100;
    }



    public void SetAim()
    {
        Vector3 aimTarget = crossHair.transform.position;
        float xd = math.abs(transform.position.x - crossHair.transform.position.x);
        float yd = math.abs(transform.position.y - crossHair.transform.position.y);
        bool ik = true;
        if (weaponCamera == CameraTypes.TwoD)
        {
            if (xd < 1 && yd < 2)
            {
                ik = false;
            }

            xd = math.sign(crossHair.transform.position.x) * 50;
            yd = math.sign(crossHair.transform.position.y) * 50;

            aimTarget = new Vector3(crossHair.transform.position.x + xd, crossHair.transform.position.y + yd,
                crossHair.transform.position.z);

        }


        if (ik && lookAtIk)
        {
            lookAtIk.solver.clampWeight = clampWeight;
            lookAtIk.solver.IKPositionWeight = lookWeight;
            //lookAtIk.solver.IKPosition = aimTarget;
            lookAtIk.solver.IKPosition = Vector3.Lerp(lookAtIk.solver.IKPosition, aimTarget, Time.deltaTime * lerpSpeed);
            lookAtIk.solver.Update();
        }

        if (ik && aim)
        {
            aim.solver.clampWeight = clampWeight;
            aim.solver.IKPosition = Vector3.Lerp(aim.solver.IKPosition, aimTarget, Time.deltaTime * lerpSpeed);

            if (targetAimWeight == 0 && currentAimWeight > Time.deltaTime * lerpSpeed)
            {
                currentAimWeight = math.lerp(currentAimWeight, targetAimWeight, Time.deltaTime * lerpSpeed);
                aim.solver.IKPositionWeight = currentAimWeight;
            }
            else
            {
                targetAimWeight = startAimWeight;
                currentAimWeight = aimWeight;
                aim.solver.IKPositionWeight = currentAimWeight;
            }
            //aim.solver.IKPosition = aimTarget;
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

        if (target == null) return;
        Crosshair();
        aimWeight = startAimWeight;
        clampWeight = startClampWeight;

        if (targetAimWeight != 0)
        {
            lookWeight = startLookWeight;
        }

        Vector3 targetDir = crossHair.position - transform.position;
        float distance = targetDir.magnitude;
        float angle = Vector3.Angle(targetDir, transform.forward);
        if (angle > (180 - clampWeight * 360) || weaponMotion == WeaponMotion.None)
        {
            targetAimWeight = 0;
            //aimWeight = 0;
            clampWeight = 1;
            lookWeight = 0;
            //return;
        }



        SetAim();
        SetIK();
    }


    Vector3 GetMousePositionTopDownPlane()
    {
        //Plane plane = new Plane();
        Plane plane = new Plane(Vector3.down, transform.position);
        Ray r = cam.ScreenPointToRay(mousePosition);
        float d = 0;
        if (plane.Raycast(r, out d))
        {
            Vector3 v = r.GetPoint(d);
            return v;
        }
        else
        {
            Vector3 v = Vector3.forward; 
            //Debug.Log("ray " + v);
            return v;
        }
        //throw new UnityException("Mouse position ray not intersecting launcher plane");
    }

    Vector3 GetMousePositionThirdPersonPlane()
    {
        //Plane plane = new Plane();
        //Vector3 _distanceFromCamera = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z - cameraZ);

        Plane plane = new Plane(transform.forward, transform.forward * cameraZ);
        Ray r = cam.ScreenPointToRay(mousePosition);
        float d = 0;
        if (plane.Raycast(r, out d))
        {
            Vector3 v = r.GetPoint(d);
            return v;
        }

        plane = new Plane(transform.right, transform.right * cameraZ);
        if (plane.Raycast(r, out d))
        {
            Vector3 v = r.GetPoint(d);
            return v;
        }

      
        throw new UnityException("Mouse position ray not intersecting launcher plane");
    }

    void Crosshair()
    {

        if (cursorTargeting == false || crossHair == null) return;

        Controller controller = player.controllers.GetLastActiveController();
        if (controller == null && simController == false) return;
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
            if (math.abs(x) < .15) x = 0;
            y = player.GetAxis("RightVertical");
            if (math.abs(y) < .15) y = 0;

            aim = new Vector3(
                x * Time.deltaTime,
                y * Time.deltaTime,
                0
            );

            aim.Normalize();
            mousePosition += new Vector3(aim.x * gamePadSensitivity, aim.y * gamePadSensitivity, 0);
        }
        else
        {
            mousePosition = player.controllers.Mouse.screenPosition;
        }

        if (weaponCamera == CameraTypes.TwoD)
        {
            mousePosition.z = cameraZ;
            worldPosition = cam.ScreenToWorldPoint(mousePosition);
            x = worldPosition.x;
            y = worldPosition.y;
            z = 0;

            targetPosition = new Vector3(
                x,
                y,
                z
            );

        }
        if (weaponCamera == CameraTypes.ThirdPerson)
        {
            mousePosition.z = cameraZ;
            worldPosition = cam.ScreenToWorldPoint(mousePosition);
            //worldPosition = GetMousePositionThirdPersonPlane();
            x = worldPosition.x;
            y = worldPosition.y;
            z = worldPosition.z;

            targetPosition = new Vector3(
                x,
                y,
                z
            );


        }
        if (weaponCamera == CameraTypes.TopDown)
        {
            mousePosition.z = cameraZ;
            worldPosition = GetMousePositionTopDownPlane();
            x = worldPosition.x;
            //y = transform.position.y + topDownY;
            y = closetEnemyWeaponTargetPosition.y + topDownY;
            z = worldPosition.z;


            targetPosition = new Vector3(
                x,
                y,
                z
            );

            if (gamePad == true)
            {
                targetPosition = new Vector3
                (
                    x = worldPosition.x,
                    y = worldPosition.y,
                    z = worldPosition.z
                    );
            }




            mousePosition = cam.WorldToScreenPoint(targetPosition);
        }



        if (mousePosition.x < xMin) mousePosition.x = xMin;
        if (mousePosition.x > xMax) mousePosition.x = xMax;
        if (mousePosition.y < yMin) mousePosition.y = yMin;
        if (mousePosition.y > yMax) mousePosition.y = yMax;


        crossHair.transform.position = targetPosition;
        crosshairImage.transform.position = mousePosition;


    }


    public void Move(float degrees, float distance)
    {
        // local coordinate rotation around the Y axis to the given angle
        Quaternion rotation = Quaternion.AngleAxis(degrees, Vector3.up);
        // add the desired distance to the direction
        Vector3 addDistanceToDirection = rotation * transform.forward * distance;
        // add the distance and direction to the current position to get the final destination
        targetPosition = transform.position + addDistanceToDirection;
        Debug.DrawRay(transform.position, addDistanceToDirection, Color.red, 10.0f);
        //transform.LookAt(this.destination);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;

        dstManager.AddComponentData(entity,
            new ActorWeaponAimComponent { weaponCamera = weaponCamera });
    }
}


