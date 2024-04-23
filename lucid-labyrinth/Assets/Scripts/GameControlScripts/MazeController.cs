using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{
    public GameObject lucidityPickup;
    public GameObject key;
    public Transform pickUpHolder;



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
        StartCoroutine(waitForMazeData());
    }
    IEnumerator waitForMazeData()
    {
        while (mazeData == null) yield return null;
        while (!mazeData.isReady) yield return null;
        Vector3 offset = new Vector3(0, 1, 0);
        foreach (Vector3 i in mazeData.lucidityPickupPos)
        {
            GameObject g = Instantiate(lucidityPickup, pickUpHolder);
            g.transform.position = i + offset;
        }
        foreach (Vector3 i in mazeData.keyPickupPos)
        {
            GameObject g = Instantiate(key, pickUpHolder);
            g.transform.position = i + offset;
        }

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
