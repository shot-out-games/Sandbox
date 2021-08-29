using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Physics.Extensions;
using UnityEngine.Experimental.GlobalIllumination;


/// <summary> Our simple impulse component. </summary>
[System.Serializable]
public struct ApplyImpulseComponent : IComponentData
{
    public float stickX;
    public float stickY;
    public bool Falling;
    public bool Grounded;
    public bool Ceiling;
    public bool BumpLeft;
    public bool BumpRight;
    public bool InJump;
    public float Force;
    public float3 Direction;
    public float3 Velocity;
    public float NegativeForce;
    public float ApproachStairBoost;
    public Translation Translation;
    public Rotation Rotation;
    public float3 LastPositionLand;
    public bool hiJump;
    public float fallingFramesCounter;
    public float fallingFramesMaximuim;


}


public class ApplyImpulseSystem : SystemBase
{

    protected override void OnUpdate()
    {
        /// Get the physics world.

        Entities.WithoutBurst().ForEach(
            (
                Entity entity,
                ref ApplyImpulseComponent applyImpulseData,
                ref PhysicsVelocity physicsVelocity,
                ref LocalToWorld localToWorld,
                ref Translation translation,
                in Rotation rotation,
                in PhysicsMass physicsMass
            ) =>
            {
                // Apply a linear impulse to the entity.

                //applyImpulseData.Direction = new float3(1,0,0);
                //applyImpulseData.Force = .025f;

                //ComponentExtensions.ApplyLinearImpulse(ref physicsVelocity, physicsMass,
                //applyImpulseData.Direction * applyImpulseData.Force);

                
                
                //physicsVelocity.Linear = applyImpulseData.Velocity;



                //translation.Value.y = 0;


                //Debug.Log("pv " + physicsVelocity.Linear);


                //physicsVelocity.Linear = new float3(0,0, 3);
                //rotation = applyImpulseData.Rotation;

                //rotation.Value = math.mul(rotation.Value, quaternion.RotateY(45));
                //rotation.Value = math.mul( applyImpulseData.Rotation.Value, quaternion.RotateY(45));

                //rotation = localToWorld.Rotation;

                //localToWorld.Value = rotation;

                //ComponentExtensions.ApplyImpulse(ref physicsVelocity, physicsMass, translation, rotation,  2.5f , translation.Value);
                //ComponentExtensions.

            }).Run();
    }
}




