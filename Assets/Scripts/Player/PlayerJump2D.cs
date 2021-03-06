using System;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using ForceMode = UnityEngine.ForceMode;



public struct PlayerJumpComponent : IComponentData
{
    public float startJumpGravityForce;
    public float gameStartJumpGravityForce;
    public float jumpFramesToPeak;
    public float addedNegativeForce;
    public float jumpDownGravityMultiplier;
    public float jumpY;
    public float airForce;
    public int jumpPoints;
    public JumpStages JumpStage;
    public float hiJumpMultiplier;
    public bool disabled;
}


public enum JumpStages
{
    Ground,
    JumpStart,
    JumpUp,
    JumpDown,
    JumpEnd
}



namespace SandBox.Player
{

    public class PlayerJump2D : MonoBehaviour, IConvertGameObjectToEntity
    {

        //public JumpStages JumpStage = JumpStages.Ground;

        [HideInInspector] public Camera mainCam;
        [HideInInspector] public float startJumpGravityForce = 9.81f;
        [HideInInspector] public float addedNegativeForce = 0f;
        [HideInInspector] public float jumpDownGravityMultiplier = 1.0f;
        [HideInInspector] public float jumpY = 6f;
        [HideInInspector] public float airForce = 500f;
        [HideInInspector]
        [SerializeField] private bool disabled = false;

        public float jumpFramesToPeak = 5;
        [Range(1, 3)]
        public int jumpPoints;

        [Range(.05f, 5f)]
        public float hiJumpMultiplier = 1;


        Animator anim;
        public AudioClip audioClip;
        public ParticleSystem ps;

        public AudioSource audioSource;



        void Awake()
        {
            anim = GetComponent<Animator>();
            mainCam = Camera.main;
        }




        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (GetComponent<PlayerRatings>())
            {
                startJumpGravityForce = GetComponent<PlayerRatings>().Ratings.startJumpGravityForce;
                addedNegativeForce = GetComponent<PlayerRatings>().Ratings.addedNegativeForce;
                jumpDownGravityMultiplier = GetComponent<PlayerRatings>().Ratings.jumpDownGravityMultiplier;
                jumpY = GetComponent<PlayerRatings>().Ratings.jumpY;
                airForce = GetComponent<PlayerRatings>().Ratings.airForce;
            }


            //float framesToPeakRatio = startJumpGravityForce / jumpFramesToPeak;
            float framesToPeakRatio = jumpFramesToPeak;
            dstManager.AddComponentData
            (
                entity,
                new PlayerJumpComponent
                {
                    startJumpGravityForce = startJumpGravityForce,
                    gameStartJumpGravityForce = startJumpGravityForce,
                    jumpFramesToPeak = framesToPeakRatio,
                    addedNegativeForce = addedNegativeForce,
                    jumpDownGravityMultiplier = jumpDownGravityMultiplier,
                    jumpY = jumpY,
                    airForce = airForce,
                    jumpPoints = jumpPoints,
                    hiJumpMultiplier =  hiJumpMultiplier,
                    disabled = disabled
                }
            );


        }
    }
}

