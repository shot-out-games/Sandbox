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
                ref RatingsComponent playerRatings,
                in PlayerMoveComponent playerMoveComponent,
                in InputController inputController,
                in PlayerTurboComponent playerTurbo
            ) =>
            {
                bool button = inputController.buttonY_held;
                bool button_released = inputController.buttonY_Released;

                float currentSpeed = playerRatings.speed;


                if (button)
                {
                    //playerMoveComponent.
                        
                    //currentSpeed = playerRatings.speed * playerTurbo.multiplier;
                    playerRatings.gameSpeed = playerRatings.speed * playerTurbo.multiplier;

                }
                else if (button_released)
                {
                    Debug.Log("turbo");
                    playerRatings.gameSpeed = playerRatings.speed;
                }


            }
        ).Run();

    }
}

