using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataToMaze : MonoBehaviour
{
    public static DataToMaze i;
    public Vector3 startPos;
    public Dictionary<string, GameObject> tileDict;
    public void Awake()
    {
        if (i == null) i = this;
        else Destroy(this.gameObject);
        startPos = new Vector3(-1000, -1000, -1000);
        transform.position = startPos;
        tileDict = new Dictionary<string, GameObject>();
        string path = "GameObject/Tile/";

        tileDict["wall"] = Resources.Load<GameObject>(path + "Wall");

        tileDict["floor"] = Resources.Load<GameObject>(path + "Floor");

        tileDict["path"] = Resources.Load<GameObject>(path + "Path");

        tileDict["door"] = Resources.Load<GameObject>(path + "Door");

        tileDict["upStair"] = Resources.Load<GameObject>(path + "upStair");

        tileDict["downStair"] = Resources.Load<GameObject>(path + "downStair");
    }


    public void dataToMaze(TileData[,] data)
    {
        StartCoroutine(dataToMazeI(data));   
    }
    IEnumerator dataToMazeI(TileData[,] data)
    {
        StaticTool.destroyChildOb(transform);
        yield return null;
        Transform prefab = new GameObject("Cell").transform;
        Vector3 tileSize = new Vector3(12, 0, 12);
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                TileData currentData = data[j, i];
                if (currentData == null) continue;
                Transform p = Instantiate(prefab, transform);
                p.localPosition = new Vector3(tileSize.x * j, 0, tileSize.z * i);
                //Debug.Log(currentData == null);
                currentData.loadInto(p);

            }
            yield return null;
        }

    }
}
