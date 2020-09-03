using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;


public class StatsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach
        (
            (
                ref StatsComponent statsComponent,
                ref SkillTreeComponent skillTree
            ) =>
            {
                if (skillTree.CurrentLevelXp >= skillTree.PointsNextLevel)
                {
                    skillTree.CurrentLevelXp = 0;
                    skillTree.CurrentLevel += 1;
                    skillTree.availablePoints += 1;
                }
            }
        ).Run();

    }

}


