﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapConnection : MonoBehaviour {
    public GameObject doorObject;
    public GameObject wallObject;
    public string category;

    // TODO: Draw an arrow from the editor

    public void UpdateDoorState(bool isDoor)
    {
        if (doorObject != null)
        {
            doorObject.SetActive(isDoor);
        }

        if (wallObject != null)
        {
            wallObject.SetActive(!isDoor);
        }
    }

    void OnDrawGizmos()
    {
        if (transform != null) {
            Gizmos.color = new Color(1, 1, 0, 0.75F);
            var start = transform.position;
            var end = start + transform.forward;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, end);
        }
        
    }
}
