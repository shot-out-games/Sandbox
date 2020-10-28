using System;
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

//[UpdateAfter(typeof(LoadHolesSystem))]
[UpdateInGroup((typeof(PresentationSystemGroup)))]
//[UpdateAfter(typeof(GameObjectAfterConversionGroup))]
//[DisableAutoCreation]
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
                load.part1= false;
            }
        ).Run();


        if (loaded == false) return;

        //Debug.Log("load system");

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        //ecb.SetComponent(loadEntity, new LoadComponent { part1 = true, part2 = true });

        if (SaveManager.instance.saveWorld == null) return;
        int savedGames = SaveManager.instance.saveData.saveGames.Count;
        if (savedGames == 0) return;

        int slot = SaveManager.instance.saveWorld.lastLoadedSlot - 1;
        int savedPlayers = SaveManager.instance.saveData.saveGames[slot].savePlayers.Count;
        if (savedPlayers == 0) return;//new game so no need to fill data


        var sg = SaveManager.instance.saveData.saveGames[slot];
        LevelManager.instance.currentLevel = sg.currentLevel;

        var dead_e = GetSingletonEntity<DeadMenuComponent>();
        //ecb.SetComponent<DeadMenuComponent>(dead_e, SaveManager.instance.saveData.saveGames[slot].savedDeadWorld);

        var win_e = GetSingletonEntity<WinnerMenuComponent>();
        //ecb.SetComponent<WinnerMenuComponent>(win_e, SaveManager.instance.saveData.saveGames[slot].savedWinnerWorld);

        var level_e = GetSingletonEntity<LevelCompleteMenuComponent>();
        //ecb.SetComponent<LevelCompleteMenuComponent>(level_e, SaveManager.instance.saveData.saveGames[slot].savedLevelWorld);

        int currentLevel =  LevelManager.instance.currentLevel;


        LevelManager.instance.NpcSaved = SaveManager.instance.saveData.saveGames[slot].NpcSaved;
        LevelManager.instance.NpcDead = SaveManager.instance.saveData.saveGames[slot].NpcDead;

        for (int i = 0; i <= currentLevel; i++)
        {

            LevelManager.instance.levelSettings[i].NpcSaved =
                SaveManager.instance.saveData.saveGames[slot].saveLevelData[i].NpcSaved;
            LevelManager.instance.levelSettings[i].NpcDead =
                SaveManager.instance.saveData.saveGames[slot].saveLevelData[i].NpcDead;
        }



        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> PlayerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        PlayerEntities.Sort(new IndexComparer());

        EntityQuery enemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyComponent>());
        NativeArray<Entity> EnemyEntities = enemyQuery.ToEntityArray(Allocator.Persistent);
        EnemyEntities.Sort(new IndexComparer());

        EntityQuery npcQuery = GetEntityQuery(ComponentType.ReadOnly<NpcComponent>());
        NativeArray<Entity> NpcEntities = npcQuery.ToEntityArray(Allocator.Persistent);
        //NpcEntities.Sort(new IndexComparer());

        EntityQuery weaponQuery = GetEntityQuery(ComponentType.ReadOnly<WeaponItemComponent>());
        NativeArray<Entity> WeaponEntities = weaponQuery.ToEntityArray(Allocator.Persistent);
        //NpcEntities.Sort(new IndexComparer());

        EntityQuery powerQuery = GetEntityQuery(ComponentType.ReadOnly<PowerItemComponent>());
        NativeArray<Entity> PowerEntities = powerQuery.ToEntityArray(Allocator.Persistent);
        //NpcEntities.Sort(new IndexComparer());


        for (int i = 0; i < PlayerEntities.Length; i++)
        {
            Entity e = PlayerEntities[i];
            var pl = SaveManager.instance.saveData.saveGames[slot].savePlayers[i];
            var player = pl.playerData.savedPlayer;
            //var skillTree = pl.playerData.skillTree;
            var health = pl.playerData.savedHealth;
            //var control = pl.playerData.savedControl;
            //var level = pl.playerData.savedLevelComplete;
            //var winner = pl.playerData.savedWinner;
            //var dead = pl.playerData.savedDead;
            //var attach = pl.playerData.savedAttachWeapon;
            ecb.SetComponent<PlayerComponent>(e, player);
            //ecb.SetComponent<SkillTreeComponent>(e, skillTree);
            ecb.SetComponent<HealthComponent>(e, health);
            //ecb.SetComponent<ControlBarComponent>(e, control);
            //ecb.SetComponent<LevelCompleteComponent>(e, level);
            //ecb.SetComponent<WinnerComponent>(e, winner);
            //ecb.SetComponent<DeadComponent>(e, dead);
            //ecb.SetComponent<AttachWeaponComponent>(e, attach);
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





        //for (int i = 0; i < EnemyEntities.Length; i++)
        //{
        //    Entity e = EnemyEntities[i];
        //    var en = SaveManager.instance.saveData.saveGames[slot].saveEnemies[i];
        //    var enemy = en.enemyData.savedEnemy;
        //    var skillTree = en.enemyData.skillTree;
        //    var health = en.enemyData.savedHealth;
        //    var level = en.enemyData.savedLevelComplete;
        //    var winner = en.enemyData.savedWinner;
        //    var dead = en.enemyData.savedDead;
        //    ecb.SetComponent<EnemyComponent>(e, enemy);
        //    ecb.SetComponent<SkillTreeComponent>(e, skillTree);
        //    ecb.SetComponent<HealthComponent>(e, health);
        //    ecb.SetComponent<LevelCompleteComponent>(e, level);
        //    ecb.SetComponent<WinnerComponent>(e, winner);
        //    ecb.SetComponent<DeadComponent>(e, dead);
        //    Translation ps = new Translation
        //    {
        //        Value =
        //        {
        //            x = en.enemyData.position[0], y = en.enemyData.position[1], z = en.enemyData.position[2]
        //        }
        //    };
        //    ecb.SetComponent<Translation>(e, ps);
        //}


        //for (int i = 0; i < NpcEntities.Length; i++)
        //{
        //    Entity e = NpcEntities[i];
        //    var np = SaveManager.instance.saveData.saveGames[slot].saveNpc[i];
        //    var npc = np.npcData.savedNpc;
        //    var skillTree = np.npcData.skillTree;
        //    var health = np.npcData.savedHealth;
        //    var level = np.npcData.savedLevelComplete;
        //    var winner = np.npcData.savedWinner;
        //    var dead = np.npcData.savedDead;
        //    ecb.SetComponent<NpcComponent>(e, npc);
        //    ecb.SetComponent<SkillTreeComponent>(e, skillTree);
        //    ecb.SetComponent<HealthComponent>(e, health);
        //    ecb.SetComponent<LevelCompleteComponent>(e, level);
        //    ecb.SetComponent<WinnerComponent>(e, winner);
        //    ecb.SetComponent<DeadComponent>(e, dead);
        //    Translation ps = new Translation
        //    {
        //        Value =
        //        {
        //            x = np.npcData.position[0], y = np.npcData.position[1], z = np.npcData.position[2]
        //        }
        //    };
        //    ecb.SetComponent<Translation>(e, ps);
        //    //Debug.Log("npc " + ps + " e " + e);
        //}

        //Debug.Log("we " + WeaponEntities.Length);
        //for (int i = 0; i < WeaponEntities.Length; i++)
        //{
        //    Entity e = WeaponEntities[i];
        //    WeaponItemComponent saveWeapon = SaveManager.instance.saveData.saveGames[slot].saveWeapons[i];
        //    //ecb.SetComponent<WeaponItemComponent>(e, saveWeapon);
        //    Debug.Log("wea " + saveWeapon.active + " e " + e);
        //}

        //Debug.Log("power " + PowerEntities.Length);
        //for (int i = 0; i < PowerEntities.Length; i++)
        //{
        //    Entity e = PowerEntities[i];
        //    PowerItemComponent savePowers = SaveManager.instance.saveData.saveGames[slot].savePowerItems[i];
        //    ecb.SetComponent<PowerItemComponent>(e, savePowers);
        //    Debug.Log("pwr " + savePowers.active + " e " + e);
        //}

        ecb.Playback(EntityManager);
        ecb.Dispose();

        PlayerEntities.Dispose();
        EnemyEntities.Dispose();
        NpcEntities.Dispose();
        WeaponEntities.Dispose();
        PowerEntities.Dispose();


        //Entities.WithoutBurst().WithAll<PlayerComponent>().ForEach(
        //    (
        //        PlayerComponentAuthoring player, WeaponManager attachWeapon, AttachWeaponComponent attachWeaponComponent,
        //        Transform transform
        //    ) =>
        //    {
        //        var e = player.playerEntity;
        //        var attachedSlot = EntityManager.GetComponentData<AttachWeaponComponent>(e).attachedWeaponSlot;
        //        var ps = EntityManager.GetComponentData<Translation>(e);
        //        var position = new Vector3(ps.Value.x, ps.Value.y, ps.Value.z);
        //        if (attachedSlot >= 0) attachWeapon.AttachPickWeapons(attachedSlot);
        //        transform.position = position;
        //    }
        //).Run();


        //Entities.WithoutBurst().WithAll<EnemyComponent>().ForEach(
        //    (
        //        EnemyComponentAuthoring enemy, Transform transform
        //    ) =>
        //    {
        //        var e = enemy.enemyEntity;
        //        var ps = EntityManager.GetComponentData<Translation>(e);
        //        var position = new Vector3(ps.Value.x, ps.Value.y, ps.Value.z);
        //        transform.position = position;
        //    }
        //).Run();


        //Entities.WithoutBurst().WithAll<NpcComponent>().ForEach(
        //    (
        //        NpcComponentAuthoring npc, Transform transform
        //    ) =>
        //    {
        //        var e = npc.npcEntity;
        //        var ps = EntityManager.GetComponentData<Translation>(e);
        //        var position = new Vector3(ps.Value.x, ps.Value.y, ps.Value.z);
        //        //Debug.Log("npc ps " + position);
        //        transform.position = position;
        //    }
        //).Run();






    }

}


