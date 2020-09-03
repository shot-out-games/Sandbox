using Unity.Entities;
using UnityEngine;
using Unity.Jobs;

//[UpdateBefore(typeof(UnityEngine.Experimental.PlayerLoop.PreLateUpdate))]
namespace SandBox.Player
{

    public class PlayerCombatSystem : JobComponentSystem
    {


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            Entities.WithoutBurst().ForEach(
                (
                    InputController inputController,
                    Animator animator,
                    Rigidbody rb,
                    PlayerMove playerMove,
                    PlayerCombat playerCombat) =>
                {

                    bool button_b = inputController.buttonB_Pressed;
                    bool button_y = inputController.buttonY_Pressed;




                    if (button_y)
                    {
                        playerCombat.SelectMove(1);
                        playerCombat.StartMove(1);
                    }
                    else if (button_b)
                    {
                        playerCombat.SelectMove(2);
                        playerCombat.StartMove(2);
                    }


                }
            ).Run();

            return default;

        }



    }
}


