using SandBox.Player;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(PlayerComponent))]
[UpdateAfter(typeof(PlayerMoveSystem))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


public class PlayerTurboSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (
                ref PlayerMoveComponent playerMoveComponent,
                in RatingsComponent playerRatings,
                in InputController inputController,
                in PlayerTurboComponent playerTurbo
            ) =>
            {
                bool button_a = inputController.buttonA_held;
                bool button_a_released = inputController.buttonA_Released;

                float currentSpeed = playerRatings.speed;


                if (button_a)
                {
                    playerMoveComponent.currentSpeed = playerRatings.speed * playerTurbo.multiplier;
                }
                else if (button_a_released)
                {
                    playerMoveComponent.currentSpeed = currentSpeed;
                }


                }
        ).Run();

        return default;
    }
}

