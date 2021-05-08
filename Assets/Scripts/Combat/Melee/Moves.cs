using RootMotion.FinalIK;
using UnityEngine;

public enum AnimationType
{
    None,
    Punch,
    Kick,
    Swing,
    Aim,
    Locomotion,
    Lowering
}

[System.Serializable]
public class Moves
{
    [Header("IK")]
    public bool usingFbb = true;
    public bool usingAim = true;
    public bool usingLimb = true;
    public FullBodyBipedEffector effector;
    public AimIK aimIk;
    public FABRIK limbIk;

    [Header("TARGETING")]
    public AnimationType animationType;
    public Transform target = null;
    public Transform pin = null;
    public float weight;
    public Transform aimTransform = null;


    [Header("EFFECTS")]
    public AudioSource moveAudioSource;
    public AudioClip moveAudioClip;
    public ParticleSystem moveParticleSystem;

    public float CalculateStrikeDistanceFromPinPosition(Transform _transform)
    {
        if (pin == null) return -1;
        
        float offset = .23f;
        float calculatedStrikeDistanceZoneBegin = Vector3.Distance(_transform.position, pin.position) - offset;//.25 
        //Debug.Log("strike start " + calculatedStrikeDistanceZoneBegin);
        return calculatedStrikeDistanceZoneBegin;

    }

}