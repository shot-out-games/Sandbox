using System;
using System.Xml.Linq;
using Rewired;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using SandBox.Player;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;


public struct PlayerMoveComponent : IComponentData
{
    public float currentSpeed;
    public float negativeForce;
    public float rotateSpeed;
    public bool snapRotation;
}



namespace SandBox.Player
{
    

    public class PlayerMove : MonoBehaviour, IConvertGameObjectToEntity
    {

        public AudioSource audioSource;
        public AudioClip clip;
        public ParticleSystem ps;



        [SerializeField]
        float rotateSpeed = 9;
        [SerializeField]
        bool snapRotation = true;

        [HideInInspector]
        public Camera mainCam;

        public float startSpeed = 4f;
        public float currentSpeed = 4f;
        private EntityManager _entityManager;
        private Entity _entity;

        //[SerializeField]
        //float negativeForce = -9.81f;
        private Animator animator;
        public int targetFrameRate = -1;

        void Start()
        {
            if (targetFrameRate >= 10)
            {
                Application.targetFrameRate = targetFrameRate;
            }

            animator = GetComponent<Animator>();
            mainCam = Camera.main;

        }



        //private void OnAnimatorMove()
        private void Update()
        {
            if (_entity == Entity.Null) return;
            //changing so we need root animation here only
            if (!ReInput.isReady) return;


        }

        void LateUpdate()
        {
            //if (Terrain.activeTerrain != null && !jumpEnabled)
            //{
            //    Vector3 newpos = transform.position;
            //    newpos.y = Terrain.activeTerrain.SampleHeight(transform.position);
            //    transform.position = newpos;
            //}

        }


        public void FacePlayer(Transform target = null, float rotateSpeed = .01f)
        {
            Vector3 lookDir = target.position - transform.position;
            lookDir.y = 0;
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

        }



        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            _entity = entity;
            _entityManager = dstManager;

            startSpeed = GetComponent<PlayerRatings>() ? GetComponent<PlayerRatings>().Ratings.speed : 4f;
            dstManager.AddComponentData(entity, new PlayerMoveComponent()
            {
                //negativeForce = negativeForce,
                currentSpeed = startSpeed, rotateSpeed = rotateSpeed,
                snapRotation = snapRotation
                

            });

        }
    }
}



