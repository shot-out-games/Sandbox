using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using EntityCommandBuffer = Unity.Entities.EntityCommandBuffer;

[System.Serializable]
public struct HoleComponent : IComponentData
{
    [System.NonSerialized]
    public Entity holeEntity;
    //public bool activate;
    public bool spawned;
    public bool open;
    public int holeHitCounter;
    public int hitTag;
    public int inLevel;

}

public class Holes : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    private EntityManager manager;
    //private Entity robotEntity;
    private Entity holeEntity;
    [HideInInspector]
    public Entity hitEntity;
    [SerializeField]
    public bool open;
    [SerializeField]
    private Material closedHoleMaterial;
    [SerializeField]
    private Material openHoleMaterial;
    private Mesh mesh;

    public int holeHitCounter;//in component but need to access in hits mono
    private bool spawned;
    [SerializeField]
    int inLevel;
    [SerializeField] ParticleSystem particleSystem;
    ParticleSystem psInstance;

    public GameObject robotPrefab;
    private GameObject robotGameObject;

    void Awake()
    {
        var go = Instantiate(robotPrefab, transform.position, transform.rotation);
        go.GetComponent<NpcComponentAuthoring>().Spawned(gameObject.name);
        robotGameObject = go;
        //robotGameObject.SetActive(false);


        open = true;
        mesh = GetComponent<MeshFilter>().mesh;
        if (particleSystem != null)
        {
            //ParticleSystem ps = Instantiate(particleSystem, transform.position, Quaternion.identity) as ParticleSystem;
            psInstance = Instantiate(particleSystem, transform.position, Quaternion.identity) as ParticleSystem;
            psInstance.Play(true);
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {

        referencedPrefabs.Add(robotPrefab);
    }

    public void SpawnPrefab()//only used by system - no other way ?
    {

        robotGameObject.GetComponent<NpcComponentAuthoring>().Activate();

        open = false;
        //holeComponent.activate = true;

        //robotGameObject.SetActive(true);
        //var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        //var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(robotPrefab, settings);
        //Entity e = manager.Instantiate(robotEntity);
        //robotPrefab.GetComponent<NpcComponentAuthoring>().AddComponents(e);
        //var go = Instantiate(robotPrefab, transform.position, transform.rotation);

    }

    private void OnTriggerEnter(Collider other)
    {
        var _tag = other.tag == "Ammo" || other.tag == "Fireball" ? 1 : 0;
        if (_tag == 0 || holeEntity == Entity.Null) return;
        if (manager.HasComponent<HoleComponent>(holeEntity) == false) return;
        var holeComponent = manager.GetComponentData<HoleComponent>(holeEntity);
        if (holeComponent.inLevel != LevelManager.instance.currentLevel) return;
        open = !open;
        holeComponent.open = open;
        holeComponent.hitTag = _tag;
        holeHitCounter = holeComponent.holeHitCounter;
        if (spawned == false && holeHitCounter == 0)
        {
            spawned = true;
            robotGameObject.GetComponent<NpcComponentAuthoring>().Activate();
            //holeComponent.activate = true;
            // robotGameObject.SetActive(true);
            //robotGameObject.SetActive(true);

            //var go = Instantiate(robotPrefab, transform.position, transform.rotation);
            //go.GetComponent<NpcComponentAuthoring>().npcEntity = holeEntity;
            //go.GetComponent<NpcComponentAuthoring>().manager = manager;
            //go.GetComponent<NpcComponentAuthoring>().AddComponents();

            //go.GetComponent<NpcComponentAuthoring>().Spawned(gameObject.name);

        }
        holeComponent.spawned = spawned;


        manager.SetComponentData(holeEntity, holeComponent);


    }

    private void Update()
    {
        if (manager == null) return;
        if (manager.HasComponent<RenderMesh>(holeEntity) == false) return;
        DrawHole(open);
    }

    public void DrawHole(bool open)
    {
        ParticleSystem.EmissionModule em = psInstance.emission;
        em.enabled = open;


        //Debug.Log("hole " + open);
        Material newMaterial = open ? openHoleMaterial : closedHoleMaterial;
        GetComponent<MeshRenderer>().material = newMaterial;
        manager.SetSharedComponentData<RenderMesh>(holeEntity,
            new RenderMesh() { material = newMaterial, mesh = mesh });
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        holeEntity = entity;
        manager = dstManager;
        manager.AddComponentData(entity, new HoleComponent()
        {
            //activate = false,
            holeEntity = entity,
            spawned = false,
            open = true,
            holeHitCounter = 0,
            hitTag = 0,
            inLevel = inLevel
        }
        );


        //Entity e = conversionSystem.GetPrimaryEntity(robotPrefab);
        //robotEntity = e;
        //Debug.Log("robot e " + robotEntity);

    }




}




