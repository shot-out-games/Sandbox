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
}



namespace SandBox.Player
{
    [RequiresEntityConversion]


    public class PlayerMove : MonoBehaviour, IConvertGameObjectToEntity
    {

        public AudioSource audioSource;
        public AudioClip clip;
        public ParticleSystem ps;



        [SerializeField]
        float rotateSpeed = 9;
        //[SerializeField]
        //private float desiredRotationAngle = ;

        [HideInInspector]
        public Camera mainCam;

        public float startSpeed = 4f;
        public float currentSpeed = 4f;
        private EntityManager _entityManager;
        private Entity _entity;

        [SerializeField]
        float negativeForce = -9.81f;
        private Animator animator;
        //private Rigidbody rigidbody;
        private bool jumpEnabled;

        //public Quaternion rotation { get; set; }
        //public float slerpDampTime { get; set; }
        public int targetFrameRate = -1;
        //public bool threeD;



        // Start is called before the first frame update
        void Start()
        {
            if (targetFrameRate >= 10)
            {
                Application.targetFrameRate = targetFrameRate;
            }

            animator = GetComponent<Animator>();
            jumpEnabled = GetComponent<PlayerJump>() || GetComponent<PlayerJump2D>();
            mainCam = Camera.main;

        }

        //if (!_entityManager.HasComponent(_entity, typeof(ApplyImpulseComponent))) return;
        //if (!_entityManager.HasComponent(_entity, typeof(PlayerMoveComponent))) return;

        //ApplyImpulseComponent applyImpulseComponent =
        //    _entityManager.GetComponentData<ApplyImpulseComponent>(_entity);
        //currentSpeed = _entityManager.GetComponentData<PlayerMoveComponent>(_entity).currentSpeed;
        //Vector3 velocity = animator.deltaPosition / Time.deltaTime * currentSpeed;
        ////applyImpulseComponent.Velocity = new float3(velocity.x, velocity.y, velocity.z);
        //applyImpulseComponent.Velocity = new float3(velocity.x, 0, velocity.z);

        //_entityManager.SetComponentData(_entity, applyImpulseComponent);
        //if (!jumpEnabled)

        //{
        //    animator.SetBool("Grounded", true);
        //}


        //private void OnAnimatorMove()
        private void Update()
        {
            if (_entity == Entity.Null) return;
            //changing so we need root animation here only

            if (!_entityManager.HasComponent(_entity, typeof(ApplyImpulseComponent))) return;
            if (!_entityManager.HasComponent(_entity, typeof(RatingsComponent))) return;
            if (!_entityManager.HasComponent(_entity, typeof(PlayerComponent))) return;
            if (!ReInput.isReady) return;

            //ApplyImpulseComponent applyImpulseComponent =
            //    _entityManager.GetComponentData<ApplyImpulseComponent>(_entity);



            ////_entityManager.SetComponentData(_entity, applyImpulseComponent);


            //bool grounded = applyImpulseComponent.Grounded;


            ////if (!jumpEnabled && animator != null)

            //if (animator != null)
            //{
            //    //Debug.Log("v " + velocity);
            //    animator.SetBool("Grounded", grounded);
            //}


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
                negativeForce = negativeForce, currentSpeed = startSpeed, rotateSpeed = rotateSpeed,
                

            });

        }
    }
}



