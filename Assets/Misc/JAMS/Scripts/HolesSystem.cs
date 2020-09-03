using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;




public class HoleSystem : JobComponentSystem
{


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithoutBurst().ForEach(
            (ref HoleComponent holeComponent, in Entity entity) =>
            {
                int counter = holeComponent.holeHitCounter + 1;
                if (holeComponent.hitTag == 1)//fireball - string not supported
                {
                    holeComponent.hitTag = 0;
                    holeComponent.open = !holeComponent.open;
                    holeComponent.holeHitCounter = counter;
                    ecb.SetComponent(entity, holeComponent);
                }
            }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();

        return default;
    }



}
