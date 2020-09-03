using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct MatchupComponent : IComponentData
{
    public bool matchupClosest;
    public bool leader;


}
