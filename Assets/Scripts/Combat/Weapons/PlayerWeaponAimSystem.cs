using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


namespace SandBox.Player
{

    [UpdateBefore(typeof(GunAmmoHandlerSystem))]
    [UpdateInGroup(typeof(TransformSystemGroup))]




    public class PlayerWeaponAimSystemLateUpdate : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithoutBurst().ForEach((in PlayerWeaponAim mb, in ActorWeaponAimComponent playerWeaponAimComponent) =>
            {
                //if (playerWeaponAimComponent.weaponRaised == WeaponMotion.None) return;
                mb.LateUpdateSystem(playerWeaponAimComponent.weaponRaised);
            }).Run();
        }
    }
}


