using Rewired;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Rendering;

public class LevelItemSystem : JobComponentSystem
{


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        int index = -1;
       
        Entities.WithoutBurst().WithStructuralChanges().ForEach(
            (in Entity e, in ItemComponent levelItem, in TriggerComponent triggerComponent, in CollisionComponent collisionComponent) =>
            {
                int type_a = collisionComponent.Part_entity;
                int type_b = collisionComponent.Part_other_entity;
                Entity collision_entity_a = collisionComponent.Character_entity;
                Entity collision_entity_b = collisionComponent.Character_other_entity;
                bool rewind_a = EntityManager.HasComponent(collision_entity_a, typeof(AmmoComponent));
                bool rewind_b = EntityManager.HasComponent(collision_entity_b, typeof(AmmoComponent));
                //Debug.Log("ra " + rewind_a + " rb " + rewind_b);
                bool rewinding_a = false;
                bool rewinding_b = false;

                if (rewind_a == true)
                {
                    rewinding_a = EntityManager.GetComponentData<AmmoComponent>(collision_entity_a).rewinding;
                }
                if (rewind_b == true)
                {
                    rewinding_b = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b).rewinding;
                }


                if ((type_a == (int)TriggerType.Lever || type_b == (int)TriggerType.Lever) && levelItem.activator && (rewinding_a || rewinding_b)) //b is ammo so causes damage to entity
                {
                    ecb.DestroyEntity(e);
                    index = 1;//win?
                }





            }
        ).Run();


        Entities.WithoutBurst().ForEach
(
    (ref WinnerComponent winnerComponent, in Entity entity) =>
    {
        if(index == 1)
        {
            winnerComponent.endGameReached = true;
        }

    }
).Run();




        //Entities.WithoutBurst().WithStructuralChanges().ForEach(
        //    (in Entity e, in ItemComponent levelItem, in TriggerComponent triggerComponent) =>
        //    {
        //        int triggered_index = levelItem.index;
        //        if (triggered_index == index)
        //        {
        //            if (e != Entity.Null)
        //                ecb.DestroyEntity(e);
        //        }
        //    }
        //).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();


        return default;
    }

}

