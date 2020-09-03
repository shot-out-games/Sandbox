using UnityEngine;
using Unity.Entities;
using Unity.Jobs;


public struct PlayerStopComponent : IComponentData
{
    public bool enabled;

}


public class PlayerStop : MonoBehaviour, IConvertGameObjectToEntity
{


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<PlayerStopComponent>(entity, new PlayerStopComponent() { enabled = false });

    }
}


[RequireComponent(typeof(PlayerComponent))]

public class PlayerStopSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (
                ref PlayerMoveComponent playerMoveComponent,
                ref PlayerStopComponent playerStopComponent,
                in RatingsComponent playerRatings,
                in InputController inputController,
                in Animator animator
            ) =>
            {
                float lb = inputController.leftBumperValue;
                float rb = inputController.rightBumperValue;



                float currentSpeed = playerRatings.speed;//need this if turbo activated first

                if (lb > 0 || rb > 0)
                {
                    playerMoveComponent.currentSpeed = 0;
                    playerStopComponent.enabled = true;
                }
                else
                {
                    playerMoveComponent.currentSpeed = currentSpeed;
                    playerStopComponent.enabled = false;
                }


            }
        ).Run();

        return default;
    }
}

