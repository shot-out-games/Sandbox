using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Transforms : MonoBehaviour
{

    [SerializeField] private Vector3 _trans_point;
    [SerializeField] private Vector3 _inv_trans_point;

    [SerializeField] private Vector3 _trans_dir;
    [SerializeField] private Vector3 _inv_trans_dir;


    [SerializeField] private Transform playerScene;
    [SerializeField] private Transform enemyScene;


    [SerializeField] private Vector3 resultVector;
    [SerializeField] private Vector3 parameterVector;


    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;

    [SerializeField] private int type = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        parameterVector = enemyScene.position;

        _trans_point = playerScene.transform.TransformPoint(parameterVector);
        _inv_trans_point = playerScene.transform.InverseTransformPoint(parameterVector);

        _trans_dir = playerScene.transform.TransformDirection(parameterVector);
        _inv_trans_dir = playerScene.transform.InverseTransformDirection(parameterVector);


        if (type == 1)
        {
            player.position = _trans_point;
        }
        if (type == 2)
        {
            player.position = _inv_trans_point;
        }
        if (type == 3)
        {
            //player.position = _trans_dir;
        }
        if (type == 4)
        {
            //player.position = _inv_trans_dir;
        }


        resultVector = player.position;


        Vector3 targetDirection = resultVector;
        targetDirection.Normalize();
        quaternion targetRotation = quaternion.LookRotation(targetDirection, math.up());
        player.rotation = targetRotation;


    }
}
