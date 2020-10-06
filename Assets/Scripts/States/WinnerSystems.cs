﻿using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;




public class WinnerSystem : SystemBase
{
    float timer = 0;
    bool showWin = false;

    protected override void OnUpdate()
    {


        //nde  and most games needs two - 1 for dead enemy final boss triggers player endgamereached
        int goal = 1;//ld 1  nde 2 
        int endGameCompleteCounter = 0;
        bool exit = false;
        bool resetLevel = false;


        Entities.WithoutBurst().ForEach
        (
            (ref WinnerComponent winnerComponent, ref HealthComponent health, ref DeadComponent dead, in RatingsComponent RatingsComponent, in Entity e
            ) =>
            {
                //ld
                if (winnerComponent.keys == 1 && RatingsComponent.tag == 1)//player
                {
                    exit = true;
                    winnerComponent.keys = 1919;
                    Debug.Log("key ld ");
                    //winnerComponent.endGameReached = true;
                    //endGameCompleteCounter += 1;
                }
                //kenney
                //if (winnerComponent.keys >= 8 && RatingsComponent.tag == 1)//player
                //{
                //    exit = true;
                //    Debug.Log("keys ");
                //    winnerComponent.endGameReached = true;
                //    endGameCompleteCounter += 1;
                //}
                //if (RatingsComponent.tag == 2 && winnerComponent.endGameReached == true)//enemy
                if (RatingsComponent.tag == 2 && winnerComponent.resetLevel)//enemy
                {
                    health.TotalDamageReceived = 0;
                    health.TotalDamageLanded = 0;
                    dead.justDead = false;
                    dead.isDead = false;
                    //endGameCompleteCounter += 1;
                    winnerComponent.resetLevel = false;
                    resetLevel = true;
                    winnerComponent.keys = 0;
                    winnerComponent.endGameReached = false;
                }

            }
        ).Run();


        ////exit = true;
        if (exit == true)
        {
            LevelManager.instance.audioSourceGame.Stop();
        }


        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (in TriggerComponent triggerComponent, in AudioSource audioSource
            ) =>
            {
                if (exit == true && triggerComponent.Type == (int)TriggerType.Home)
                {

                    //Debug.Log("boss song");
                    audioSource.Play();
                }
                else if (exit == true)
                {
                    audioSource.Stop();
                }

            }
        ).Run();



        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (in TriggerComponent triggerComponent, in Entity e
            ) =>
            {
                if (exit == true && triggerComponent.Type == (int)TriggerType.Home)
                {

                    //EntityManager.DestroyEntity(e);//destroyed for collect keys not for  final boss key entry

                }

            }
        ).Run();

        bool loopBroken = false;
        toggleStates winner = toggleStates.off;

        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (ref WinnerComponent winnerComponent, in Entity e, in PlayerComponent playerComponent, in SkillTreeComponent skillTreeComponent
            ) =>
            {
                if(winnerComponent.winConditionMet == toggleStates.post)
                {
                    timer = timer + Time.DeltaTime;
                    winner = toggleStates.post;
                }
                else if (winnerComponent.winConditionMet == toggleStates.on)
                {
                    winner = toggleStates.on;
                    winnerComponent.winConditionMet = toggleStates.post;
                }
                else if (skillTreeComponent.CurrentLevel >= 8 && winnerComponent.winnerCounter == 0)
                {
                    winnerComponent.winnerCounter = -1;
                    loopBroken = true;
                }

            }
        ).Run();



        //exit = true;
        if (loopBroken == true)
        {

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (in StartGameMenuComponent messageMenuComponent, in StartGameMenuGroup messageMenu) =>
                {
                    if (messageMenu.showOnce == false)
                    {
                        messageMenu.showOnce = true;
                        Debug.Log("loop ");
                        messageMenu.messageString = "Level 8 ... The Loop must be broken";
                        messageMenu.ShowMenu();
                    }
                }
            ).Run();

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (WeaponManager weaponManager) =>
                {
                    weaponManager.DetachPrimaryWeapon();

                }
            ).Run();

        }
        else if (exit == true)
        {

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (in StartGameMenuComponent messageMenuComponent, in StartGameMenuGroup messageMenu) =>
                {
                    if (messageMenu.showOnce == false)
                    {
                        messageMenu.showOnce = true;
                        messageMenu.showTimeLength = 3.0f;
                        messageMenu.messageString = "I am you ... I am you from a time you can not understand ... ... ...";
                        messageMenu.showTimeLength = 2.1f;
                        messageMenu.ShowMenu();
                    }
                }
            ).Run();

        }
        else if (winner == toggleStates.on)
        {

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (in StartGameMenuComponent messageMenuComponent, in StartGameMenuGroup messageMenu) =>
                {
                    if (messageMenu.showOnce == false)
                    {
                        messageMenu.showOnce = true;
                        //messageMenu.messageString = "Exit is Open ... Boss can be destroyed";
                        messageMenu.messageString = "Seppuku .. It was the only way ... Die a hero or  live long enough to be the villain ... ";
                        messageMenu.showTimeLength = 5.0f;
                        messageMenu.ShowMenu();
                    }
                }
            ).Run();
        }
        else if (winner == toggleStates.post && timer > 5.0f && showWin == false)
        {
            showWin = true;
            timer = 0;
            int currentLevel = LevelManager.instance.currentLevel;
            LevelManager.instance.audioSourceGame.Stop();
            LevelManager.instance.audioSourceMenu.Stop();


            Entities.WithoutBurst().ForEach
            (
                (ref WinnerMenuComponent winnerMenuComponent, in WinnerMenuGroup winnerMenuGroup) =>
                {
                    LevelManager.instance.levelSettings[currentLevel].completed = true;
                    if (winnerMenuComponent.hide == true)
                    {
                    LevelManager.instance.endGame = true;
                        winnerMenuGroup.ShowMenu();
                        winnerMenuComponent.hide = false;
                    }
                }
            ).Run();

        }


        //if (resetLevel == false)
        //{

        //    Entities.WithoutBurst().ForEach
        //    (
        //        (AudioSource audioSource, TriggerComponent trigger) =>
        //        {
        //            audioSource.Stop();
        //        }
        //    ).Run();

        //    Entities.WithoutBurst().ForEach
        //    (
        //        (ref WinnerMenuComponent winnerMenuComponent, in WinnerMenuGroup winnerMenuGroup) =>
        //        {
        //            LevelManager.instance.levelSettings[currentLevel].completed = true;
        //            //Debug.Log("eg ");
        //            if (winnerMenuComponent.hide == true)
        //            {
        //                // Debug.Log("win show ");
        //                LevelManager.instance.endGame = true;
        //                winnerMenuGroup.ShowMenu();
        //                winnerMenuComponent.hide = false;
        //            }
        //        }
        //    ).Run();

        //}


        if (resetLevel == true)
        {
            Debug.Log("reset level 0");

            Entities.WithoutBurst().ForEach
            (
                (AudioSource audioSource, TriggerComponent trigger) => { audioSource.Stop(); }
            ).Run();

            Entities.WithoutBurst().WithStructuralChanges().ForEach
            (
                (Lighting lighting) =>
                {
                    Debug.Log("reset level 1");
                    lighting.directionalLight.color = lighting.directionalLight.color * .5f;
                    lighting.intensity = lighting.intensity * .5f;
                    lighting.UpdateLights();

                }
            ).Run();




            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (in StartGameMenuComponent messageMenuComponent, in StartGameMenuGroup messageMenu) =>
                {
                    if (messageMenu.showOnce == false)
                    {
                        messageMenu.showOnce = true;
                        messageMenu.messageString = "That is not the way ....";
                        messageMenu.ShowMenu();
                    }
                }
            ).Run();




            Entities.WithoutBurst().ForEach
            (
                (ref PlayerComponent playerComponent, ref Translation translation, ref Rotation rotation) =>
                {
                    //Debug.Log("reset ");
                    translation.Value = playerComponent.startPosition;
                }
            ).Run();



        }

    }

}





