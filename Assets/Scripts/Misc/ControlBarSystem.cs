using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ControlBarSystem : SystemBase
{
    protected override void OnUpdate()
    {

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            Entities.WithoutBurst().ForEach((ref DeadComponent deadComponent,
                    ref ControlBarComponent controlComponent, 
                    ref RatingsComponent ratingsComponent, in Entity entity) =>
                {

                   

                   var dead = EntityManager.GetComponentData<DeadComponent>(entity).isDead;
                    if(dead == false)
                    {
                        if (controlComponent.value > 20)
                        {
                            controlComponent.value -= 1 * Time.DeltaTime;
                            //Debug.Log("tm " + controlComponent.value);
                        }
                    }


                    //float val = controlComponent.maxHealth;
                    //val = 25f;

                    //if (controlComponent.value >= val && dead.isDead == false && dead.justDead == false)
                    //{
                    //    dead.isDead = true;
                    //    dead.justDead = true;
                    //    ecb.SetComponent(entity, dead);//tag player or enemy
                    //}

                }
            ).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();


        //.Schedule();
    }
}
