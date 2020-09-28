using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct SaveComponent : IComponentData
{
    public bool value;
}


public class SaveSystem : SystemBase
{
    [DeallocateOnJobCompletion]
    public NativeArray<Entity> PlayerEntities;
    [DeallocateOnJobCompletion]
    public NativeArray<Entity> EnemyEntities;
    [DeallocateOnJobCompletion]
    public NativeArray<Entity> NpcEntities;

    [DeallocateOnJobCompletion]
    public NativeArray<Entity> WeaponEntities;
    [DeallocateOnJobCompletion]
    public NativeArray<Entity> PowerEntities;

    private EntityQuery playerQuery;
    private EntityQuery enemyQuery;
    private EntityQuery npcQuery;
    private EntityQuery weaponQuery;
    private EntityQuery powerQuery;



    protected override void OnCreate()
    {
        //EnemyEntities = new NativeList<Entity>(100, Allocator.Persistent);
    }


    protected override void OnUpdate()
    {
        bool saving = false;

        Entities.WithoutBurst().ForEach
        (
            (
                ref SaveComponent save
            ) =>
            {
                saving = save.value;
                save.value = false;
            }
        ).Run();


        if (saving == false) return;

        playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        enemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyComponent>());
        npcQuery = GetEntityQuery(ComponentType.ReadOnly<NpcComponent>());
        weaponQuery = GetEntityQuery(ComponentType.ReadOnly<WeaponItemComponent>());
        powerQuery = GetEntityQuery(ComponentType.ReadOnly<PowerItemComponent>());

        PlayerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        EnemyEntities = enemyQuery.ToEntityArray(Allocator.Persistent);
        NpcEntities = npcQuery.ToEntityArray(Allocator.Persistent);
        WeaponEntities = weaponQuery.ToEntityArray(Allocator.Persistent);
        PowerEntities = powerQuery.ToEntityArray(Allocator.Persistent);

        //PlayerEntities.Sort(new IndexComparer());
        //EnemyEntities.Sort(new IndexComparer());
        //NpcEntities.Sort(new IndexComparer());


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
        SaveManager.instance.saveData.saveGames[slot].saveNpc.Clear();
        SaveManager.instance.saveData.saveGames[slot].saveWeapons.Clear();
        SaveManager.instance.saveData.saveGames[slot].savePowerItems.Clear();


        SaveManager.instance.saveData.saveGames[slot].savedHoles.Clear();
        SaveManager.instance.saveData.saveGames[slot].saveLevelData.Clear();

        int level = LevelManager.instance.currentLevel;
        for (int i = 0; i <= level; i++)
        {
            SaveManager.instance.saveData.saveGames[slot].saveLevelData.Add(new LevelSettings());
        }

        SaveManager.instance.saveData.saveGames[slot].currentLevel = level;

        var single_dead = GetSingleton<DeadMenuComponent>();
        SaveManager.instance.saveData.saveGames[slot].savedDeadWorld = single_dead;

        var single_win = GetSingleton<WinnerMenuComponent>();
        SaveManager.instance.saveData.saveGames[slot].savedWinnerWorld = single_win;

        var single_level = GetSingleton<LevelCompleteMenuComponent>();
        SaveManager.instance.saveData.saveGames[slot].savedLevelWorld = single_level;

        SaveManager.instance.saveData.saveGames[slot].NpcSaved = LevelManager.instance.NpcSaved;
        SaveManager.instance.saveData.saveGames[slot].NpcDead = LevelManager.instance.NpcDead;



        for (int i = 0; i <= level; i++)
        {
            SaveManager.instance.saveData.saveGames[slot].saveLevelData[i].NpcSaved =
                LevelManager.instance.levelSettings[i].NpcSaved;
            SaveManager.instance.saveData.saveGames[slot].saveLevelData[i].NpcDead =
                LevelManager.instance.levelSettings[i].NpcDead;
        }


        Entities.WithoutBurst().ForEach
        (
            (
                HoleComponent holeComponent
            ) =>
            {
                SaveManager.instance.saveData.saveGames[slot].savedHoles.Add(holeComponent);
            }
        ).Run();


        //Entities.WithoutBurst().ForEach
        //(
        //    (
        //        WeaponItemComponent weaponComponent
        //    ) =>
        //    {
        //        SaveManager.instance.saveData.saveGames[slot].saveWeapons.Add(weaponComponent);
        //    }
        //).Run();

        Entities.WithoutBurst().ForEach
        (
            (
                PowerItemComponent powerComponent
            ) =>
            {
                SaveManager.instance.saveData.saveGames[slot].savePowerItems.Add(powerComponent);
            }
        ).Run();




        for (int i = 0; i < PlayerEntities.Length; i++)
        {

            Entity e = PlayerEntities[i];
            PlayerComponent player = EntityManager.GetComponentData<PlayerComponent>(e);
            HealthComponent health = EntityManager.GetComponentData<HealthComponent>(e);
            ControlBarComponent control = EntityManager.GetComponentData<ControlBarComponent>(e);
            SkillTreeComponent skill = EntityManager.GetComponentData<SkillTreeComponent>(e);
            DeadComponent dead = EntityManager.GetComponentData<DeadComponent>(e);
            LevelCompleteComponent levelComplete = EntityManager.GetComponentData<LevelCompleteComponent>(e);
            WinnerComponent win = EntityManager.GetComponentData<WinnerComponent>(e);
            AttachWeaponComponent attachWeapon = EntityManager.GetComponentData<AttachWeaponComponent>(e);
            Translation ps = EntityManager.GetComponentData<Translation>(e);
            var pl = new SavePlayers
            {
                playerData = new PlayerData
                {
                    savedPlayer = player,
                    savedHealth = health,
                    savedControl =  control,
                    skillTree = skill,
                    savedDead = dead,
                    savedLevelComplete = levelComplete,
                    savedWinner = win,
                    savedAttachWeapon = attachWeapon,
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
            SkillTreeComponent skill = EntityManager.GetComponentData<SkillTreeComponent>(e);
            DeadComponent dead = EntityManager.GetComponentData<DeadComponent>(e);
            LevelCompleteComponent levelComplete = EntityManager.GetComponentData<LevelCompleteComponent>(e);
            WinnerComponent win = EntityManager.GetComponentData<WinnerComponent>(e);
            Translation ps = EntityManager.GetComponentData<Translation>(e);

            var en = new SaveEnemies
            {
                enemyData = new EnemyData
                {
                    savedEnemy = enemy,
                    savedHealth = health,
                    skillTree = skill,
                    savedDead = dead,
                    savedLevelComplete = levelComplete,
                    savedWinner = win,
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

        for (int i = 0; i < NpcEntities.Length; i++)
        {
            Entity e = NpcEntities[i];

            NpcComponent npc = EntityManager.GetComponentData<NpcComponent>(e);
            HealthComponent health = EntityManager.GetComponentData<HealthComponent>(e);
            SkillTreeComponent skill = EntityManager.GetComponentData<SkillTreeComponent>(e);
            DeadComponent dead = EntityManager.GetComponentData<DeadComponent>(e);
            LevelCompleteComponent levelComplete = EntityManager.GetComponentData<LevelCompleteComponent>(e);
            WinnerComponent win = EntityManager.GetComponentData<WinnerComponent>(e);

            Translation ps = EntityManager.GetComponentData<Translation>(e);

            var np = new SaveNpc
            {
                npcData = new NpcData
                {
                    savedNpc = npc,
                    savedHealth = health,
                    skillTree = skill,
                    savedDead = dead,
                    savedLevelComplete = levelComplete,
                    savedWinner = win,
                    position =
                    {
                        [0] = ps.Value.x,
                        [1] = ps.Value.y,
                        [2] = ps.Value.z
                    }
                }
            };
            SaveManager.instance.saveData.saveGames[slot].saveNpc.Add(np);
        }

        PlayerEntities.Dispose();
        EnemyEntities.Dispose();
        NpcEntities.Dispose();
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





