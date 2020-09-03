using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class NpcData
{
    public float[] position = new float[3];
    public NpcComponent savedNpc;
    public SkillTreeComponent skillTree;
    public LevelCompleteComponent savedLevelComplete;
    public DeadComponent savedDead;
    public WinnerComponent savedWinner;
    public HealthComponent savedHealth;
    //this project only
    public HitsComponent savedHits;


}
