using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;

//[UpdateBefore(typeof(UnityEngine.Experimental.PlayerLoop.PreLateUpdate))]
namespace SandBox.Player
{

    public class PlayerCombatSystem : SystemBase
    {


        protected override void OnUpdate()
        {

            Entities.WithoutBurst().ForEach(
                (
                    PlayerCombat playerCombat,
                    Animator animator,
                    in InputControllerComponent inputController,
                    in ApplyImpulseComponent applyImpulse

                ) =>
                {

                    bool buttonB = inputController.buttonB_Pressed;//kick types
                    bool buttonY = inputController.buttonY_Pressed;//punch types

                    bool allowKick = buttonY == true && (math.abs(animator.GetFloat("Vertical")) < .1 || applyImpulse.Grounded == false) ;



                    if (buttonB)//punch
                    {
                        playerCombat.SelectMove(1);
                    }
                    else if (allowKick)//kick
                    {
                        //Debug.Log("allow kick");
                        playerCombat.SelectMove(2);
                    }


                }
            ).Run();


        }



    }
}


