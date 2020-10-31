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

                   
                    //example
                    if(deadComponent.isDead == false)
                    {
                        if (controlComponent.value > 20)
                        {
                            controlComponent.value -= 1 * Time.DeltaTime;
                        }
                    }

                }
            ).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();

    }
}
