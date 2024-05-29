
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePiece : MonoBehaviour
{
    Color colorOG;

    int turn = 0;

    // Start is called before the first frame update
    void Start()
    {
        colorOG = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void IncrementTurn()
    {
        turn++;

        if(turn == 4)
        {
            GetComponent<MeshRenderer>().material.color = colorOG;
            Destroy(this);
        }
    }
}
