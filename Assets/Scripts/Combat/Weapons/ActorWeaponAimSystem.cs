using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;



//[UpdateBefore(typeof(GunAmmoHandlerSystem))]
[UpdateInGroup(typeof(TransformSystemGroup))]
//[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]

public class EnemyWeaponAimSystemLateUpdate : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((in EnemyWeaponAim mb, in ActorWeaponAimComponent playerWeaponAimComponent) =>
        {
            mb.LateUpdateSystem();
        }).Run();
    }
}


//[UpdateBefore(typeof(GunAmmoHandlerSystem))]
[UpdateInGroup(typeof(TransformSystemGroup))]
//[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]

public class PlayerWeaponAimSystemLateUpdate : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((in PlayerWeaponAim mb, in ActorWeaponAimComponent playerWeaponAimComponent) =>
        {
            mb.LateUpdateSystem(playerWeaponAimComponent.weaponRaised);
        }).Run();
    }
}



