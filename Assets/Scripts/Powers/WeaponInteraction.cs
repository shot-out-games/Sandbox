using Rewired;
using RootMotion.FinalIK;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public struct WeaponInteractionComponent : IComponentData
{
    public int weaponType;
}

public struct CharacterInteractionComponent : IComponentData
{

}



public class WeaponInteraction : MonoBehaviour, IConvertGameObjectToEntity
{
    private Entity e;
    private EntityManager manager;

    [SerializeField] private InteractionSystem interactionSystem;

    public bool interactKeyPressed;
    [SerializeField]
    private bool inputRequired;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip clip;

    void Start()
    {
        // Listen to interaction events
        //    interactionSystem.OnInteractionStart += OnStart;
        // interactionSystem.OnInteractionPause += OnPause;
        //interactionSystem.OnInteractionResume += OnDrop;
        interactionSystem.OnInteractionStop += OnStop;
        audioSource = GetComponent<AudioSource>();


    }

    private void OnStop(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
    {


        interactionSystem.ik.enabled = false;

        manager.RemoveComponent<CharacterInteractionComponent>(e);


    }

    private void LateUpdate()
    {
        interactionSystem.ik.solver.Update();
    }



    private void Update()
    {

        var closestTriggerIndex = interactionSystem.GetClosestTriggerIndex();
        if (closestTriggerIndex <= -1) return;
        if ((interactKeyPressed || inputRequired == false) && interactionSystem.inInteraction == false)
        {
            var io = interactionSystem.GetClosestInteractionObjectInRange();
            var go = io.gameObject;
            if (go.GetComponent<WeaponItem>() == true)
            {
                audioSource.PlayOneShot(clip);

                //add weapon type picked up to check against elegible weapon list (attachedweapon script)
                var item_e = go.GetComponent<WeaponItem>().e;
                int weaponType = manager.GetComponentData<WeaponItemComponent>(item_e).weaponType; //1 is gun
                WeaponItemComponent weaponItem = manager.GetComponentData<WeaponItemComponent>(item_e);
                weaponItem.active = false;

                manager.SetComponentData(e, new WeaponInteractionComponent { weaponType = weaponType });
                manager.SetComponentData(item_e, weaponItem);


                go.SetActive(false);
                //manager.DestroyEntity(item_e);//destroy item entity - system when pick up on deletes GO already

                //look at weapons to see if they have as available same one picked up here and if so activate
                //system instead but then runs every frame
                if (GetComponent<AttachWeapon>())
                {
                    var potentialWeapons = GetComponent<AttachWeapon>().weaponsList.Count;
                    for (var i = 0; i < potentialWeapons; i++)
                        if ((int)GetComponent<AttachWeapon>().weaponsList[i].weaponType == weaponType)
                        {
                            GetComponent<AttachWeapon>().AttachPickWeapons(i);
                        }
                }
            }
            interactionSystem.ik.enabled = true;
            manager.AddComponentData<CharacterInteractionComponent>(e, new CharacterInteractionComponent());
            interactionSystem.TriggerInteraction(closestTriggerIndex, true);
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        manager.AddComponentData<WeaponInteractionComponent>(e,
            new WeaponInteractionComponent { weaponType = 0 }); //0 is none

    }



    void OnDestroy()
    {
        if (interactionSystem == null) return;
        interactionSystem.OnInteractionStop -= OnStop;
    }
}


public class PowerInteractionSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (
                WeaponInteraction weaponInteraction,
                in InputController inputController

            ) =>
            {
                weaponInteraction.interactKeyPressed = inputController.buttonA_Pressed;
            }

        ).Run();

        return default;
    }
}
