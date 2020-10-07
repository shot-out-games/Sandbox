using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;


public class StatsSystem : SystemBase
{
    protected override void OnUpdate()
    {

        bool showMessage = false;
        Entities.WithoutBurst().ForEach
        (
            (
                Entity e,
                ref StatsComponent statsComponent,
                ref SkillTreeComponent skillTree,
                ref RatingsComponent ratingsComponent,
                ref HealthComponent healthComponent
                 
            ) =>
            {
                //Debug.Log("skill tree available");
                if (skillTree.CurrentLevelXp >= skillTree.PointsNextLevel)
                {
                    skillTree.CurrentLevelXp = 0;
                    skillTree.CurrentLevel += 1;
                    skillTree.availablePoints += 1;

                    if (HasComponent<LevelUpMechanicComponent>(e) == true)
                    {
                        ratingsComponent.gameSpeed = ratingsComponent.gameSpeed + 1;
                        ratingsComponent.gameWeaponPower = ratingsComponent.gameWeaponPower * 2f;
                        ratingsComponent.maxHealth = ratingsComponent.maxHealth * 2f;
                        showMessage = true;
                        Debug.Log("speed boost");
                    }

                }
            }
        ).Run();

        Entities.WithoutBurst().WithStructuralChanges().ForEach(
            (in StartGameMenuComponent messageMenuComponent, in StartGameMenuGroup messageMenu) =>
            {
                if (showMessage == true)
                {

                    messageMenu.messageString = "Level Up ...  Max Health Up ... HP Up ... Durability Up";
                    messageMenu.ShowMenu();
                }
            }
        ).Run();










    }






}


