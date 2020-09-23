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
    public float fov;
    [SerializeField]
    float maxFov = 200;
    [SerializeField]
    float minFov = 2;

    [SerializeField]
    float multiplier = 2;
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


        if (player.GetAxisRaw("RightVertical") >= 1 && gamePad)
        {
            fov -= Time.deltaTime * multiplier;
            ChangeFov(fov);
        }
        else if (player.GetAxisRaw("RightVertical") <= -1 && gamePad)
        {
            fov += Time.deltaTime * multiplier;
            ChangeFov(fov);
        }
        else if (player.GetAxisRaw("Move Vertical") >= 1 && gamePad == false)
        {
            fov -= Time.deltaTime * multiplier;
            ChangeFov(fov);
        }
        else if (player.GetAxisRaw("Move Vertical") <= -1 && gamePad == false)
        {
            fov += Time.deltaTime * multiplier;
            ChangeFov(fov);
        }

    }


    public void ChangeFov(float _fov)
    {
        //fov = fov > maxFov ?  maxFov : fov;
        //fov = fov < minFov ?  minFov : fov;
        fov = math.clamp(fov, minFov, maxFov);
        vcam.m_Lens.FieldOfView = _fov;
    }



}
