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

class SynchronizeGameObjectTransformsWithTriggerEntities : SystemBase
{
    [NativeDisableParallelForRestriction] private EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_Query = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(TriggerComponent),
                typeof(Tracker),
                typeof(Transform),
                typeof(LocalToWorld)
            }
        });



    }

    protected override void OnUpdate()
    {
        //var localToWorlds = m_Query.ToComponentDataArrayAsync<LocalToWorld>(Allocator.TempJob, out var jobHandle);
        var triggerComponents = m_Query.ToComponentDataArrayAsync<TriggerComponent>(Allocator.TempJob, out var jobHandle);
        var localToWorlds = m_Query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

        //var entities = m_Query.ToEntityArray(Allocator.Temp);

        Dependency = new SyncTransforms
        {
            //CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
            TriggerComponents = triggerComponents
        }.Schedule(m_Query.GetTransformAccessArray(), JobHandle.CombineDependencies(Dependency, jobHandle));







        Entities.ForEach((ref TriggerComponent triggerComponent, ref Translation translation, ref Rotation rotation) =>
        {
            translation.Value = triggerComponent.Position;
            rotation.Value = triggerComponent.Rotation;

        }).ScheduleParallel();


    }

    [BurstCompile]
    struct SyncTransforms : IJobParallelForTransform
    {
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<TriggerComponent> TriggerComponents;

        [NativeDisableParallelForRestriction] public EntityCommandBuffer CommandBuffer;
        public void Execute(int index, TransformAccess transform)
        {
            transform.position = TriggerComponents[index].Position;
            transform.rotation = TriggerComponents[index].Rotation;
            TriggerComponent triggerComponent = TriggerComponents[index];
            Entity e = triggerComponent.Entity;

        }
    }


    [BurstCompile]
    struct SyncTranslations : IJobParallelFor
    {
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<TriggerComponent> TriggerComponents;

        [NativeDisableParallelForRestriction] public EntityCommandBuffer CommandBuffer;
        public void Execute(int index)
        {
            //transform.position = TriggerComponents[index].Position;
            //transform.rotation = TriggerComponents[index].Rotation;
            TriggerComponent triggerComponent = TriggerComponents[index];
            Entity e = triggerComponent.Entity;

        }
    }







}

