using Rewired;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Rendering;

public class LevelItemSystem : SystemBase
{


    protected override void OnUpdate()
    {

        var index = new NativeArray<int>(1, Allocator.TempJob);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        index[0] = -1;

        Entities.ForEach(
            (in Entity e, in ItemComponent levelItem, in TriggerComponent triggerComponent, in CollisionComponent collisionComponent) =>
            {
                int type_a = collisionComponent.Part_entity;
                int type_b = collisionComponent.Part_other_entity;
                Entity collision_entity_a = collisionComponent.Character_entity;
                Entity collision_entity_b = collisionComponent.Character_other_entity;
                bool rewind_a = HasComponent<AmmoComponent>(collision_entity_a);
                bool rewind_b = HasComponent<AmmoComponent>(collision_entity_b);
                bool rewinding_a = false;
                bool rewinding_b = false;

                if (rewind_a == true)
                {
                    rewinding_a = GetComponent<AmmoComponent>(collision_entity_a).rewinding;
                }
                if (rewind_b == true)
                {
                    rewinding_b = GetComponent<AmmoComponent>(collision_entity_b).rewinding;
                }


                if ((type_a == (int)TriggerType.Lever || type_b == (int)TriggerType.Lever) && levelItem.activator && (rewinding_a || rewinding_b)) //b is ammo so causes damage to entity
                {
                    ecb.DestroyEntity(e);
                    index[0] = 1;//win?
                }





            }
        ).Schedule();


        Entities.WithoutBurst().ForEach
        (
        (ref WinnerComponent winnerComponent, in Entity entity) =>
        {
            if (index[0] == 1)
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
        index.Dispose();

    }

}


























