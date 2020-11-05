using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct DeadComponent : IComponentData
{
    public bool isDead;
    public bool isDying;
    public bool playDeadEffects;
    //public bool justDead;
    public int dieLevel;
    public int tag;
    public bool checkLossCondition;
}


public class DeadSystem : SystemBase //really game over system currently
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        int currentLevel = LevelManager.instance.currentLevelCompleted;
        //bool levelComplete = LevelManager.instance.levelSettings[currentLevel].completed;

        Entities.WithoutBurst().ForEach
        (
            (ref DeadComponent deadComponent, in Entity entity, in Animator animator) =>
            {
                if (deadComponent.isDying
                    && deadComponent.tag == 1)//player
                {
                    deadComponent.isDying = false;
                    deadComponent.playDeadEffects = true;
                    animator.SetInteger("Dead", 1);
                    LevelManager.instance.levelSettings[currentLevel].playersDead += 1;
                }
            }
        ).Run();






        bool enemyJustDead = false;

        Entities.WithoutBurst().ForEach
        (
            (ref DeadComponent deadComponent, ref WinnerComponent winnerComponent, ref PhysicsVelocity pv, ref Translation translation,
            in Entity entity) =>
            {


                if (HasComponent<FlingMechanicComponent>(entity) && deadComponent.isDead == true)
                {
                    translation.Value.y = 4f;
                }


                if (deadComponent.isDying
                    && deadComponent.tag == 2)//enemy
                {
                    Debug.Log("set dead");
                    deadComponent.isDying = false;
                    deadComponent.playDeadEffects = true;
                    //ecb.DestroyEntity(entity);//for now 
                    if (winnerComponent.checkWinCondition == true)//this  (and all with this true) enemy must be defeated to win the game
                    {
                        winnerComponent.endGameReached = true;
                    }
                    //else
                    //{
                    //deadComponent.justDead = false;
                    enemyJustDead = true;
                    LevelManager.instance.levelSettings[currentLevel].enemiesDead += 1;
                    //}
                }
            }
        ).Run();


        if (enemyJustDead == true)
        {
            Entities.WithoutBurst().ForEach
            (
                (Animator animator) =>
                {
                    //animator.SetInteger("Zone", 4);
                    animator.SetInteger("Dead", 1);
                }
            ).Run();
        }


        if (enemyJustDead == true)
        {

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (in ShowMessageMenuComponent messageMenuComponent, in ShowMessageMenuGroup messageMenu) =>
                {
                    messageMenu.messageString = "... Destroyed ... ";
                    messageMenu.ShowMenu();
                }
            ).Run();


            //add bonuses for defeating enemies here

        }


        ecb.Playback(EntityManager);
        ecb.Dispose();

        //return default;
    }

}



