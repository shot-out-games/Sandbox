using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Rewired;
using Unity.Mathematics;

public class CameraControls : MonoBehaviour
{

    public Player player;
    public int playerId = 0; // The Rewired player id of this character

    public CinemachineVirtualCamera vcam;
    public CinemachineFreeLook freeLook;
    public CinemachineFreeLook freeLookCombat;
    //public CinemachineFollowZoom FollowZoom;
    public float fov;
    [SerializeField]
    float maxFov = 200;
    [SerializeField]
    float minFov = 2;

    [SerializeField]
    float multiplier = 2;
    [SerializeField]
    float combatZoomFactor = .8f;
    [SerializeField]
    bool active;


    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);

        ChangeFov(fov);

    }

    void Update()
    {
        if (active == false) return;

        Controller controller = player.controllers.GetLastActiveController();
        if (controller == null) return;


        bool gamePad = controller.type == ControllerType.Joystick;
        bool keyboard = controller.type == ControllerType.Keyboard;


        if (player.GetAxisRaw("Dpad Vertical") >= 1)
        {
            fov -= Time.deltaTime * multiplier;
            ChangeFov(fov);
        }
        else if (player.GetAxisRaw("Dpad Vertical") <= -1)
        {
            fov += Time.deltaTime * multiplier;
            ChangeFov(fov);
        }
        else if (player.GetAxisRaw("Dpad Vertical") >= 1)
        {
            fov -= Time.deltaTime * multiplier;
            ChangeFov(fov);
        }
        else if (player.GetAxisRaw("Dpad Vertical") <= -1)
        {
            fov += Time.deltaTime * multiplier;
            ChangeFov(fov);
        }



    }


    public void ChangeFov(float _fov)
    {
        //fov = fov > maxFov ?  maxFov : fov;
        //fov = fov < minFov ?  minFov : fov;
        if (freeLook)
        {
            fov = math.clamp(fov, minFov, maxFov);
            //Debug.Log("fov " + fov);
            freeLook.m_Lens.FieldOfView = _fov;
            if (freeLookCombat)
            {
                freeLookCombat.m_Lens.FieldOfView = _fov * combatZoomFactor;
            }
        }

        if (vcam)
        {
            fov = math.clamp(fov, minFov, maxFov);
            //Debug.Log("fov " + fov);
            vcam.m_Lens.FieldOfView = _fov;
        }



    }



}
