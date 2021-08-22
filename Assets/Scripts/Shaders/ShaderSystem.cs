using SandBox.Player;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


//skinned mesh not compatible currently
//MonoBehaviour Only

public class ShaderSystem : SystemBase
{
    protected override void OnUpdate()
    {

        Entities.WithoutBurst().ForEach( (Entity e, in RenderMesh renderMesh,  in ShaderComponent shaderComponent) =>
        {
            Debug.Log("renderMesh " + renderMesh);
            Debug.Log("renderMesh " + renderMesh.mesh);
            Debug.Log("renderMesh " + renderMesh.material);

        }
        ).Run();


        




    }













}

