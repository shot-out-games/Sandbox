using RootMotion.FinalIK;
using UnityEngine;


[System.Serializable]

public class Moves
{
    public bool playerMove;
    public bool enemyMove;
    public Transform target = null;
    public Transform pin = null;
    public float weight;
    public FullBodyBipedEffector effector;
    [HideInInspector]
    public float calculatedStrikeDistanceZoneBegin;
    public bool usingFbb = true;
    public bool usingAim = true;
    public AimIK aimIk;
    public bool active = true;
    public Transform aimTransform = null;


    public float strikeDistanceAdjustment
    { get; set; } = 1.0f;

    public void CalculateStrikeDistanceFromPinPosition(Transform _transform)
    {
        float offset = .23f;
        calculatedStrikeDistanceZoneBegin = Vector3.Distance(_transform.position, pin.position) -  offset;//.25 
    }

}