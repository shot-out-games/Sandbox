//using System;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Entities.UniversalDelegates;
//using Unity.Jobs;
//using Unity.Transforms;
//using UnityEngine;



////[UpdateInGroup((typeof(InitializationSystemGroup)))]
//public class LoadHolesSystem : SystemBase
//{


//    protected override void OnUpdate()
//    {
//        bool loading = false;

//        Entity loadEntity = Entity.Null;
        
//        Entities.ForEach
//        (
//            (
//                ref LoadComponent load, in Entity e
//            ) =>
//            {
//                loadEntity = e;
//                loading = load.part2;
//                load.part2= false;
//            }
//        ).Run();

//        if (loading == false) return;

//        //Debug.Log("load holes");

//        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
//        //ecb.SetComponent(loadEntity, new LoadComponent { part1 = true, part2 = true });

//        if (SaveManager.instance.saveWorld == null) return;
//        int savedGames = SaveManager.instance.saveData.saveGames.Count;
//        if (savedGames == 0) return;

//        int slot = SaveManager.instance.saveWorld.lastLoadedSlot - 1;
//        int savedPlayers = SaveManager.instance.saveData.saveGames[slot].savePlayers.Count;
//        if (savedPlayers == 0) return;//new game so no need to fill data



//        int index = 0;
//        Entities.WithStructuralChanges().WithoutBurst().ForEach
//        (
//            (
//                in Entity e, in Holes holes
//            ) =>
//            {
//                var hole = SaveManager.instance.saveData.saveGames[slot].savedHoles[index];
//                //Debug.Log("hole " + hole.spawned + " e " + e );
//                if (hole.spawned == true)
//                {
//                    hole.open = false;
//                    holes.SpawnPrefab();
//                    //hole.activate = true;
//                }
//                ecb.SetComponent<HoleComponent>(e, hole);
//                index += 1;
//            }
//        ).Run();




//        ecb.Playback(EntityManager);
//        ecb.Dispose();

//    }

//}


