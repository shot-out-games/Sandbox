using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;



[UpdateInGroup(typeof(SimulationSystemGroup))]

public class InputControllerSystemUpdate : SystemBase
{
    protected override void OnUpdate()
    {


        Entities.WithoutBurst().ForEach((InputController inputController, ref Translation translation, in Rotation rotation) =>
        {

            inputController.UpdateSystem();


        }).Run();
    }
}



