﻿using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]


public struct RatingsComponent : IComponentData
{
    public int tag;
    public float speed;
    public float power;
    public float maxHealth;
    public float shootRangeDistance;
    public float chaseRangeDistance;
    public float combatRangeDistance;


}


