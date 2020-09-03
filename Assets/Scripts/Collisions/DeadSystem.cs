using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct DeadComponent : IComponentData
{
    public bool isDead;
    public bool justDead;
    public int tag;
    public bool checkLossCondition;
}


public class DeadSystem : JobComponentSystem //really game over system currently
{



    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {



        int currentLevel = LevelManager.instance.currentLevel;
        bool levelComplete = LevelManager.instance.levelSettings[currentLevel].completed;
        if (levelComplete) return default;//cant kill player or enemy because beat level



        int lives = LevelManager.instance.levelSettings[currentLevel].lives;
        //Debug.Log("l " + lives);
        //int dead = 0;
        bool dead = false;
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);






        Entity menu_e = Entity.Null;
        WinnerMenuComponent winnerMenu = new WinnerMenuComponent();

        Entities.WithoutBurst().ForEach
        (
            (ref WinnerMenuComponent winnerMenuComponent, in WinnerMenuGroup winnerMenuGroup, in Entity e) =>
            {
                menu_e = e;
                winnerMenu = winnerMenuComponent;
            }
        ).Run();


        Entities.WithoutBurst().ForEach
        (
            (ref DeadComponent deadComponent, in Entity entity, in Animator animator) =>
            {
                if (deadComponent.isDead
                    && deadComponent.tag == 1 && deadComponent.justDead == true)//player
                {
                    deadComponent.justDead = false;
                    animator.SetInteger("Dead", 1);
                    LevelManager.instance.levelSettings[currentLevel].playersDead += 1;
                    dead = true;
                }
            }
        ).Run();


        if (dead == true)
        {
            Entities.WithoutBurst().ForEach
            (
                (ref DeadMenuComponent deadMenuComponent, in DeadMenuGroup deadMenuGroup) =>
                {
                    if (deadMenuComponent.hide == true)
                    {
                        deadMenuGroup.ShowMenu();
                        deadMenuComponent.hide = false;
                    }
                }
            ).Run();


        }




        Entities.WithoutBurst().ForEach
        (
            (ref DeadComponent deadComponent, in Entity entity, in Animator animator, in NavMeshAgent agent) =>
            {
                if (deadComponent.isDead
                    && deadComponent.tag == 3 && deadComponent.justDead == true)//npc 
                {
                    deadComponent.justDead = false;
                    agent.enabled = false;
                    animator.SetInteger("Zone", 4);
                    animator.SetInteger("Dead", 1);
                    winnerMenu.npcDeadCounter += 1;
                    EntityManager.SetComponentData(menu_e, winnerMenu);
                    LevelManager.instance.levelSettings[currentLevel].NpcDead += 1;
                }
            }
        ).Run();


        bool enemyJustDead = false;

        Entities.WithoutBurst().ForEach
        (
            (ref DeadComponent deadComponent, ref WinnerComponent winnerComponent, in NavMeshAgent navMeshAgent, in Entity entity, in Animator animator) =>
            {
                if (deadComponent.isDead
                    && deadComponent.tag == 2 && deadComponent.justDead == true)//enemy
                {
                    //ecb.DestroyEntity(entity);//for now 
                    deadComponent.justDead = false;
                    Debug.Log("sh0");
                    enemyJustDead = true;
                    navMeshAgent.speed = 0;
                    animator.SetInteger("Zone", 4);
                    animator.SetInteger("Dead", 1);
                    LevelManager.instance.levelSettings[currentLevel].enemiesDead += 1;

                    if (winnerComponent.checkWinCondition == true)
                    {
                        winnerComponent.endGameReached = true;
                    }



                }
            }
        ).Run();



        if (enemyJustDead == true)
        {

       

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (in StartGameMenuComponent messageMenuComponent, in StartGameMenuGroup messageMenu) =>
                {
                    messageMenu.messageString = "Opposition destroyed ... Reverse Boost";
                    messageMenu.ShowMenu();
                }
            ).Run();

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (ref ControlBarComponent health, in PlayerComponent player) =>
                {
                    health.value = health.value * .9f;
                }
            ).Run();

        }


        ecb.Playback(EntityManager);
        ecb.Dispose();

        return default;
    }

}



