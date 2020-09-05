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
    public float rotateSlerpDampTime;
}



namespace SandBox.Player
{
    [RequiresEntityConversion]


    public class PlayerMove : MonoBehaviour, IConvertGameObjectToEntity
    {

        public AudioSource audioSource;
        public AudioClip clip;
        public ParticleSystem ps;



        //[SerializeField]
        float rotateSlerpDampTime = 9f;//na

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
        public float slerpDampTime { get; set; }




        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            jumpEnabled = GetComponent<PlayerJump>() || GetComponent<PlayerJumpDots>();
            mainCam = Camera.main;

        }

        void FixedUpdate()
        {

        }


        private void OnAnimatorMove()
        //private void Update()
        {
            if (_entity == Entity.Null) return;


            if (!_entityManager.HasComponent(_entity, typeof(ApplyImpulseComponent))) return;
            //if (!_entityManager.HasComponent(_entity, typeof(PlayerMoveComponent))) return;
            if (!_entityManager.HasComponent(_entity, typeof(RatingsComponent))) return;
            if (!ReInput.isReady) return;





            ApplyImpulseComponent applyImpulseComponent =
                _entityManager.GetComponentData<ApplyImpulseComponent>(_entity);


            //float h = ReInput.players.GetPlayer(0).GetAxis("Move Horizontal");
            float h = applyImpulseComponent.stickX;




            currentSpeed = _entityManager.GetComponentData<RatingsComponent>(_entity).gameSpeed;
            Vector3 velocity = animator.deltaPosition / Time.deltaTime * currentSpeed;


            float size =  math.length(applyImpulseComponent.Velocity);
            //Debug.Log("size " + size);
            size = 1;

            float v = applyImpulseComponent.InJump? negativeForce : 0;
            if (applyImpulseComponent.Falling)
            {
                v = negativeForce;
            }

            if (applyImpulseComponent.BumpLeft == true)
            {
                if (h < 0)
                {
                    //h = applyImpulseComponent.Velocity.x * -1;
                    //v = applyImpulseComponent.Velocity.y * -1;
                    h = -negativeForce * size;
                    v = negativeForce * size;
                    applyImpulseComponent.InJump = false;
                }
            }
            else if (applyImpulseComponent.BumpRight == true)
            {
                if (h > 0)
                {
                    //h = applyImpulseComponent.Velocity.x * -1;
                    //v = applyImpulseComponent.Velocity.y * -1;
                    h = negativeForce * size;
                    v = negativeForce * size;
                    applyImpulseComponent.InJump = false;
                }
            }




            //vy = negativeForce;

            velocity.x = h * currentSpeed;
            applyImpulseComponent.Velocity = new float3(velocity.x, v, velocity.z);            //

            //Debug.Log("v " + applyImpulseComponent.Velocity);
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
            dstManager.AddComponentData(entity, new PlayerMoveComponent() { negativeForce = negativeForce, currentSpeed = startSpeed, rotateSlerpDampTime = rotateSlerpDampTime });

        }
    }
}



