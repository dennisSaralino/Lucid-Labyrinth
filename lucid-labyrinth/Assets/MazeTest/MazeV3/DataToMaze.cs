using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataToMaze : MonoBehaviour
{
    public static DataToMaze i;
    public Vector3 startPos;
    public void Awake()
    {
        if (i == null) i = this;
        else Destroy(this.gameObject);
        startPos = new Vector3(-1000, -1000, -1000);
        transform.position = startPos;
    }


    public void dataToMaze(mazeTile[,] data)
    {
        
    }
}
