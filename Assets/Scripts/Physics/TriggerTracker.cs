using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

//public class TriggerTracker : MonoBehaviour { }

[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(EndFrameLocalToParentSystem))]
//[UpdateAfter(typeof(FollowTriggerComponent))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


class SynchronizeGameObjectTransformsWithTriggerEntities : SystemBase
{
    //[NativeDisableParallelForRestriction] private EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        base.OnCreate();
        //m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_Query = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(TrackerComponent),
                //typeof(Tracker),
                typeof(Transform),
                typeof(LocalToWorld)
            }
        });



    }

    protected override void OnUpdate()
    {
        //var localToWorlds = m_Query.ToComponentDataArrayAsync<LocalToWorld>(Allocator.TempJob, out var jobHandle);
        var trackerComponents = m_Query.ToComponentDataArray<TrackerComponent>(Allocator.TempJob);
        //var localToWorlds = m_Query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

        //var entities = m_Query.ToEntityArray(Allocator.Temp);

        Entities.WithoutBurst().ForEach(
            (ref TrackerComponent trackComponent, in Tracker tracker) =>
            {
                trackComponent.Position = tracker.track.transform.position;
                trackComponent.Rotation = tracker.track.transform.rotation;
            }
            ).Run();


        Dependency = new SyncTransforms
        {
            //CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
            TrackerComponents = trackerComponents
        }.Schedule(m_Query.GetTransformAccessArray());




        Entities.ForEach((ref Translation translation, ref Rotation rotation, in TrackerComponent trackerComponent, in Entity e) =>
        {
            translation.Value = trackerComponent.Position;
            rotation.Value = trackerComponent.Rotation;
       

        }).Schedule();

        //Dependency.Complete();

        //m_Query.Dispose();
        //trackerComponents.Dispose();
        //localToWorlds.Dispose();
        //jobHandle.Complete();


    }

    [BurstCompile]
    struct SyncTransforms : IJobParallelForTransform
    {
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<TrackerComponent> TrackerComponents;

        //[NativeDisableParallelForRestriction] public EntityCommandBuffer CommandBuffer;
        public void Execute(int index, TransformAccess transform)
        {
            transform.position = TrackerComponents[index].Position;
            transform.rotation = TrackerComponents[index].Rotation;
        }

    }


    //[BurstCompile]
    //struct SyncTranslations : IJobParallelFor
    //{
    //    [DeallocateOnJobCompletion]
    //    [ReadOnly] public NativeArray<TriggerComponent> TriggerComponents;

    //    [NativeDisableParallelForRestriction] public EntityCommandBuffer CommandBuffer;
    //    public void Execute(int index)
    //    {
    //        //transform.position = TriggerComponents[index].Position;
    //        //transform.rotation = TriggerComponents[index].Rotation;
    //        TriggerComponent triggerComponent = TriggerComponents[index];
    //        Entity e = triggerComponent.Entity;

    //    }
    //}







}

