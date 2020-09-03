using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


namespace SandBox.Player
{


    //[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PlayerWeaponAimSystemLateUpdate))]

    public class PlayerWeaponAimSystemUpdate : SystemBase
    {
        protected override void OnUpdate()
        {


            Entities.WithoutBurst().ForEach((PlayerWeaponAim mb) =>
            {
                mb.UpdateSystem();
            }).Run();
        }
    }


    [UpdateInGroup(typeof(PresentationSystemGroup))]

    public class PlayerWeaponAimSystemLateUpdate : SystemBase
    {
        protected override void OnUpdate()
        {


            Entities.WithoutBurst().ForEach((PlayerWeaponAim mb) =>
            {
                mb.LateUpdateSystem();
            }).Run();
        }
    }
}


