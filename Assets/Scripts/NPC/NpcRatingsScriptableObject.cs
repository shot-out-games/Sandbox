using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Npc1", menuName = "NPC Ratings")]
public class NpcRatingsScriptableObject : ScriptableObject
{
    public float speed = 3.5f;
    public float combatStrikeDistanceZoneBegin = 1.8f;
    public float combatStrikeDistanceZoneEnd = 3.6f;
    public float combatRangeDistance = 6.4f;
    public float chaseRange = 18.4f;
    public float aggression = 50;
    public float maxHealth = 100;


}



