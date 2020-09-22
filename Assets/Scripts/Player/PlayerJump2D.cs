using System;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using ForceMode = UnityEngine.ForceMode;



namespace SandBox.Player
{

    public class PlayerJump2D : MonoBehaviour, IConvertGameObjectToEntity
    {

        public JumpStages JumpStage = JumpStages.Ground;
        [HideInInspector]
        public Camera mainCam;
        [HideInInspector]
        public float startJumpGravityForce = 9.81f;
        [HideInInspector]
        public float addedNegativeForce = 0f;
        [HideInInspector]
        public float jumpDownGravityMultiplier = 1.0f;
        [HideInInspector]
        public float jumpY = 6f;
        [HideInInspector]
        public float airForce = 500f;
        public float jumpFramesToPeak = 5;

        

        Animator anim;
        public AudioClip audioClip;
        public ParticleSystem ps;

        public AudioSource audioSource;



        void Awake()
        {
            anim = GetComponent<Animator>();
            mainCam = Camera.main;
        }




        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (GetComponent<PlayerRatings>())
            {
                startJumpGravityForce = GetComponent<PlayerRatings>().Ratings.startJumpGravityForce;
                addedNegativeForce = GetComponent<PlayerRatings>().Ratings.addedNegativeForce;
                jumpDownGravityMultiplier = GetComponent<PlayerRatings>().Ratings.jumpDownGravityMultiplier;
                jumpY = GetComponent<PlayerRatings>().Ratings.jumpY;
                airForce = GetComponent<PlayerRatings>().Ratings.airForce;
            }


            //float framesToPeakRatio = startJumpGravityForce / jumpFramesToPeak;
            float framesToPeakRatio = jumpFramesToPeak;
            dstManager.AddComponentData
                (
                    entity,
                    new PlayerJumpComponent
                    {
                        startJumpGravityForce = startJumpGravityForce,
                        gameStartJumpGravityForce = startJumpGravityForce,
                        jumpFramesToPeak = framesToPeakRatio,
                        addedNegativeForce = addedNegativeForce,
                        jumpDownGravityMultiplier = jumpDownGravityMultiplier,
                        jumpY = jumpY,
                        airForce = airForce
                    }
                );


        }
    }


    //[UpdateBefore(typeof(InputControllerSystemUpdate))]

    //[UpdateAfter(typeof(BuildPhysicsWorld))]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

    //[UpdateBefore()(typeof(MoveCollisionSystem))]
    //[UpdateBefore(typeof(PlayerMoveSystem))]
    //[DisableAutoCreation]

    public class PlayerJumpSystem2D : SystemBase
    {
        int frames = 0;
        private int airFrames = 0;
        float buttonHeldFrames = 0;
        //private int fallingFramesCounter = 0;


        protected override void OnUpdate()
        {

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
            (
                (
                    Entity e,
                    PlayerJump2D playerJump,
                    InputController inputController,
                    ref Translation translation,
                    ref PhysicsVelocity pv,
                    ref ApplyImpulseComponent applyImpulseComponent,
                    in PlayerJumpComponent playerJumpComponent
                    ) =>
                {

                    if (EntityManager.GetComponentData<Pause>(e).value == 1) return;

                    bool variableJump = true;
                    //int fallingFramesAllowed = 3;
                    //bool cancelVariableJump = false;

                    float leftStickX = inputController.leftStickX;

                    bool button_x = inputController.buttonX_Pressed;
                    bool button_x_held = inputController.buttonX_held;
                    //if (button_x) cancelVariableJump = true;

                    //startJumpGravityForce over jumpFramesToPeak so ie 50 and 10 50


                    //Debug.Log("x " + button_x);
                    //Debug.Log("x held " + button_x_held);
                    pv.Linear = applyImpulseComponent.Velocity;
                    float airForceAdd = 0;

                    float gameToDefaultJumpForce = playerJumpComponent.gameStartJumpGravityForce / playerJumpComponent.startJumpGravityForce;

                    float jumpFrames = gameToDefaultJumpForce * playerJumpComponent.jumpFramesToPeak;//increases if now jumping higher
                    float originalJumpFrames = playerJumpComponent.jumpFramesToPeak;
                    //float jumpPower = playerJumpComponent.gameStartJumpGravityForce + playerJumpComponent.addedNegativeForce;//added to offset down player movement neg force . the two should be equal ideally
                    //float originalJumpPower = playerJumpComponent.startJumpGravityForce + playerJumpComponent.addedNegativeForce;//added to offset down player movement neg force . the two should be equal ideally
                    float jumpPower = playerJumpComponent.gameStartJumpGravityForce;
                    float originalJumpPower = playerJumpComponent.startJumpGravityForce;

                    float standardJumpHeight = originalJumpPower * originalJumpFrames;//total height of jump at peak - ref only
                    float hiJumpMultiplier = 2f;//multiply jump power by this - frames 
                    float hiJumpAirFramesMax = jumpFrames * hiJumpMultiplier;

                    //float expDownAdj = playerJumpComponent.gameStartJumpGravityForce /
                    //                playerJumpComponent.startJumpGravityForce;
                    //Debug.Log("jump frames " + jumpFrames);
                    //Debug.Log("jump power " + jumpPower);



                    //Debug.Log("added neg " + playerJumpComponent.addedNegativeForce);


                    if (applyImpulseComponent.InJump == false)
                    {
                        frames = 0;
                        airFrames = 0;
                        playerJump.JumpStage = JumpStages.Ground;
                        //cancelVariableJump = false;
                        applyImpulseComponent.hiJump = false;


                    }

                    if (applyImpulseComponent.Falling)
                    {
                        return;
                    }
             

                 
                    if ((button_x == true || button_x_held == true) && applyImpulseComponent.InJump == false && frames == 0)
                    {
                        applyImpulseComponent.InJump = true;
                        applyImpulseComponent.Grounded = false;
                        applyImpulseComponent.Falling = false;
                        applyImpulseComponent.hiJump = false;

                        frames = 1;
                        //Debug.Log(" start fr " + frames);
                        inputController.gameObject.GetComponent<Animator>().SetTrigger("JumpStage");
                        inputController.gameObject.GetComponent<Animator>().applyRootMotion = false;
                        playerJump.JumpStage = JumpStages.JumpStart;
                        float3 vel = new float3(pv.Linear.x, originalJumpPower, 0);
                        pv.Linear = vel;
                    }
                    else if (frames >= 1 && frames <= originalJumpFrames && applyImpulseComponent.InJump == true && applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                    {
                        frames = frames + 1;
                        //Debug.Log(" jump up fr " + frames);
                        if (frames == originalJumpFrames - 2 && button_x_held == true && variableJump == false)//make sure number here less than jump up frames at some point
                        {
                            // Debug.Log("start high jump");
                            buttonHeldFrames = 1;
                        }
                        else if (frames == originalJumpFrames - 1 && button_x_held == true && variableJump == false)//make sure number here less than jump up frames at some point
                        {
                            // Debug.Log("start high jump");
                            buttonHeldFrames = 2;
                        }
                        else if (frames == originalJumpFrames && button_x_held == true && (buttonHeldFrames == 2 || variableJump == true)) //make sure number here less than jump up frames at some point
                        {
                            //Debug.Log("bhf " + buttonHeldFrames);
                            applyImpulseComponent.hiJump = true;
                            buttonHeldFrames = 0;
                        }
                        else
                        {
                            buttonHeldFrames = 0;
                        }


                        float3 vel = new float3(pv.Linear.x, originalJumpPower, 0);
                        pv.Linear = vel;
                    }
                    else if (applyImpulseComponent.hiJump == true &&
                             (frames > 1 && airFrames < hiJumpAirFramesMax && variableJump == false)
                             ||
                             (frames > 1 && airFrames < hiJumpAirFramesMax && variableJump == true && button_x_held == true && button_x == false)


                        )//6 on higher jump after 6th frame held - very static could problem like if game has quick jump with low jump frames it will go up after jumpframes peak
                    {

                        frames++;
                        airFrames++;
                        //Debug.Log(" air frames " + airFrames);
                        float3 vel = new float3(pv.Linear.x, pv.Linear.y, 0);
                        pv.Linear.y = jumpPower * hiJumpMultiplier;

                    }
                    else if (applyImpulseComponent.hiJump == true &&
                             (airFrames >= hiJumpAirFramesMax 
                                || variableJump == true && (button_x_held == false || button_x == false)))
                    //6 on higher jump after 6th frame held - very static could problem like if game has quick jump with low jump frames it will go up after jumpframes peak
                    {
                        applyImpulseComponent.hiJump = false;
                    }
                    else if (playerJump.JumpStage == JumpStages.JumpStart)
                    {

                        frames++;
                        //Debug.Log(" fr start-up " + frames);
                        playerJump.JumpStage = JumpStages.JumpUp;
                        airForceAdd = leftStickX * playerJumpComponent.airForce;
                        float3 vel = new float3(pv.Linear.x, pv.Linear.y, 0);
                        pv.Linear = vel;
                        pv.Linear.x += airForceAdd;
                    }
                    else if (playerJump.JumpStage == JumpStages.JumpUp)
                    {
                        frames++;
                        //Debug.Log(" fr up " + frames);

                        airForceAdd = leftStickX * playerJumpComponent.airForce;
                        float3 vel = new float3(pv.Linear.x, pv.Linear.y, 0);
                        pv.Linear = vel;
                        //pv.Linear.y = vel.y * expDownAdj;
                        //Debug.Log("vy " + vel.y);
                        pv.Linear.x += airForceAdd;
                    }






                    if (button_x == true && frames == 1)
                    {
                        AudioSource audioSource = playerJump.audioSource;
                        if (playerJump.audioClip && audioSource)
                        {
                            audioSource.PlayOneShot(playerJump.audioClip);
                        }
                        if (playerJump.ps)
                        {
                            playerJump.ps.transform.SetParent(playerJump.transform);
                            playerJump.ps.Play(true);
                        }
                    }

                }
                )
                ).Run();

        }


    }


}
