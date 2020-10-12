using Rewired;
using RootMotion.FinalIK;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public struct WeaponInteractionComponent : IComponentData
{
    public int weaponType;
    public bool canPickup;
}

public struct CharacterInteractionComponent : IComponentData
{

}



public class WeaponInteraction : MonoBehaviour, IConvertGameObjectToEntity
{
    public Entity e;
    private EntityManager manager;

    [SerializeField] private InteractionSystem interactionSystem;
//    [HideInInspector]
    public InteractionObject interactionObject;//set from  picked up weaponitem

    public bool interactKeyPressed;
    [SerializeField]
    private bool inputRequired;

    
    public bool canPickup;

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

        //var weaponItem = manager.GetComponentData<WeaponItemComponent>(e);
        //weaponItem.pickedUp = true;
        //manager.SetComponentData(e, weaponItem);

        interactionSystem.ik.enabled = false;
        manager.RemoveComponent<CharacterInteractionComponent>(e);

    }

    private void LateUpdate()
    {
        interactionSystem.ik.solver.Update();
    }


    public void UpdateSystem()
    {
        if ((interactKeyPressed || inputRequired == false) && interactionSystem.inInteraction == false && interactionObject != null)
        {
            Debug.Log("interaction ");
            //var weaponItem = manager.GetComponentData<WeaponItemComponent>(e);
            //if (weaponItem.pickedUp == false)
            //{
                interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, interactionObject, true);
                interactionSystem.ik.enabled = true;
            //}

        }

    }


    //private void UpdateWIP()
    //{

    //    var closestTriggerIndex = interactionSystem.GetClosestTriggerIndex();
    //    if (closestTriggerIndex <= -1) return;
    //    if ((interactKeyPressed || inputRequired == false) && interactionSystem.inInteraction == false)
    //    {
    //        var io = interactionSystem.GetClosestInteractionObjectInRange();
    //        var go = io.gameObject;
    //        if (go.GetComponent<WeaponItem>() == true)
    //        {
    //            audioSource.PlayOneShot(clip);

    //            //add weapon type picked up to check against elegible weapon list (attachedweapon script)
    //            var item_e = go.GetComponent<WeaponItem>().e;

    //            int weaponType = manager.GetComponentData<WeaponItemComponent>(item_e).weaponType; //1 is gun
    //            WeaponItemComponent weaponItem = manager.GetComponentData<WeaponItemComponent>(item_e);
    //            weaponItem.active = false;

    //            manager.SetComponentData(e, new WeaponInteractionComponent { weaponType = weaponType });
    //            manager.SetComponentData(item_e, weaponItem);


    //            go.SetActive(false);
    //            //manager.DestroyEntity(item_e);//destroy item entity - system when pick up on deletes GO already

    //            //look at weapons to see if they have as available same one picked up here and if so activate
    //            //system instead but then runs every frame
    //            if (GetComponent<WeaponManager>())
    //            {
    //                var potentialWeapons = GetComponent<WeaponManager>().weaponsList.Count;
    //                for (var i = 0; i < potentialWeapons; i++)
    //                    if ((int)GetComponent<WeaponManager>().weaponsList[i].weaponType == weaponType)
    //                    {
    //                        GetComponent<WeaponManager>().AttachPickWeapons(i);
    //                    }
    //            }
    //        }
    //        interactionSystem.ik.enabled = true;
    //        manager.AddComponentData<CharacterInteractionComponent>(e, new CharacterInteractionComponent());
    //        interactionSystem.TriggerInteraction(closestTriggerIndex, true);
    //    }
    //}

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        manager.AddComponentData<WeaponInteractionComponent>(e,
            new WeaponInteractionComponent { weaponType = 0, canPickup = canPickup }); //0 is none

    }



    void OnDestroy()
    {
        if (interactionSystem == null) return;
        interactionSystem.OnInteractionStop -= OnStop;
    }
}


public class PowerInteractionSystem : SystemBase
{

    protected override void OnUpdate()
    {
        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (
                WeaponInteraction weaponInteraction,
                InputController inputController

            ) =>
            {
                weaponInteraction.interactKeyPressed = inputController.buttonB_Pressed;
            }

        ).Run();

        //Entities.WithoutBurst().ForEach
        //(
        //    ( 
        //        WeaponItemComponent weaponItem

        //    ) =>
        //    {


        //    }

        //).Run();


    }
}
