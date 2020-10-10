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
    //public float calculatedStrikeDistanceZoneBegin;
    public bool usingFbb = true;
    public bool usingAim = true;
    public bool usingLimb = true;
    public AimIK aimIk;
    public FABRIK limbIk;
    public bool active = true;
    public Transform aimTransform = null;
    public AudioSource moveAudioSource;
    public AudioClip moveAudioClip;
    public ParticleSystem moveParticleSystem;



    //   { get; set; } = 1.0f;

    public float CalculateStrikeDistanceFromPinPosition(Transform _transform)
    {
        if (pin == null) return -1;
        
        float offset = .23f;
        float calculatedStrikeDistanceZoneBegin = Vector3.Distance(_transform.position, pin.position) - offset;//.25 
        //Debug.Log("strike start " + calculatedStrikeDistanceZoneBegin);
        return calculatedStrikeDistanceZoneBegin;

    }

}