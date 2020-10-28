using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;


public class EnemyBehaviorSystem : SystemBase
{
    private Random random;

    protected override void OnCreate()
    {
        random = new Random();
        random.InitState(10);

    }


    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


        Entities.WithoutBurst().WithStructuralChanges().WithNone<Pause>().ForEach
        (
        (
            ref EnemyMovementComponent enemyBasicMovementComponent,
            ref EnemyWeaponMovementComponent enemyWeaponMovementComponent,
            ref EnemyMeleeMovementComponent enemyMeleeMovementComponent,
            in Entity entity

        ) =>
        {

            //if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
            if (EntityManager.GetComponentData<DeadComponent>(entity).isDead) return;

            if (enemyMeleeMovementComponent.switchUp == false || enemyWeaponMovementComponent.switchUp == false) return;

            enemyMeleeMovementComponent.switchUpTimer += Time.DeltaTime;
            enemyWeaponMovementComponent.switchUpTimer += Time.DeltaTime;



            if (enemyMeleeMovementComponent.enabled && enemyMeleeMovementComponent.switchUpTimer <= enemyMeleeMovementComponent.currentSwitchUpTime) return;
            if (enemyWeaponMovementComponent.enabled && enemyWeaponMovementComponent.switchUpTimer <= enemyWeaponMovementComponent.currentSwitchUpTime) return;


            //Debug.Log("timer1  " + enemyWeaponMovementComponent.switchUpTimer);
            enemyMeleeMovementComponent.currentSwitchUpTime = random.NextFloat(enemyMeleeMovementComponent.originalSwitchUpTime * .5f, enemyMeleeMovementComponent.originalSwitchUpTime * 1.5f);
            enemyWeaponMovementComponent.currentSwitchUpTime = random.NextFloat(enemyWeaponMovementComponent.originalSwitchUpTime * .5f, enemyWeaponMovementComponent.originalSwitchUpTime * 1.5f);
            

            enemyMeleeMovementComponent.switchUpTimer = 0;
            enemyWeaponMovementComponent.switchUpTimer = 0;

            //ignore or turn off basic movement for this
            enemyBasicMovementComponent.enabled = false;
            enemyMeleeMovementComponent.enabled = !enemyMeleeMovementComponent.enabled;
            enemyWeaponMovementComponent.enabled = !enemyWeaponMovementComponent.enabled;


        }
        ).Run();



        //return default;


    }





}
