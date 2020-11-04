﻿using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using System.Linq;



public class SaveSystem : SystemBase
{
    [DeallocateOnJobCompletion]
    public NativeArray<Entity> PlayerEntities;
    [DeallocateOnJobCompletion]
    public NativeArray<Entity> EnemyEntities;


    private EntityQuery playerQuery;
    private EntityQuery enemyQuery;



    protected override void OnCreate()
    {
    }


    protected override void OnUpdate()
    {


        if (SaveManager.instance.saveMainGame == false) return;
        SaveManager.instance.saveMainGame = false;
        Debug.Log("saving main");

        playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        enemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyComponent>());

        PlayerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        EnemyEntities = enemyQuery.ToEntityArray(Allocator.Persistent);

        //setup manager
        int slot = SaveManager.instance.saveWorld.lastLoadedSlot - 1;
        int savedGames = SaveManager.instance.saveData.saveGames.Count;
        if (savedGames == 0)
        {
            SaveManager.instance.saveData.saveGames.Clear();
            SaveManager.instance.saveData.saveGames.Add(new SaveGames()); //slot 0
            SaveManager.instance.saveData.saveGames.Add(new SaveGames()); // slot 1
            SaveManager.instance.saveData.saveGames.Add(new SaveGames()); // slot 2
        }

        SaveManager.instance.saveData.saveGames[slot].savePlayers.Clear();
        SaveManager.instance.saveData.saveGames[slot].saveEnemies.Clear();
        SaveManager.instance.saveData.saveGames[slot].saveLevelData.Clear();

        int level = LevelManager.instance.currentLevelCompleted;
        for (int i = 0; i <= level; i++)
        {
            SaveManager.instance.saveData.saveGames[slot].saveLevelData.Add(new LevelSettings());
        }

        SaveManager.instance.saveData.saveGames[slot].currentLevel = level;
        Debug.Log("save current level " + level);

        for (int i = 0; i < PlayerEntities.Length; i++)
        {

            Entity e = PlayerEntities[i];
            PlayerComponent player = EntityManager.GetComponentData<PlayerComponent>(e);
            HealthComponent health = EntityManager.GetComponentData<HealthComponent>(e);
            Translation ps = EntityManager.GetComponentData<Translation>(e);
            var pl = new SavePlayers
            {
                playerData = new PlayerData
                {
                    savedPlayer = player,
                    savedHealth = health,
                    position =
                    {
                        [0] = ps.Value.x,
                        [1] = ps.Value.y,
                        [2] = ps.Value.z
                    }
                }
            };
            SaveManager.instance.saveData.saveGames[slot].savePlayers.Add(pl);
        }


        for (int i = 0; i < EnemyEntities.Length; i++)
        {
            Entity e = EnemyEntities[i];
            EnemyComponent enemy = EntityManager.GetComponentData<EnemyComponent>(e);
            HealthComponent health = EntityManager.GetComponentData<HealthComponent>(e);
            Translation ps = EntityManager.GetComponentData<Translation>(e);

            var en = new SaveEnemies
            {
                enemyData = new EnemyData
                {
                    savedEnemy = enemy,
                    savedHealth = health,
                    position =
                    {
                        [0] = ps.Value.x,
                        [1] = ps.Value.y,
                        [2] = ps.Value.z
                    }
                }
            };
            SaveManager.instance.saveData.saveGames[slot].saveEnemies.Add(en);
        }
        PlayerEntities.Dispose();
        EnemyEntities.Dispose();
    }


}

public struct IndexComparer : IComparer<Entity>  
{
    public int Compare(Entity a, Entity b)
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (manager.HasComponent<CharacterSaveComponent>(a) == false) return 0;
        if (manager.HasComponent<CharacterSaveComponent>(b) == false) return 0;

        int a_index = manager.GetComponentData<CharacterSaveComponent>(a).saveIndex;
        int b_index = manager.GetComponentData<CharacterSaveComponent>(b).saveIndex;
        if (a_index > b_index)
            return 1;
        else if (a_index < b_index)
            return -1;
        else
            return 0;

    }
}





