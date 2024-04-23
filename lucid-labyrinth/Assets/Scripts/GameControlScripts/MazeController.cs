using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{
    public mazeData mazeData;
    public PlayerController player;
    public static MazeController i;
    public bool inTest;
    public void Awake()
    {
        if (i == null) i = this;
        else Destroy(this.gameObject);
        if(!inTest)
            player = GameObject.Find("PlayerPrefab").GetComponent<PlayerController>();
    }
    public void spawnPickup()
    {
        
    }
    public void FixedUpdate()
    {
        if(!inTest)
            mazeData.checkCurrentPos(player);
    }
}
