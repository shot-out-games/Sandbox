using Unity.Entities;
using UnityEngine;
using SandBox.Player;

public class RunSystems : MonoBehaviour
{
    private EntityManager entityManager;

    private  PlayerJumpSystem2D playerJump;

    private void Start()
    {

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (entityManager == default) return;

        playerJump.World.GetOrCreateSystem<PlayerJumpSystem2D>();
        //playerJump.Update();

    }


    private void FixedUpdate()
    {
        if (entityManager == default) return;
        //playerJump.Update();
    }
}
