using System;
using System.Xml.Linq;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using SandBox.Player;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[System.Serializable]
public struct ApplyImpulseWithoutAnimationComponent : IComponentData
{
    public float Force;
    public float3 Direction;
    public float3 Velocity;
    public float3 NegativeForce;
    public Translation Translation;
    public Rotation Rotation;
}

namespace SandBox.Player
{
    [RequiresEntityConversion]


    public class PlayerMoveWithoutAnimation : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        float rotateSlerpDampTime = 9f;

        [HideInInspector]
        public Camera mainCam;

        public float startSpeed = 4f;
        public float currentSpeed = 4f;
        private EntityManager _entityManager;
        private Entity _entity;

        [SerializeField]
        float negativeForce = -9.81f;
        private bool jumpEnabled;

        public float slerpDampTime { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            jumpEnabled = GetComponent<PlayerJump>() || GetComponent<PlayerJumpDots>();
            mainCam = Camera.main;

        }



        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            _entity = entity;
            _entityManager = dstManager;

            startSpeed = GetComponent<PlayerRatings>() ? GetComponent<PlayerRatings>().Ratings.speed : 4f;
            dstManager.AddComponentData(entity, new PlayerMoveComponent() { currentSpeed = startSpeed, rotateSlerpDampTime = rotateSlerpDampTime });
            dstManager.AddComponentData(entity, new ApplyImpulseWithoutAnimationComponent() { Force = 0, Direction = Vector3.zero });


        }
    }
}





public class PlayerMoveSystemWithoutAnimation : JobComponentSystem
{


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {


        Entities.WithoutBurst().ForEach(
            (ref PlayerMoveComponent playerMove, ref ApplyImpulseWithoutAnimationComponent applyImpulse,
             in Entity entity, in InputController inputController, in Translation translation, in Rotation rotation) =>
            {

                float leftStickX = inputController.leftStickX;
                float leftStickY = inputController.leftStickY;
                Vector3 stickInput = new Vector3(leftStickX, 0, leftStickY);
                float stickSpeed = stickInput.sqrMagnitude;
                applyImpulse.Velocity = new float3(leftStickX * playerMove.currentSpeed, 0, leftStickY * playerMove.currentSpeed);

            }
        ).Run();
        return default;
    }

}



public class PlayerRotateWithoutAnimationSystem : JobComponentSystem
{





    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().ForEach
        (
            (
                PlayerMoveWithoutAnimation playerMove,
                ref Rotation rotation,
                in Pause pause, in DeadComponent deadComponent,
                in PlayerMoveComponent playerMoveComponent,
                in InputController inputController

            ) =>
            {
                float leftStickX = inputController.leftStickX;
                float leftStickY = inputController.leftStickY;
                bool rotating = inputController.rotating;


                if (rotating && leftStickX != leftStickY && pause.value == 0 && !deadComponent.isDead)
                {
                    Camera cam = playerMove.mainCam;
                    float slerpDampTime = playerMoveComponent.rotateSlerpDampTime;
                    Quaternion camRotation = cam.transform.rotation;
                    camRotation.x = 0;
                    camRotation.z = 0;
                    // we need some axis derived from camera but aligned with floor plane
                    Vector3 forward =
                        cam.transform.TransformDirection(Vector3.forward); 
                    //forward of camera to forward of world
                    //local forward vector of camera will become world vector position that is passed to the forward vector of the player (target) rigidbody 
                    //(The cam and player(target) vector will now always point in the same direction)

                    forward.y = 0f;

                    forward = forward.normalized;
                    Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
                    Vector3 targetDirection = (leftStickX * right + leftStickY * forward);
                    if (targetDirection.sqrMagnitude > 1f)
                    {
                        targetDirection = targetDirection.normalized;
                    }


                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                    rotation.Value = targetRotation;
                }
            }
        ).Run();

        return default;


    }
}


public class ApplyImpulseWithoutAnimationSystem : SystemBase
{

    protected override void OnUpdate()
    {

        Entities.WithoutBurst().ForEach(
            (
                Entity entity,
                ref PhysicsVelocity physicsVelocity,
                ref LocalToWorld localToWorld,
                ref Translation translation,
                in Rotation rotation,
                in PhysicsMass physicsMass,
                in ApplyImpulseWithoutAnimationComponent applyImpulseData
            ) =>
            {
                physicsVelocity.Linear = applyImpulseData.Velocity;
            }).Run();
    }
}

