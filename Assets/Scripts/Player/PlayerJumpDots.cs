using System;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using ForceMode = UnityEngine.ForceMode;



namespace SandBox.Player
{
    [RequireComponent(typeof(InputController))]
    [RequireComponent(typeof(PlayerMove))]

    public class PlayerJumpDots : MonoBehaviour, IConvertGameObjectToEntity
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


        [HideInInspector]
        public bool handleJump;
        [HideInInspector]
        public float currentGravity;
        //[HideInInspector]
        public float currentGroundCheckDistanceToGround;
        public bool grounded = true;

        Animator anim;
        public AudioClip audioClip;
        public ParticleSystem ps;

        //public Vector3 stickInput;
        public Transform groundCheck;
        //[HideInInspector]
        public float3 targetForce;
        [HideInInspector]
        public string lastSurface;



        void Awake()
        {
            anim = GetComponent<Animator>();
            mainCam = Camera.main;
        }

        void Start()
        {
            currentGravity = startJumpGravityForce;
        }


        public float CalculateJumpVerticalSpeed()
        {
            float jumpPower = 0;
            jumpPower = Mathf.Sqrt(2f * jumpY * currentGravity);
            return jumpPower;

        }


        public float DistanceToGround()
        {

            UnityEngine.RaycastHit hit;
            float checkGroundDistance = .25f;
            float y_above = .15f;
            Vector3 down = new Vector3(0, y_above, 0);//start at y above ground fire ray check ground distance down through ground
            if (groundCheck != null) down = groundCheck.position;

            Debug.DrawRay(down, Vector3.down * 10.0f, Color.blue);
            var landingRay = new UnityEngine.Ray(down, Vector3.down);
            LayerMask mask = ~(1 << LayerMask.NameToLayer("Player"));//so it cant touch itself
            var touchGround = Physics.Raycast(landingRay, out hit, checkGroundDistance, mask);
            currentGroundCheckDistanceToGround = hit.distance;


            if (hit.collider != null)
            {

                if (
                    hit.collider.tag == "Ground" ||
                    hit.collider.tag == "Terrain")
                {
                    if (JumpStage == JumpStages.JumpDown)
                    {
                        JumpStage = JumpStages.Ground;
                        Debug.Log("grounded");
                        grounded = true;
                        lastSurface = hit.collider.tag;

                    }
                    if (ps)
                    {
                        ps.Stop();
                    }
                }
            }

            return currentGroundCheckDistanceToGround;

        }

        void LateUpdate()
        {

            if (Terrain.activeTerrain != null && JumpStage == JumpStages.Ground && lastSurface == "Terrain")
            {
                grounded = true;
                Vector3 campos = transform.position;
                float responsiveness = 6f;
                float groundLevel = Terrain.activeTerrain.SampleHeight(campos);
                campos.y = Mathf.Lerp(campos.y, groundLevel, Time.deltaTime * responsiveness); // 10 responsiveness results in a good not too lazy movement
                transform.position = campos;
            }

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
                        addedNegativeForce = addedNegativeForce,
                        jumpDownGravityMultiplier = jumpDownGravityMultiplier,
                        jumpY = jumpY,
                        airForce = airForce
                    }
                );


        }
    }


    [UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]



    public class PlayerJumpSystemDots : SystemBase
    {


        protected override void OnUpdate()
        {

            Entities.WithoutBurst().ForEach(
            (
                (
                    Entity e,
                    PlayerJumpDots playerJump,
                    InputController inputController,
                    Animator animator,
                    ref LocalToWorld localToWorld,
                    ref PhysicsVelocity pv,
                    //in Translation translation,
                    //in Rotation rotation,
                    in PhysicsMass pm,
                    in PlayerJumpComponent playerJumpComponent
                    ) =>
                {

                    if (EntityManager.GetComponentData<Pause>(e).value == 1) return;

                    PhysicsWorld physicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>().PhysicsWorld;
                    int rigidbodyIndex = PhysicsWorldExtensions.GetRigidBodyIndex(physicsWorld, e);

                    float3 currentVelocity = physicsWorld.GetLinearVelocity(rigidbodyIndex);
                    float yVelocity = currentVelocity.y;
                    //Debug.Log("yo " + math.round(yVelocity * 100));

                    float airForceAddZ = 0f;
                    float airForceAddX = 0f;


                    float leftStickX = inputController.leftStickX;
                    float leftStickY = inputController.leftStickY;
                    Vector3 stickInput = new Vector3(leftStickX, 0, leftStickY);
                    Vector3 forward =
                        playerJump.mainCam.transform.TransformDirection(Vector3.forward); //forward of camera to forward of world
                    forward.y = 0f;
                    forward = forward.normalized;
                    Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
                    Vector3 targetDirection = (stickInput.x * right + stickInput.z * forward);



                    bool button_x = inputController.buttonX_Pressed;

                    if (targetDirection.sqrMagnitude > 1f)
                    {
                        targetDirection = targetDirection.normalized;
                    }
                    if (playerJump.JumpStage == JumpStages.Ground)
                    {
                        if (playerJump.handleJump)
                        {
                            playerJump.currentGravity = playerJumpComponent.startJumpGravityForce;
                            playerJump.handleJump = false;
                            playerJump.JumpStage = JumpStages.JumpStart;
                            playerJump.grounded = false;
                            float3 vel =  new float3(0, playerJump.CalculateJumpVerticalSpeed(), 0);
                            //rb.AddForce(vel, ForceMode.VelocityChange);
                            //PhysicsWorldExtensions.ApplyLinearImpulse(physicsWorld, rigidbodyIndex, vel);
                            pv.Linear  += vel;
                            var unitMass = new PhysicsMass { InverseInertia = new float3(1.0f), InverseMass = 1.0f, Transform = pm.Transform };
                            PhysicsWorldExtensions.SetLinearVelocity(physicsWorld, rigidbodyIndex, pv.Linear);
                            //pv.ApplyImpulse(unitMass, localToWorld.Position, localToWorld.Rotation, vel, animator.transform.up);
                            Debug.Log("vel " + vel);
                            return;
                        }
                    }
                    else if (playerJump.JumpStage == JumpStages.JumpStart)
                    {
                        playerJump.JumpStage = JumpStages.JumpUp;
                    }
                    else if (playerJump.JumpStage == JumpStages.JumpUp && yVelocity < -.0001f)// just called one frame on way down
                    //else if (playerJump.JumpStage == JumpStages.JumpUp)// just called one frame on way down
                    {
                        float verticalJumpForce = -playerJump.currentGravity;
                        playerJump.JumpStage = JumpStages.JumpDown;
                        verticalJumpForce *= playerJumpComponent.jumpDownGravityMultiplier;
                        airForceAddZ = targetDirection.z * playerJumpComponent.airForce;
                        airForceAddX = targetDirection.x * playerJumpComponent.airForce;
                        //rb.AddForce(new Vector3(0, verticalJumpForce, 0), ForceMode.VelocityChange);
                        Debug.Log("001");
                        //PhysicsWorldExtensions.ApplyLinearImpulse(physicsWorld, rigidbodyIndex, new float3(0, verticalJumpForce, 0));
                        pv.Linear += new float3(0, verticalJumpForce, 0);

                    }
                    else if (playerJump.JumpStage == JumpStages.JumpUp)
                    {
                        airForceAddZ = targetDirection.z * playerJumpComponent.airForce;
                        airForceAddX = targetDirection.x * playerJumpComponent.airForce;
                    }
                    else if (playerJump.JumpStage == JumpStages.JumpDown)
                    {
                        playerJump.grounded = false;
                        airForceAddZ = targetDirection.z * playerJumpComponent.airForce;
                        airForceAddX = targetDirection.x * playerJumpComponent.airForce;
                    }


                    float yDist = playerJump.DistanceToGround();
                    //Debug.Log("dist " + yDist);


                    float airForceAddY = playerJump.grounded ? 0 : playerJumpComponent.addedNegativeForce;


                    animator.SetBool("Grounded", playerJump.grounded);
                    playerJump.targetForce  = new Vector3(airForceAddX, airForceAddY, airForceAddZ);//added neg force - positive # less than gravity for hang time - neg number for more force downward
                                                                                                   //rb.AddForce(playerJump.targetForce, ForceMode.Acceleration);
                    //PhysicsWorldExtensions.ApplyLinearImpulse(physicsWorld, rigidbodyIndex, playerJump.targetForce);
                    //Debug.Log("002");
                    pv.Linear += playerJump.targetForce;

                    if (playerJump.JumpStage == JumpStages.Ground)
                    {

                        if (button_x)
                        {
                            AudioSource audioSource = playerJump.GetComponent<AudioSource>();
                            playerJump.handleJump = true;


                            if (playerJump.audioClip && audioSource)
                            {
                                audioSource.PlayOneShot(playerJump.audioClip);
                                Debug.Log("clip " + audioSource.clip);

                            }
                            if (playerJump.ps)
                            {
                                playerJump.ps.transform.SetParent(animator.transform);
                                //playerJump.ps.transform.position = Vector3.zero;
                                playerJump.ps.Play(true);
                            }
                        }
                    }
                }
                )
                ).Run();


            //return default;

        }


    }


}
