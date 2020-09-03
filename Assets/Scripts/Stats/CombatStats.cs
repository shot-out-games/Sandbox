using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatStatsList
{
    public float hpLanded { set; get; }
    public float hpReceived { set; get; }
    public bool missed { set; get; }

}


public class CombatStats
{
    public List<CombatStatsList> combatStatsList = new List<CombatStatsList>();

    public void AddStats(float _hpLanded, float _hpReceived, bool _missed)
    {

    }

}
