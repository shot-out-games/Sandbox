using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesManager : MonoBehaviour
{
    public List<Moves> Moves = new List<Moves>();

    private void Start()
    {
        //Debug.Log("moves " + Moves.Count +  " " + transform.parent.name);

        for (int i = 0; i < Moves.Count; i++)
        {
            Moves move = Moves[i];
            //Debug.Log("move " + move.animationType);
        }
        //SelectMove();

    }



}
