using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class EnemyData
{
    public float[] position = new float[3];
    public SkillTreeComponent skillTree;
    public EnemyComponent savedEnemy;
    public LevelCompleteComponent savedLevelComplete;
    public DeadComponent savedDead;
    public WinnerComponent savedWinner;
    public HealthComponent savedHealth;


}
