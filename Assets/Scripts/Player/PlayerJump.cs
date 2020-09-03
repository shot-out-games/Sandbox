using System;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;


public struct PlayerJumpComponent : IComponentData
{
    public float startJumpGravityForce;
    public float jumpFramesToPeak;
    public float addedNegativeForce;
    public float jumpDownGravityMultiplier;
    public float jumpY;
    public float airForce;
}


public enum JumpStages
{
    Ground,
    JumpStart,
    JumpUp,
    JumpDown,
    JumpEnd
}



namespace SandBox.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InputController))]
    [RequireComponent(typeof(PlayerMove))]

    public class PlayerJump : MonoBehaviour, IConvertGameObjectToEntity
    {
        private Camera mainCam;

        float startJumpGravityForce = 9.81f;
        float addedNegativeForce = 0f;
        float jumpDownGravityMultiplier = 1.0f;
        float jumpY = 6f;
        float airForce = 500f;

        public JumpStages JumpStage = JumpStages.Ground;

        public bool handleJump;
        private Rigidbody rb;
        float currentGravity;
        public float currentGroundCheckDistanceToGround;
        public bool grounded = true;

        Animator anim;
        public AudioClip audioClip;
        public ParticleSystem ps;
        public AudioSource audioSource;

        public Vector3 stickInput;
        [SerializeField] private Transform groundCheck;
        [SerializeField]
        private Vector3 targetForce;

        private string lastSurface;
        


        void Awake()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            mainCam = Camera.main;
        }

        void Start()
        {
            currentGravity = startJumpGravityForce;
        }

        void FixedUpdate()
        {
            float yVelocity = rb.velocity.y;
            float airForceAddZ = 0f;
            float airForceAddX = 0f;

            Vector3 forward =
                mainCam.transform.TransformDirection(Vector3.forward); //forward of camera to forward of world
            forward.y = 0f;
            forward = forward.normalized;
            Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
            Vector3 targetDirection = (stickInput.x * right + stickInput.z * forward);
            if (targetDirection.sqrMagnitude > 1f)
            {
                targetDirection = targetDirection.normalized;
            }
            if (JumpStage == JumpStages.Ground)
            {
                if (handleJump)
                {
                    currentGravity = startJumpGravityForce;
                    handleJump = false;
                    JumpStage = JumpStages.JumpStart;
                    grounded = false;
                    Vector3 vel = new Vector3(0, CalculateJumpVerticalSpeed(), 0);
                    rb.AddForce(vel, ForceMode.VelocityChange);
                    return;
                }
            }
            else if (JumpStage == JumpStages.JumpStart)
            {
                JumpStage = JumpStages.JumpUp;
            }
            else if (JumpStage == JumpStages.JumpUp && yVelocity < -.001f)// just called one frame on way down
            {
                float verticalJumpForce = -currentGravity;
                JumpStage = JumpStages.JumpDown;
                verticalJumpForce *= jumpDownGravityMultiplier;
                airForceAddZ = targetDirection.z * airForce;
                airForceAddX = targetDirection.x * airForce;
                rb.AddForce(new Vector3(0, verticalJumpForce, 0), ForceMode.VelocityChange);

            }
            else if (JumpStage == JumpStages.JumpUp)
            {
                airForceAddZ = targetDirection.z * airForce;
                airForceAddX = targetDirection.x * airForce;
            }
            else if (JumpStage == JumpStages.JumpDown)
            {
                grounded = false;
                airForceAddZ = targetDirection.z * airForce;
                airForceAddX = targetDirection.x * airForce;
            }


            DistanceToGround();

            float airForceAddY = grounded ? 0 : addedNegativeForce;


            anim.SetBool("Grounded", grounded);
            targetForce = new Vector3(airForceAddX, airForceAddY, airForceAddZ);//added neg force - positive # less than gravity for hang time - neg number for more force downward
            rb.AddForce(targetForce, ForceMode.Acceleration);

        }


        float CalculateJumpVerticalSpeed()
        {
            float jumpPower = 0;
            jumpPower = Mathf.Sqrt(2f * jumpY * currentGravity);
            return jumpPower;

        }


        public float DistanceToGround()
        {

            RaycastHit hit;
            float checkGroundDistance = 1f;
            float y_above = .15f;
            Vector3 down = new Vector3(0, y_above, 0);//start at y above ground fire ray check ground distance down through ground
            if (groundCheck != null) down = groundCheck.position;

            Debug.DrawRay(down, Vector3.down * 2.0f, Color.blue);
            var landingRay = new Ray(down, Vector3.down);
            LayerMask mask = ~(1 << LayerMask.NameToLayer("Player"));//so it cant touch itself
            var touchGround = Physics.Raycast(landingRay, out hit, checkGroundDistance, mask);
            currentGroundCheckDistanceToGround = hit.distance;

            if (hit.collider != null)
            {

                if (
                    hit.collider.tag == "Ground" ||
                    hit.collider.tag == "Terrain")
                    //|| currentGroundCheckDistanceToGround <= y_above)
                {
                    if (JumpStage == JumpStages.JumpDown)
                    {
                        JumpStage = JumpStages.Ground;
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



    public class PlayerJumpSystem : JobComponentSystem
    {


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
            (
                (
                    in Entity e,
                    in PlayerJump playerJump,
                    in InputController inputController,
                    in Animator animator,
                    in Rigidbody rb,
                    in Transform transform
                    ) =>
                {

                    if (EntityManager.GetComponentData<Pause>(e).value == 1) return;


                    float leftStickX = inputController.leftStickX;
                    float leftStickY = inputController.leftStickY;
                    Vector3 stickInput = new Vector3(leftStickX, 0, leftStickY);


                    //stickInput = forward;
                    playerJump.stickInput = stickInput;
                    //float stickSpeed = stickInput.sqrMagnitude;

                    bool button_x = inputController.buttonX_Pressed;

                    if (playerJump.JumpStage == JumpStages.Ground)
                    {
                        if (button_x)
                        {
                            AudioSource audioSource = playerJump.audioSource;
                            playerJump.handleJump = true;
                            if (playerJump.audioClip && audioSource)
                            {
                                audioSource.PlayOneShot(playerJump.audioClip);
                            }
                            if (playerJump.ps)
                            {
                                playerJump.ps.transform.SetParent(transform);
                                //playerJump.ps.transform.position = Vector3.zero;
                                playerJump.ps.Play(true);
                            }
                        }
                    }
                }
                )
                ).Run();


            return default;

        }



    }


}
