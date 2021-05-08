using SandBox.Player;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(PlayerComponent))]


public class PlayerTurboSystem : SystemBase
{

    protected override void OnUpdate()
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

    }
}

