using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPositioner : MonoBehaviour
{
    [SerializeField]
    private Transform startLocation;
    // Start is called before the first frame update
    //[SerializeField] private Transform childTriggers;


    void Awake()
    {
       // if (childTriggers != null)
          //  childTriggers.parent = null;

        transform.position = startLocation.position;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("awake " + " pos " + transform.position);

    }
}
