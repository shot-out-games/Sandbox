using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;




public class HoleSystem : SystemBase
{


    protected override void OnUpdate()
    {

        Entities.ForEach(
            (ref HoleComponent holeComponent, in Entity entity) =>
            {
                int counter = holeComponent.holeHitCounter + 1;
                if (holeComponent.hitTag == 1) //fireball - string not supported
                {
                    holeComponent.hitTag = 0;
                    holeComponent.open = !holeComponent.open;
                    holeComponent.holeHitCounter = counter;
                }
            }
        ).Schedule();



    }



}
