using UnityEngine;
using Rewired;
using Unity.Entities;


public struct InputControllerComponent : IComponentData
{
    public Entity playerEntity;
    public int playerId; // The Rewired player id of this character
    public float leftStickX;
    public float leftStickY;
    public bool leftStickXreleased;
    public bool leftStickYreleased;
    public float dpadX;
    public float dpadY;
    public bool dpadXreleased;
    public bool dpadYreleased;
    public float deadZone;
    public bool rotating;
    public bool mouse;

    public float leftTriggerLast;
    public float rightTriggerLast;
    public bool leftTriggerPressed;
    public bool rightTriggerPressed;
    public float leftTriggerChange;
    public float rightTriggerChange;

    public bool leftTriggerDown;
    public bool rightTriggerDown;



    public bool leftBumperPressed;
    public bool leftBumperReleased;
    public float leftBumperValue;

    public bool rightBumperPressed;
    public bool rightBumperReleased;
    public float rightBumperValue;

    public float leftTriggerValue;
    public float rightTriggerValue;


    public bool buttonA_Pressed;
    public bool buttonA_held;
    public bool buttonA_Released;

    public bool buttonB_Pressed;
    public bool buttonB_held;
    public bool buttonB_Released;

    public bool buttonX_Pressed;
    public bool buttonX_held;
    public bool buttonX_Released;

    public bool buttonY_Pressed;
    public bool buttonY_held;
    public bool buttonY_Released;

    public bool buttonSelect_Pressed;
    public bool buttonSelect_held;
    public bool buttonSelect_Released;


}


public class InputController : MonoBehaviour, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity playerEntity;
    [HideInInspector]
    public bool mouse;
    public bool rotating = false;


    public Player player;
    public int playerId = 0; // The Rewired player id of this character
    float deadZone = Mathf.Epsilon;


    [Header("Left Stick")]
    public float leftStickX = 0;
    public bool leftStickXreleased;
    public float leftStickY = 0;
    public bool leftStickYreleased;


    [Header("D-PAD")]
    public float dpadX = 0;
    public bool dpadXreleased;
    public float dpadY = 0;
    public bool dpadYreleased;


    [Header("Triggers")]
    public float leftTriggerLast = 0f;
    public float rightTriggerLast = 0f;
    public bool leftTriggerPressed;
    public bool rightTriggerPressed;
    public float leftTriggerChange;
    public float rightTriggerChange;
    public float leftTriggerValue;
    public float rightTriggerValue;
    public bool leftTriggerDown;
    public bool rightTriggerDown;
    public bool leftTriggerUp;
    public bool rightTriggerUp;


    [Header("Bumpers")]
    public bool leftBumperPressed;
    public bool leftBumperReleased;
    public float leftBumperValue;
    public bool rightBumperPressed;
    public bool rightBumperReleased;
    public float rightBumperValue;




    [Header("Face Buttons")]
    public bool buttonA_Pressed;
    public bool buttonA_held;
    public bool buttonA_Released;

    public bool buttonB_Pressed;
    public bool buttonB_held;
    public bool buttonB_Released;

    public bool buttonX_Pressed;
    public bool buttonX_held;
    public bool buttonX_Released;

    public bool buttonY_Pressed;
    public bool buttonY_held;
    public bool buttonY_Released;

    public bool buttonSelect_Pressed;
    public bool buttonSelect_held;
    public bool buttonSelect_Released;


    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);

    }

    public void UpdateSystem()
    {

        if (!ReInput.isReady) return;

        if (manager == default || playerEntity == Entity.Null) return;


        bool hasComponent = manager.HasComponent<InputControllerComponent>(playerEntity);
        if (hasComponent == false) return;

        Controller controller = player.controllers.GetLastActiveController();
        if (controller != null)
        {
            mouse = false;
            if (controller.type == ControllerType.Mouse)
            {
                mouse = true;
            }
        }


        //if (Application.platform == RuntimePlatform.Android)
        //#if UNITY_ANDROID
        //{
        //     Debug.Log("Do something special here!");
        //}
        //#endif


        leftStickX = player.GetAxis("Move Horizontal");
        leftStickY = player.GetAxis("Move Vertical");

        leftStickXreleased = Mathf.Abs(player.GetAxisPrev("Move Horizontal")) < deadZone;
        leftStickYreleased = Mathf.Abs(player.GetAxisPrev("Move Vertical")) < deadZone;



        dpadX = player.GetAxis("Dpad Horizontal");
        dpadY = player.GetAxis("Dpad Vertical");

        dpadXreleased = player.GetAxisPrev("Dpad Horizontal") <= .000001f;
        dpadYreleased = player.GetAxisPrev("Dpad Vertical") <= .000001f;


        buttonA_Pressed = player.GetButtonDown("FireA");
        buttonA_held = player.GetButton("FireA");
        buttonA_Released = player.GetButtonUp("FireA");

        buttonB_Pressed = player.GetButtonDown("FireB");
        buttonB_held = player.GetButton("FireB");
        buttonB_Released = player.GetButtonUp("FireB");


        buttonX_Pressed = player.GetButtonDown("FireX");
        buttonX_held = player.GetButton("FireX");
        buttonX_Released = player.GetButtonUp("FireX");

        buttonY_Pressed = player.GetButtonDown("FireY");
        buttonY_held = player.GetButton("FireY");
        buttonY_Released = player.GetButtonUp("FireY");

        buttonSelect_Pressed = player.GetButtonDown("Select");
        buttonSelect_held = player.GetButton("Select");
        buttonSelect_Released = player.GetButtonUp("Select");





        leftBumperValue = player.GetAxis("LeftBumper");
        leftBumperPressed = leftBumperValue > 0.15f;


        rightBumperValue = player.GetAxis("RightBumper");
        rightBumperPressed = player.GetButtonDown("RightBumper");

        leftTriggerValue = player.GetAxis("LeftTrigger");
        rightTriggerValue = player.GetAxis("RightTrigger");

        leftTriggerPressed = player.GetButtonDown("LeftTrigger");
        rightTriggerPressed = player.GetButtonDown("RightTrigger");
        
        leftTriggerLast = player.GetAxisPrev("LeftTrigger");
        rightTriggerLast = player.GetAxisPrev("RightTrigger");

        leftTriggerChange = player.GetAxisDelta("LeftTrigger");
        rightTriggerChange = player.GetAxisDelta("RightTrigger");

        leftTriggerDown = player.GetButton("LeftTrigger");
        rightTriggerDown = player.GetButton("RightTrigger");

        leftTriggerUp = player.GetButtonUp("LeftTrigger");
        rightTriggerUp = player.GetButtonUp("RightTrigger");

        leftStickX = Mathf.Abs(leftStickX) < deadZone ? 0 : leftStickX;
        leftStickY = Mathf.Abs(leftStickY) < deadZone ? 0 : leftStickY;
        UpdateInputControllerComponent();


    }

    
    void UpdateInputControllerComponent()
    {
        var inputControllerComponent =
            manager.GetComponentData<InputControllerComponent>(playerEntity);

        inputControllerComponent.playerEntity = playerEntity;
        inputControllerComponent.playerId = playerId;
        inputControllerComponent.mouse = mouse;

        inputControllerComponent.deadZone = deadZone;
        inputControllerComponent.rotating = rotating;

        inputControllerComponent.leftStickX = leftStickX;
        inputControllerComponent.leftStickY = leftStickY;
        inputControllerComponent.leftStickXreleased = leftStickXreleased;
        inputControllerComponent.leftStickYreleased = leftStickYreleased;


        inputControllerComponent.dpadX = dpadX;
        inputControllerComponent.dpadY = dpadY;
        inputControllerComponent.dpadXreleased = dpadXreleased;
        inputControllerComponent.dpadYreleased = dpadYreleased;



        inputControllerComponent.leftTriggerLast = leftTriggerLast;
        inputControllerComponent.rightTriggerLast = rightTriggerLast;



        inputControllerComponent.leftTriggerPressed = leftTriggerPressed;
        inputControllerComponent.rightTriggerPressed = rightTriggerPressed;

        inputControllerComponent.leftTriggerDown = leftTriggerDown;
        inputControllerComponent.rightTriggerDown = rightTriggerDown;

        inputControllerComponent.leftTriggerChange = leftTriggerChange;
        inputControllerComponent.rightTriggerChange = rightTriggerChange;

        inputControllerComponent.leftBumperPressed = leftBumperPressed;
        inputControllerComponent.rightBumperPressed = rightBumperPressed;

        inputControllerComponent.leftBumperReleased = leftBumperReleased;
        inputControllerComponent.rightBumperReleased = rightBumperReleased;


        inputControllerComponent.leftBumperValue = leftBumperValue;
        inputControllerComponent.rightBumperValue = rightBumperValue;

        inputControllerComponent.leftTriggerValue = leftTriggerValue;
        inputControllerComponent.rightTriggerValue = rightTriggerValue;

        inputControllerComponent.buttonA_Pressed = buttonA_Pressed;
        inputControllerComponent.buttonA_held = buttonA_held;
        inputControllerComponent.buttonA_Released = buttonA_Released;

        inputControllerComponent.buttonB_Pressed = buttonB_Pressed;
        inputControllerComponent.buttonB_held = buttonB_held;
        inputControllerComponent.buttonB_Released = buttonB_Released;

        inputControllerComponent.buttonX_Pressed = buttonX_Pressed;
        inputControllerComponent.buttonX_held = buttonX_held;
        inputControllerComponent.buttonX_Released = buttonX_Released;

        inputControllerComponent.buttonY_Pressed = buttonY_Pressed;
        inputControllerComponent.buttonY_held = buttonY_held;
        inputControllerComponent.buttonY_Released = buttonY_Released;

        inputControllerComponent.buttonSelect_Pressed = buttonSelect_Pressed;
        inputControllerComponent.buttonSelect_held = buttonSelect_held;
        inputControllerComponent.buttonSelect_Released = buttonSelect_Released;

        manager.SetComponentData<InputControllerComponent>(playerEntity, inputControllerComponent);
    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        manager = dstManager;
        playerEntity = entity;
        manager.AddComponentData<InputControllerComponent>(playerEntity, new InputControllerComponent());
    }
}

