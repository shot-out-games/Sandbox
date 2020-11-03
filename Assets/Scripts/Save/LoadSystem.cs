﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[System.Serializable]
public struct LoadComponent : IComponentData
{
    public Entity e;
    public bool part1;
    public bool part2;
}
[UpdateInGroup((typeof(PresentationSystemGroup)))]
public class LoadSystem : SystemBase
{

    protected override void OnUpdate()
    {
        //return;

        bool loaded = false;
        Entity loadEntity = Entity.Null;

        Entities.ForEach
        (
            (
                ref LoadComponent load, in Entity e
            ) =>
            {
                loadEntity = e;
                loaded = load.part1;
                load.part1 = false;
            }
        ).Run();


        if (loaded == false) return;
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        if (SaveManager.instance.saveWorld == null) return;
        int savedGames = SaveManager.instance.saveData.saveGames.Count;
        if (savedGames == 0)
        {
            SaveManager.instance.saveData.saveGames.Clear();
            SaveManager.instance.saveData.saveGames.Add(new SaveGames()); //slot 0
            SaveManager.instance.saveData.saveGames.Add(new SaveGames()); // slot 1
            SaveManager.instance.saveData.saveGames.Add(new SaveGames()); // slot 2
            return;
        }

        int slot = SaveManager.instance.saveWorld.lastLoadedSlot - 1;
        if (slot == -1)//new file
        {
            slot = 0;
            SaveManager.instance.saveWorld.lastLoadedSlot = 1;
        }

        if (SaveManager.instance.saveData.saveGames[slot].savePlayers == null) return;
        if (SaveManager.instance.saveData.saveGames[slot].saveEnemies == null) return;
        if (SaveManager.instance.saveData.saveGames[slot].savePlayers.Count == 0) return;
        if (SaveManager.instance.saveData.saveGames[slot].saveEnemies.Count == 0) return;




        var sg = SaveManager.instance.saveData.saveGames[slot];
        LevelManager.instance.currentLevelCompleted = sg.currentLevel;

        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> PlayerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        PlayerEntities.Sort(new IndexComparer());

        EntityQuery enemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyComponent>());
        NativeArray<Entity> EnemyEntities = enemyQuery.ToEntityArray(Allocator.Persistent);
        EnemyEntities.Sort(new IndexComparer());


        for (int i = 0; i < PlayerEntities.Length; i++)
        {
            Entity e = PlayerEntities[i];
            var pl = SaveManager.instance.saveData.saveGames[slot].savePlayers[i];
            var player = pl.playerData.savedPlayer;
            var health = pl.playerData.savedHealth;
            ecb.SetComponent<PlayerComponent>(e, player);
            ecb.SetComponent<HealthComponent>(e, health);
            Translation ps = new Translation
            {
                Value =
                {
                    x = pl.playerData.position[0], y = pl.playerData.position[1], z = pl.playerData.position[2]
                }
            };
            Debug.Log("ps " + ps);
            ecb.SetComponent<Translation>(e, ps);
        }





        for (int i = 0; i < EnemyEntities.Length; i++)
        {
            Entity e = EnemyEntities[i];
            var en = SaveManager.instance.saveData.saveGames[slot].saveEnemies[i];
            var enemy = en.enemyData.savedEnemy;
            var health = en.enemyData.savedHealth;
            ecb.SetComponent<EnemyComponent>(e, enemy);
            ecb.SetComponent<HealthComponent>(e, health);
            Translation ps = new Translation
            {
                Value =
                {
                    x = en.enemyData.position[0], y = en.enemyData.position[1], z = en.enemyData.position[2]
                }
            };
            ecb.SetComponent<Translation>(e, ps);
        }



        ecb.Playback(EntityManager);
        ecb.Dispose();

        PlayerEntities.Dispose();
        EnemyEntities.Dispose();



    }

}


public class LoadNextLevelSystem : SystemBase
{

    protected override void OnUpdate()
    {

        //bool levelLoaded = GetSingleton<LevelCompleteMenuComponent>().levelLoaded;
        //bool levelSetup = GetSingleton<SceneSwitcherComponent>().saveScene;
        //if (levelLoaded == true || sceneSaved == true) return;//level loaded set to false when showing end level menu so now can load but only after setup is true


        //bool saveScene = SaveLevelManager.instance.saveScene;
        //if (saveScene == false) return;

        bool saveScene = SaveLevelManager.instance.saveScene;
        bool loadNextScene = SaveLevelManager.instance.loadNextScene;

        if (loadNextScene == false) return;


        SaveLevelManager.instance.saveScene = false;
        SaveLevelManager.instance.loadNextScene = false;




        var ecb = new EntityCommandBuffer(Allocator.Temp);


        int savedLevelPlayerIndex = 0;
        Entities.WithoutBurst().WithAll<PlayerComponent>().ForEach((ref ScoreComponent scoreComponent, in Entity e) =>
            {
                scoreComponent = SaveLevelManager.instance.saveLevelPlayers[savedLevelPlayerIndex].playerLevelData
                 .savedLevelScores;
                Debug.Log("load next level " + SaveLevelManager.instance.saveLevelPlayers.Count);

                savedLevelPlayerIndex = savedLevelPlayerIndex + 1;
            }
        ).Run();

        savedLevelPlayerIndex = 0;
        Entities.WithoutBurst().WithAll<PlayerComponent>().ForEach((ref HealthComponent healthComponent, in Entity e) =>
            {
                healthComponent = SaveLevelManager.instance.saveLevelPlayers[savedLevelPlayerIndex].playerLevelData
                 .savedLevelHealth;
                //Debug.Log("id1  " + savedLevelPlayerIndex);


                savedLevelPlayerIndex++;

            }
        ).Run();


        //var sceneSwitcher = GetSingleton<SceneSwitcherComponent>();
        var sceneEntity = GetSingletonEntity<SceneSwitcherComponent>();
        ecb.DestroyEntity(sceneEntity);




        ecb.Playback(EntityManager);
        ecb.Dispose();




    }

}