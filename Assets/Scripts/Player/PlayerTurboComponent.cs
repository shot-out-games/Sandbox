using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]

public struct PlayerTurboComponent : IComponentData
{
    public float multiplier;

}
