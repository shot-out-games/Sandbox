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


        private void OnAnimatorMove()
        {
            if (_entity == Entity.Null) return;
            //changing so we need root animation here only

            if (!_entityManager.HasComponent(_entity, typeof(ApplyImpulseComponent))) return;
            if (!_entityManager.HasComponent(_entity, typeof(RatingsComponent))) return;
            if (!_entityManager.HasComponent(_entity, typeof(PlayerComponent))) return;
            if (!ReInput.isReady) return;

            bool threeD = _entityManager.GetComponentData<PlayerComponent>(_entity).threeD;


            ApplyImpulseComponent applyImpulseComponent =
                _entityManager.GetComponentData<ApplyImpulseComponent>(_entity);


            float h = applyImpulseComponent.stickX;
            float stickY = applyImpulseComponent.stickY;

            currentSpeed = _entityManager.GetComponentData<RatingsComponent>(_entity).gameSpeed;
            Vector3 velocity = animator.deltaPosition / Time.deltaTime * currentSpeed;


            //float size = math.length(applyImpulseComponent.Velocity);
            //size = 1;


            float v = applyImpulseComponent.InJump ? negativeForce : 0;
            if (applyImpulseComponent.Falling || applyImpulseComponent.fallingFramesCounter > 1)
            {
                v = negativeForce;
            }

            //if (applyImpulseComponent.BumpLeft == true)
            //{
            //    if (h < 0)
            //    {
            //        h = -negativeForce * size;
            //        v = negativeForce * size;
            //        applyImpulseComponent.InJump = false;
            //    }
            //}
            //else if (applyImpulseComponent.BumpRight == true)
            //{
            //    if (h > 0)
            //    {
            //        h = negativeForce * size;
            //        v = negativeForce * size;
            //        applyImpulseComponent.InJump = false;
            //    }
            //}




            //vy = negativeForce;

            //velocity.x = h * currentSpeed;
            if (threeD)
            {
                //velocity.z = stickY * currentSpeed;
            }

            //velocity.y = v;
            //velocity.x = 0;

            applyImpulseComponent.Velocity = new float3(velocity.x, 0, velocity.z);


            applyImpulseComponent.Velocity = velocity;



            //Vector3 RIGHT = transform.TransformDirection(Vector3.right);//transform right
            //Vector3 FORWARD = transform.TransformDirection(Vector3.forward);


            

            //applyImpulseComponent.Velocity = new float3(0 , v, 0);            //
            //applyImpulseComponent.Velocity = new float3(velocity.x, v, velocity.z);            //

            //Debug.Log("velocity " + applyImpulseComponent.Velocity);
            _entityManager.SetComponentData(_entity, applyImpulseComponent);


            bool grounded = applyImpulseComponent.Grounded;


            //if (!jumpEnabled && animator != null)

            if (animator != null)
            {
                //Debug.Log("v " + velocity);
                animator.SetBool("Grounded", grounded);
            }


        }

        void LateUpdate()
        {
            if (Terrain.activeTerrain != null && !jumpEnabled)
            {
                Vector3 newpos = transform.position;
                newpos.y = Terrain.activeTerrain.SampleHeight(transform.position);
                transform.position = newpos;
            }

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



