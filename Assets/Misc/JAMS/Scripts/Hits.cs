using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;


[System.Serializable]
public struct HitsComponent : IComponentData
{
    public bool hit;
    public int hitCounter;
}


public class Hits : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private bool skipStartTrigger = true; //skip if on hole (robots) when starting

    public Entity hitEntity;
    private EntityManager manager;
    private string tag = "Trigger";

    void Start()
    {
        //tag = "Trigger";
    }


    void Update()
    {
        //if (gameObject.tag == "NPC")
        //{
        //    Debug.DrawRay(transform.position, transform.forward * 1.0f, Color.red);
        //}

        //int layerMask = ~LayerMask.NameToLayer("Goal");//???
        //float r = 1.0f;
        //RaycastHit hit;
        //if (Physics.SphereCast(transform.position, r, transform.forward, out hit, r, layerMask))
        //{

        //    if (hit.collider.tag == "Goal" && gameObject.tag == "NPC")
        //    {
        //        //OnTriggerEnter(hit.collider);
        //        Debug.Log("hit " + hit.collider.tag + " " + gameObject.tag);
        //    }
        //}


    }


    private void OnCollisionEnter(Collision other)
    {

        if (manager == null) return;


        if (gameObject.tag == "Player")
        {
            //Debug.Log("player hit trigger " + other.gameObject.tag);
        }



        int currentLevel = LevelManager.instance.currentLevel;
        //int complete = LevelManager.instance.NpcDead + LevelManager.instance.NpcSaved;
        int complete = LevelManager.instance.levelSettings[currentLevel].NpcDead + LevelManager.instance.levelSettings[currentLevel].NpcSaved;
        int targets = LevelManager.instance.levelSettings[currentLevel].potentialLevelTargets;
        //targets = 1;

        if (gameObject.tag == "NPC")
        {
            if (manager.HasComponent<NpcComponent>(hitEntity) == false) return;
            if (manager.GetComponentData<NpcComponent>(hitEntity).active == false) return;
        }


        if (other.gameObject.GetComponent<Holes>())
            if (other.gameObject.tag == tag && other.gameObject.GetComponent<Holes>().open)
            {
                var holeHitCounter = other.gameObject.GetComponent<Holes>().holeHitCounter;

                if (holeHitCounter == 0 && skipStartTrigger)//probably dont need now since entities created at start
                {
                    skipStartTrigger = false;
                    //no damage allowed yet 
                }
                else
                {
                    if (manager.HasComponent(hitEntity, typeof(HitsComponent)))
                    {
                        var hitsComponent = manager.GetComponentData<HitsComponent>(hitEntity);
                        hitsComponent.hit = true;
                        hitsComponent.hitCounter++;
                        manager.SetComponentData(hitEntity, hitsComponent);
                    }
                }
            }

        //win condition
        if ((other.gameObject.tag == "Goal" || other.gameObject.tag == "End") && gameObject.tag == "NPC")
        {

            //goalCounter normally added for real goals ie close hole count - then winner system check to see if goal reached
            if (manager.HasComponent(hitEntity, typeof(WinnerComponent)) == false) return;
            if (manager.HasComponent(hitEntity, typeof(LevelCompleteComponent)) == false) return;
            if (other.gameObject.GetComponent<DestinationEligible>() == false) return;

            Entity e = other.gameObject.GetComponent<DestinationEligible>().entity;//object entity
            bool eligible = manager.GetComponentData<DestinationEligibleComponent>(e).priority;
            if (eligible == false) return;


            WinnerComponent winner = manager.GetComponentData<WinnerComponent>(hitEntity);
            winner.goalCounter += 1;
            if (winner.endGameReached == true || winner.targetReached == true) return;

            Debug.Log("npc coll");

            if (other.gameObject.tag == "End") winner.endGameReached = true;
            else if (other.gameObject.tag == "Goal") winner.targetReached = true;

            manager.SetComponentData(hitEntity, winner);
            LevelCompleteComponent levelComplete = manager.GetComponentData<LevelCompleteComponent>(hitEntity);
            levelComplete.goalCounter += 1;
            levelComplete.targetReached = true;
            levelComplete.active = true;
            manager.SetComponentData(hitEntity, levelComplete);
        }
        else if ((other.gameObject.tag == "Goal" || other.gameObject.tag == "End") && gameObject.tag == "Player" && complete >= targets)
        //else if (gameObject.tag == "Player" && complete >= targets)
        {



                    //goalCounter normally added for real goals ie close hole count - then winner system check to see if goal reached
                    if (manager.HasComponent(hitEntity, typeof(WinnerComponent)) == false) return;
            if (manager.HasComponent(hitEntity, typeof(LevelCompleteComponent)) == false) return;
            //if (other.gameObject.GetComponent<DestinationEligible>() == false) return;

           // Entity e = other.gameObject.GetComponent<DestinationEligible>().entity;//object entity
            //bool eligible = manager.GetComponentData<DestinationEligibleComponent>(e).priority;
            //if (eligible == false) return;


            WinnerComponent winner = manager.GetComponentData<WinnerComponent>(hitEntity);
            winner.goalCounter += 1;
            if (winner.endGameReached == true) return;


            //Debug.Log("tag " + gameObject.tag);
            Debug.Log("player coll");


            if (other.gameObject.tag == "End") winner.endGameReached = true;
            else if (other.gameObject.tag == "Goal") winner.targetReached = true;


            manager.SetComponentData(hitEntity, winner);
            LevelCompleteComponent levelComplete = manager.GetComponentData<LevelCompleteComponent>(hitEntity);
            levelComplete.goalCounter += 1;
            levelComplete.targetReached = true;
            levelComplete.active = true;
            manager.SetComponentData(hitEntity, levelComplete);
        }

    }



    private void OnTriggerEnter(Collider other)//needed ????
    {

        if (manager == null) return;

        //if (gameObject.tag == "Player")
        //{
        //    Debug.Log("player hit trigger " + other.gameObject.tag);
        //}



        int currentLevel = LevelManager.instance.currentLevel;
        //int complete = LevelManager.instance.NpcDead + LevelManager.instance.NpcSaved;
        int complete = LevelManager.instance.levelSettings[currentLevel].NpcDead + LevelManager.instance.levelSettings[currentLevel].NpcSaved;
        int targets = LevelManager.instance.levelSettings[currentLevel].potentialLevelTargets;
        //targets = 1;


        if (gameObject.tag == "NPC")
        {
            if (manager.HasComponent<NpcComponent>(hitEntity) == false) return;
            if (manager.GetComponentData<NpcComponent>(hitEntity).active == false) return;
        }


        if (other.GetComponent<Holes>())
            if (other.gameObject.tag == tag && other.GetComponent<Holes>().open)
            {
                var holeHitCounter = other.GetComponent<Holes>().holeHitCounter;

                if (holeHitCounter == 0 && skipStartTrigger)//probably dont need now since entities created at start
                {
                    skipStartTrigger = false;
                    //no damage allowed yet 
                }
                else
                {
                    if (manager.HasComponent(hitEntity, typeof(HitsComponent)))
                    {
                        var hitsComponent = manager.GetComponentData<HitsComponent>(hitEntity);
                        hitsComponent.hit = true;
                        hitsComponent.hitCounter++;
                        manager.SetComponentData(hitEntity, hitsComponent);
                    }
                }
            }

        //win condition
        if ((other.gameObject.tag == "Goal" || other.gameObject.tag == "End") && gameObject.tag == "NPC")
        {
            //goalCounter normally added for real goals ie close hole count - then winner system check to see if goal reached
            if (manager.HasComponent(hitEntity, typeof(WinnerComponent)) == false) return;
            if (manager.HasComponent(hitEntity, typeof(LevelCompleteComponent)) == false) return;
            if (other.gameObject.GetComponent<DestinationEligible>() == false) return;

            Entity e = other.gameObject.GetComponent<DestinationEligible>().entity;//object entity
            bool eligible = manager.GetComponentData<DestinationEligibleComponent>(e).priority;
            if (eligible == false) return;


            WinnerComponent winner = manager.GetComponentData<WinnerComponent>(hitEntity);
            winner.goalCounter += 1;
            if (winner.endGameReached == true || winner.targetReached == true) return;

            Debug.Log("npc trigger");


            //Debug.Log("tag " + gameObject.tag);

            if (other.gameObject.tag == "End") winner.endGameReached = true;
            else if (other.gameObject.tag == "Goal") winner.targetReached = true;

            manager.SetComponentData(hitEntity, winner);
            LevelCompleteComponent levelComplete = manager.GetComponentData<LevelCompleteComponent>(hitEntity);
            levelComplete.goalCounter += 1;
            levelComplete.targetReached = true;
            levelComplete.active = true;
            manager.SetComponentData(hitEntity, levelComplete);
        }
        else if ((other.gameObject.tag == "Goal" || other.gameObject.tag == "End") && gameObject.tag == "Player" && complete >= targets)
        {



            //goalCounter normally added for real goals ie close hole count - then winner system check to see if goal reached
            if (manager.HasComponent(hitEntity, typeof(WinnerComponent)) == false) return;
            if (manager.HasComponent(hitEntity, typeof(LevelCompleteComponent)) == false) return;
            //if (other.gameObject.GetComponent<DestinationEligible>() == false) return;

            //Entity e = other.gameObject.GetComponent<DestinationEligible>().entity;//object entity
            //bool eligible = manager.GetComponentData<DestinationEligibleComponent>(e).priority;
            //if (eligible == false) return;


            WinnerComponent winner = manager.GetComponentData<WinnerComponent>(hitEntity);
            winner.goalCounter += 1;
            if (winner.endGameReached == true) return;
            Debug.Log("player trigger");


            //Debug.Log("tag " + gameObject.tag);


            if (other.gameObject.tag == "End") winner.endGameReached = true;
            else if (other.gameObject.tag == "Goal") winner.targetReached = true;


            manager.SetComponentData(hitEntity, winner);
            LevelCompleteComponent levelComplete = manager.GetComponentData<LevelCompleteComponent>(hitEntity);
            levelComplete.goalCounter += 1;
            levelComplete.targetReached = true;
            levelComplete.active = true;
            manager.SetComponentData(hitEntity, levelComplete);
        }
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        hitEntity = entity;
        manager = dstManager;
        dstManager.AddComponentData<HitsComponent>(entity, new HitsComponent() { hit = false, hitCounter = 0 });
    }
}


public class HitsSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        float playerDamageIncreaseMultiplier = 0;
        bool npcDead = false;
        bool playerDamaged = false;
        Entities.WithoutBurst().WithStructuralChanges().ForEach(
            (ref HitsComponent hitsComponent, in Hits hits) =>
            {
                if (hitsComponent.hit)
                {
                    playerDamageIncreaseMultiplier = .15f;

                    npcDead = EntityManager.HasComponent<NpcComponent>(hits.hitEntity);//can be player in pit
                    playerDamaged = !npcDead;

                    hitsComponent.hit = false;
                    ecb.AddComponent<DamageComponent>(hits.hitEntity,
                        new DamageComponent { DamageLanded = 0, DamageReceived = 100 });
                }
            }
        ).Run();




        if (npcDead || playerDamaged)
        {

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (ref HealthComponent health,
                    in PlayerComponent player,
                    in RatingsComponent ratings
                ) =>
                {
                    if (npcDead == true)
                    {
                        float damage = ratings.maxHealth * .15f;
                        health.TotalDamageReceived = health.TotalDamageReceived + damage;
                    }

                }
            ).Run();


            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (in StartGameMenuComponent messageMenuComponent, in StartGameMenuGroup messageMenu) =>
                {
                    if (npcDead == true)
                    {
                        messageMenu.messageString = "Little Roby has perished";
                    }
                    else
                    {
                        messageMenu.messageString = "Please get out of the flames";
                    }
                    messageMenu.ShowMenu();
                }
            ).Run();

        }



        //    Entities.WithoutBurst().WithStructuralChanges().ForEach(
        //        (LevelOpen levelOpen, StartGameMenuComponent messageMenuComponent, StartGameMenuGroup messageMenu) =>
        //        {
        //            Debug.Log("show" + levelOpen.oneRemains);

        //            if (levelOpen.oneRemains == true)
        //            {
        //                messageMenu.messageString = "One Little Roby needs your help";
        //                messageMenu.ShowMenu();
        //                levelOpen.messageShown = true;

        //            }
        //        }
        //    ).Run();

        //}

        //var start_e = GetSingletonEntity<StartGameMenuComponent>();



        //float damage = entityManager.GetComponentData<HealthComponent>(entity).TotalDamageReceived;



        ecb.Playback(EntityManager);
        ecb.Dispose();

        return default;
    }
}