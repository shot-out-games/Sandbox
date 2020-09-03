using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;



//[UpdateInGroup(typeof(InitializationSystemGroup))]

////[UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
//public class PhysicsDisableSystem : SystemBase
//{

//    StepPhysicsWorld stepPhysicsWorld;
//    BuildPhysicsWorld buildPhysicsWorld;
//    private ExportPhysicsWorld exportPhysicsWorld;
//    protected override void OnCreate()
//    {
//        //stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
//        //buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
//        //exportPhysicsWorld = World.GetOrCreateSystem<ExportPhysicsWorld>();
//    }

//    protected override void OnUpdate()
//    {
//        //buildPhysicsWorld.Enabled = false;
//        //stepPhysicsWorld.Enabled = false;
//        //exportPhysicsWorld.Enabled = false;

//    }


//}


//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

//public class PhysicsEnableSystem : SystemBase
//{

//    StepPhysicsWorld stepPhysicsWorld;
//    BuildPhysicsWorld buildPhysicsWorld;
//    private ExportPhysicsWorld exportPhysicsWorld;


//    protected override void OnCreate()
//    {

//        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
//        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
//        exportPhysicsWorld = World.GetOrCreateSystem<ExportPhysicsWorld>();

//        buildPhysicsWorld.Enabled = false;
//        stepPhysicsWorld.Enabled = false;
//        exportPhysicsWorld.Enabled = false;

//    }

//    protected override void OnUpdate()
//    {


//        var stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
//        var buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
//        var exportPhysicsWorld = World.GetOrCreateSystem<ExportPhysicsWorld>();

//        buildPhysicsWorld.Enabled = true;
//        stepPhysicsWorld.Enabled = true;
//        exportPhysicsWorld.Enabled = true;


//        buildPhysicsWorld.Update();
//        stepPhysicsWorld.Update();
//        exportPhysicsWorld.Update();

//    }


//}


public class PlayerSystem : ComponentSystem
{
    private bool once = false;

    protected override void OnUpdate()
    {
        if (false == once)
        {
            FixedRateUtils
                .EnableFixedRateWithCatchUp(World.GetOrCreateSystem<SimulationSystemGroup>(),
                    UnityEngine.Time.fixedDeltaTime);
            FixedRateUtils
             .EnableFixedRateWithCatchUp(World.GetOrCreateSystem<FixedStepSimulationSystemGroup>(),
              UnityEngine.Time.fixedDeltaTime);
            once = true;
        }
    }
}
