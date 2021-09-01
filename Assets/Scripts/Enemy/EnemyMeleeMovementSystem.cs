using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

public class EnemyMeleeMovementSystem : SystemBase
{

    protected override void OnUpdate()
    {

        Entities.WithoutBurst().WithNone<Pause>().WithAll<EnemyComponent>().
            WithAll<EnemyMeleeMovementComponent>().WithAll<EnemyWeaponMovementComponent>().WithStructuralChanges().ForEach
        (
        (

            //Animator animator,
            EnemyMove enemyMove,
            EnemyWeaponAim aim,
            Entity e,
            ref Rotation rotation, ref Translation trans, ref LocalToWorld ltw,
            ref EnemyStateComponent enemyState,
            ref GunComponent gun
            //in EnemyBehaviourComponent enemyBehaviourComponent


        ) =>
        {
            if (HasComponent<DeadComponent>(e) == false) return;
            if (GetComponent<DeadComponent>(e).isDead) return;
            Animator animator = enemyMove.anim;
            //if (enemyMovementComponent.enabled == false) return;
            var defensiveRole = GetComponent<DefensiveStrategyComponent>(e).currentRole;
            var EnemyBasicMovementComponent = GetComponent<EnemyMovementComponent>(e);
            var EnemyMeleeMovementComponent = GetComponent<EnemyMeleeMovementComponent>(e);
            var EnemyWeaponMovementComponent = GetComponent<EnemyWeaponMovementComponent>(e);
            var enemyBehaviourComponent = GetComponent<EnemyBehaviourComponent>(e);

            bool basicMovement = EnemyBasicMovementComponent.enabled;
            bool meleeMovement = EnemyMeleeMovementComponent.enabled;
            bool weaponMovement = EnemyWeaponMovementComponent.enabled;

            enemyMove.speedMultiple = 1;
            MoveStates MoveState = enemyState.MoveState;
            EnemyRoles role = enemyMove.enemyRole;

            if (enemyMove.target != null && enemyMove.enemyRole != EnemyRoles.None)
            {

                Vector3 enemyPosition = animator.transform.position;
                Vector3 homePosition = enemyMove.originalPosition;
                bool stayHome = enemyBehaviourComponent.useDistanceFromStation;
                float dist = Vector3.Distance(enemyMove.target.position, enemyPosition);
                float distFromStation = Vector3.Distance(homePosition, enemyPosition);
                float chaseRange = enemyBehaviourComponent.chaseRange;
                if (weaponMovement)
                {
                    //aim.weaponRaised = false;
                    aim.SetAnimationLayerWeights();
                    aim.weaponRaised = false;
                    gun.IsFiring = 0;
                    if (dist < EnemyWeaponMovementComponent.shootRangeDistance)
                    {
                        aim.weaponRaised = true;
                        gun.IsFiring = 1;
                        MoveState = MoveStates.Idle;//need new state for when shooting then animation movement adjust to this
                        enemyMove.AnimationMovement();
                        enemyMove.FacePlayer();
                    }
                }


                //float backupZoneClose = animator.GetComponent<EnemyMelee>().currentStrikeDistanceZoneBegin;
                float backupZoneClose = EnemyMeleeMovementComponent.combatStrikeDistanceZoneBegin;
                float backupZoneFar = EnemyMeleeMovementComponent.combatStrikeDistanceZoneEnd;

                bool strike = false;
                if (dist < backupZoneClose && meleeMovement)
                {
                    MoveState = MoveStates.Default;
                    enemyMove.backup = true;//only time to turn on 
                    enemyMove.speedMultiple = dist / backupZoneClose;//try zero
                    strike = true;
                }

                if (enemyMove.backup && dist > backupZoneFar && meleeMovement)
                {
                    MoveState = MoveStates.Default;
                    enemyMove.backup = false;//only time to turn off
                }
                else if (dist >= backupZoneClose && dist <= backupZoneFar && meleeMovement)
                {
                    enemyMove.speedMultiple = math.sqrt((dist - backupZoneClose) / (backupZoneFar - backupZoneClose));
                    enemyMove.speedMultiple = 1;
                    MoveState = MoveStates.Default;
                    int n = Random.Range(0, 10);
                    //if (enemyMove.backup == true && n <= 2)
                    //{
                    strike = true;
                    //enemyMove.backup = false;//try

                    //}
                    //else if (enemyMove.backup == false && n <= 5)
                    //{
                    //  strike = true;
                    //}


                    //try below
                    //if (n <= 5)
                    //{
                    //  strike = true;
                    //enemyMove.backup = false;
                    //}

                }


                bool backup = enemyMove.backup;
                if (stayHome && distFromStation > chaseRange) chaseRange = distFromStation;


                if (strike && dist < chaseRange && meleeMovement)
                {
                    MoveState = MoveStates.Default;
                    animator.SetInteger("Zone", 3);
                    //Debug.Log("zone 1 strike move");
                    enemyMove.SetDestination();
                    enemyMove.FacePlayer();

                }
                else if (backup && dist < chaseRange && meleeMovement)
                {
                    MoveState = MoveStates.Default;
                    animator.SetInteger("Zone", 2);
                    enemyMove.SetBackup();
                    enemyMove.FacePlayer();

                }
                else if (dist < EnemyMeleeMovementComponent.combatRangeDistance && dist < chaseRange && meleeMovement)
                {
                    MoveState = MoveStates.Default;
                    //Debug.Log("zone 1 melee move");
                    animator.SetInteger("Zone", 2);
                    enemyMove.SetDestination();
                    enemyMove.FacePlayer();

                }
                else if (dist < chaseRange)
                {
                    if (animator.GetComponent<EnemyMelee>())
                    {
                        animator.GetComponent<EnemyMelee>().currentStrikeDistanceAdjustment = 1; //reset when out of strike range
                    }

                    MoveState = MoveStates.Chase;
                    //Debug.Log("zone 1 melee");
                    animator.SetInteger("Zone", 1);
                    enemyMove.SetDestination();
                    enemyMove.FacePlayer();

                }
                else if (dist >= chaseRange && (role == EnemyRoles.Chase || defensiveRole == DefensiveRoles.Chase))
                {
                    animator.SetInteger("Zone", 1);
                    MoveState = MoveStates.Idle;
                    enemyMove.AnimationMovement();//dont chase just idle animation - sets animation param Z=0
                    enemyMove.FaceWaypoint();

                }
                else if (dist >= chaseRange && role == EnemyRoles.Patrol)
                {
                    animator.SetInteger("Zone", 1);
                    MoveState = MoveStates.Patrol;
                    enemyMove.Patrol();
                    enemyMove.FaceWaypoint();

                }

                enemyState = new EnemyStateComponent() { MoveState = MoveState };

            }
            else if (role == EnemyRoles.Patrol)
            {
                animator.SetInteger("Zone", 1);
                MoveState = MoveStates.Patrol;
                enemyMove.Patrol();
                enemyMove.FaceWaypoint();

            }

            //ltw = new LocalToWorld
            //{
            //  Value = float4x4.TRS(enemyMove.transform.position, enemyMove.transform.rotation, Vector3.one)
            //};
            rotation.Value = enemyMove.transform.rotation;
            trans.Value = enemyMove.transform.position;
            //trans.Value = trans.Value + ltw.Forward;//start ray out before pointing down because we are checking the ground a little bit in front
            //Debug.Log("ltw fwd " + ltw.Forward);


        }
        ).Run();

    }

}






