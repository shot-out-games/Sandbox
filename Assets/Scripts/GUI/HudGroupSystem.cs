using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class HudGroupSystem : SystemBase
{
    protected override void OnUpdate()
    {


        int level = 0;


        Entities.WithAll<PlayerComponent>().WithoutBurst().WithStructuralChanges().ForEach((Entity entity, SkillTreeComponent skillTree
        ) =>
        {
            level = skillTree.CurrentLevel;

        }).Run();




        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity,

            HudGroup hudGroup

        ) =>
        {

            string levelLabel = "Level ";
            hudGroup.ShowLabelLevelTargets(levelLabel, level);


        }).Run();



    }
}
