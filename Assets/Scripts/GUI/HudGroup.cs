using UnityEngine;
using TMPro;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;


//mish mash
//not consistent using entity components for some and static level settings (easier) for others

public class HudGroup : MonoBehaviour, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity e;
    public int cubes = 5;

    [SerializeField]
    private TextMeshProUGUI labelLevelName;
    [SerializeField]
    private TextMeshProUGUI label0;
    [SerializeField]
    private TextMeshProUGUI label1;
    [SerializeField] private TextMeshProUGUI label2;

    [SerializeField]
    private TextMeshProUGUI labelWorldName;
    [SerializeField]
    private TextMeshProUGUI label3;
    [SerializeField]
    private TextMeshProUGUI label4;
    [SerializeField]
    private TextMeshProUGUI label5;



    [SerializeField]
    LevelCompleteMenuGroup levelCompleteMenuGroup;
    [SerializeField]
    WinnerMenuGroup winnerCompleteMenuGroup;


//    NativeArray<Entity> list;//experimental

    void ShowLabelLevelHeader()
    {

        int currentLevel = LevelManager.instance.currentLevel;
        string levelName = LevelManager.instance.levelSettings[currentLevel].levelName;
        labelLevelName.text = levelName;
    }

    void ShowLabelGameHeader()
    {

        string worldName = LevelManager.instance.worldName;
        labelWorldName.text = worldName;
    }


    public void ShowLabelLevelTargets(string _label, int _count)
    {
        //label0.text = "Cubes Remain " + cubes;
        label0.text = _label + " " + _count;


    }


    void ShowLabelLevelSaved()
    {
        Entity entity = levelCompleteMenuGroup.entity;
        bool hasComponent = manager.HasComponent(entity, typeof(LevelCompleteMenuComponent));
        if (hasComponent == false) return;

        int currentLevel = LevelManager.instance.currentLevel;
        string levelName = LevelManager.instance.levelSettings[currentLevel].levelName;
        int targetsReachedHome = LevelManager.instance.levelSettings[currentLevel].NpcSaved;
        //int targetsReachedHome = manager.GetComponentData<LevelCompleteMenuComponent>(entity).levelTargetReachedCounter;
        label1.text = "Saved  :  " + targetsReachedHome.ToString();

    }

    void ShowLabelLevelDead()
    {
        int currentLevel = LevelManager.instance.currentLevel;
        int deadNPC = LevelManager.instance.levelSettings[currentLevel].NpcDead;

        label2.text = "Dead : " + deadNPC;


    }



    void ShowLabelGameTargets()
    {
        int currentLevel = LevelManager.instance.currentLevel;
        int potentialCumulativeTargets = LevelManager.instance.potentialCumulativeGameTargets[currentLevel];

        label3.text = "Robies : " + potentialCumulativeTargets;


    }

    void ShowLabelGameSaved()
    {
        Entity entity = winnerCompleteMenuGroup.entity;
        bool hasComponent = manager.HasComponent(entity, typeof(WinnerMenuComponent));
        if (hasComponent == false) return;


        int currentLevel = LevelManager.instance.currentLevel;
        int targetsReachedHome = LevelManager.instance.NpcSaved;

        //  int targetsReachedHome =
        //   manager.GetComponentData<WinnerMenuComponent>(entity).levelTargetReachedCounter; 
        //manager.GetComponentData<WinnerMenuComponent>(entity).endGameTargetReachedCounter;
        label4.text = "Saved : " + targetsReachedHome.ToString();


    }

    void ShowLabelGameDead()
    {
        Entity entity = winnerCompleteMenuGroup.entity;
        bool hasComponent = manager.HasComponent(entity, typeof(WinnerMenuComponent));
        if (hasComponent == false) return;

        int currentLevel = LevelManager.instance.currentLevel;


        int deadNpc =
            manager.GetComponentData<WinnerMenuComponent>(entity).npcDeadCounter;
        label5.text = "Dead :  " + deadNpc.ToString();


    }



    void Update()
    {
        if (manager == null || e == Entity.Null) return;

        //ShowLabelLevelHeader();
        //ShowLabelLevelTargets();
        //ShowLabelLevelSaved();
        //ShowLabelLevelDead();

        //ShowLabelGameHeader();
        //ShowLabelGameTargets();
        //ShowLabelGameSaved();
        //ShowLabelGameDead();

    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        manager = dstManager;
        this.e = entity;
    }
}

