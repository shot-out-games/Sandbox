using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


namespace SandBox.Player
{


    //[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    //[UpdateBefore(typeof(PlayerWeaponAimSystemLateUpdate))]

//    [UpdateInGroup(typeof(PresentationSystemGroup))]
    //[UpdateInGroup(typeof(TransformSystemGroup))]

    ////[UpdateBefore(typeof(PlayerInputAmmoSystem))]
    ////[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]



    //public class PlayerWeaponAimSystemUpdate : SystemBase
    //{
    //    protected override void OnUpdate()
    //    {
    //        Entities.WithoutBurst().WithStructuralChanges().ForEach((PlayerWeaponAim mb) =>
    //        {
    //            mb.UpdateSystem();
    //        }).Run();
    //    }
    //}


    [UpdateInGroup(typeof(PresentationSystemGroup))]


    public class PlayerWeaponAimSystemLateUpdate : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithoutBurst().ForEach((in PlayerWeaponAim mb) =>
            {
                mb.LateUpdateSystem();
            }).Run();
        }
    }
}


