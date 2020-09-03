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

            dstManager.AddComponentData
                (
                    entity,
                    new PlayerJumpComponent
                    {
                        startJumpGravityForce = startJumpGravityForce,
                        jumpFramesToPeak = jumpFramesToPeak,
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


    //[UpdateBefore()(typeof(MoveCollisionSystem))]
    //[UpdateBefore(typeof(PlayerMoveSystem))]
    //[DisableAutoCreation]

    public class PlayerJumpSystem2D : SystemBase
    {
        int frames = 0;
        private int airFrames = 0;

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
                    float leftStickX = inputController.leftStickX;


                    bool button_x = inputController.buttonX_Pressed;
                    bool button_x_held = inputController.buttonX_held;


                    pv.Linear = applyImpulseComponent.Velocity;
                    float airForceAdd = 0;
                    float jumpFrames = playerJumpComponent.jumpFramesToPeak;
                    float jumpPower = playerJumpComponent.startJumpGravityForce / jumpFrames + playerJumpComponent.addedNegativeForce;//added to offset down player movement neg force . the two should be equal ideally


                    //Debug.Log("grounded " + applyImpulseComponent.Grounded);
                    //Debug.Log("in jump " + applyImpulseComponent.InJump);
                    //Debug.Log("ceiling " + applyImpulseComponent.Ceiling);
                    //Debug.Log("falling " + applyImpulseComponent.Falling);

                    //Debug.Log("added neg " + playerJumpComponent.addedNegativeForce);


                    if (applyImpulseComponent.InJump == false)
                    {
                        frames = 0;
                        airFrames = 0;
                        playerJump.JumpStage = JumpStages.Ground;

                    }



                    if ((applyImpulseComponent.BumpLeft == true || applyImpulseComponent.BumpRight == true) &&
                        applyImpulseComponent.Grounded == false)
                    {
                        //applyImpulseComponent.BumpLeft = false;
                        //applyImpulseComponent.BumpRight = false;
                        applyImpulseComponent.Ceiling = false;
                        applyImpulseComponent.Falling = true;
                        applyImpulseComponent.Grounded = false;
                        applyImpulseComponent.InJump = false;

                    }
                    else if (applyImpulseComponent.Ceiling == true)
                    {
                        //float3 vel = new float3(pv.Linear.x, -jumpPower*5 , 0);
                        //pv.Linear = vel;
                        applyImpulseComponent.Ceiling = false;
                        //applyImpulseComponent.Falling = true;
                        //applyImpulseComponent.Grounded = false;
                        //applyImpulseComponent.InJump = false;

                    }
                    else if (button_x == true && applyImpulseComponent.InJump == false)
                    {
                        applyImpulseComponent.InJump = true;
                        applyImpulseComponent.Grounded = false;
                        applyImpulseComponent.Falling = false;
                        frames = 1;
                        //Debug.Log(" start fr " + frames);
                        inputController.gameObject.GetComponent<Animator>().SetTrigger("JumpStage");
                        inputController.gameObject.GetComponent<Animator>().applyRootMotion = false;
                        playerJump.JumpStage = JumpStages.JumpStart;
                        float3 vel = new float3(pv.Linear.x, jumpPower, 0);
                        pv.Linear = vel;
                        //airForceAdd = leftStickX * playerJumpComponent.airForce;
                        //pv.Linear.x += airForceAdd / 2;
                    }
                    else if (frames >= 1 && frames <= jumpFrames && applyImpulseComponent.InJump == true && applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                    //else if (frames >= 1 && frames <= 1 && applyImpulseComponent.InJump == true)
                    {
                        frames = frames + 1;
                        //Debug.Log(" fr boost " + frames);
                        float3 vel = new float3(pv.Linear.x, jumpPower, 0);
                        pv.Linear = vel;
                    }
                    else if (button_x == false && applyImpulseComponent.InJump == true && button_x_held == true
                             &&
                             frames >= (jumpFrames + 1) && frames <= (jumpFrames + 1)
                             && airFrames == 0
                             )
                    {
                        frames++;
                        //Debug.Log(" fr held " + frames);
                        airFrames++;
                        float3 vel = new float3(pv.Linear.x, jumpPower * 2, 0);//60 pct to regular jump
                        pv.Linear = vel;
                        //airForceAdd = leftStickX * playerJumpComponent.airForce;
                        //pv.Linear.x += airForceAdd;
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



                    //Debug.Log("v " + pv.Linear);



                }
                )
                ).Run();

        }


    }


}
