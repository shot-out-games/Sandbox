using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;



public struct FreezeComponent : IComponentData
{

    public float freezeTime;
    public bool isFrozen;
    public bool stopAnimation;
}



[UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]

public class FreezeSystem : SystemBase
{
    public float timeToAdd = 1.9f;

    protected override void OnUpdate()
    {


        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity e,  EnemyMove move, 
            ref FreezeComponent freezeComponent, 
            ref EnemyStateComponent enemyStateComponent,
            in RatingsComponent ratingsComponent
            ) =>
        {
            bool hasDamage = EntityManager.HasComponent(e, typeof(DamageComponent));
            if (hasDamage)
            {

                //if (move.stunEffect)
                //{
                //    if (move.stunEffect.isPlaying == false)
                //    {
                //        move.stunEffect.transform.SetParent(move.transform);
                //        move.stunEffect.Play(true);
                //    }
                //}



                freezeComponent.freezeTime += timeToAdd;
                freezeComponent.isFrozen = true;
            }
            else
            {
                freezeComponent.freezeTime -= Time.DeltaTime;
                if(freezeComponent.freezeTime <= 0)
                {
                    freezeComponent.freezeTime = 0;
                    freezeComponent.isFrozen = false;
                    //move.agent.speed = ratingsComponent.speed;
                    // move.moveSpeed = ratingsComponent.speed;
                    move.agent.speed = move.moveSpeed;

                    if (move.stunEffect)
                    {
                        move.stunEffect.Stop();
                    }

                }
            }

            if(freezeComponent.isFrozen == true)
            {
                //    move.moveSpeed = 0;
                move.agent.speed = 0;
                enemyStateComponent.MoveState = MoveStates.Idle;
                if (move.stunEffect && move.ps)
                {
                    if (move.ps.isPlaying == false)
                    {
                        move.stunEffect.transform.SetParent(move.transform);
                        move.stunEffect.Play(true);
                    }
                }



            }

            //Debug.Log("speed " + move.moveSpeed);
            //Debug.Log("freeze time " + freezeComponent.freezeTime);


        }).Run();
    }
}
