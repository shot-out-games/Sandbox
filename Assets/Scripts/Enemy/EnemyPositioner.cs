﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorClass
{
    public GameObject actorPrefab;
    public Transform actorTransform;
}



public class EnemyPositioner : MonoBehaviour
{
    [SerializeField]
    private ActorClass[] enemies;
    //[SerializeField]
    //GameObject enemyPrefab;

    // Start is called before the first frame update
    //[SerializeField] private Transform childTriggers;


    void Awake()
    {
        if (enemies.Length == 0) return;
        // if (childTriggers != null)
        //  childTriggers.parent = null;

        GameObject defaultPrefab = enemies[0].actorPrefab;

        //transform.position = startLocation.position;
        foreach (var enemy in enemies)
        {
            GameObject prefabGameObject = enemy.actorPrefab;
            if (prefabGameObject == null) prefabGameObject = defaultPrefab;
            GameObject go = Instantiate(prefabGameObject, enemy.actorTransform.position, enemy.actorTransform.rotation);

        }

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("awake " + " pos " + transform.position);

    }
}
