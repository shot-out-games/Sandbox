using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateBefore(typeof(InputControllerSystemLateUpdate))]

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
public class InputControllerSystemLateUpdate : SystemBase
{
    protected override void OnUpdate()
    {


        Entities.WithoutBurst().ForEach((InputController inputController, ref Translation translation, in Rotation rotation) =>
        {

            inputController.LateUpdateSystem();


        }).Run();
    }
}
