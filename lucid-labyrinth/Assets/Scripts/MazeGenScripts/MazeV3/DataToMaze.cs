using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using System.Linq;
public class DataToMaze : MonoBehaviour
{
    public static DataToMaze i;
    public Vector3 startPos;
    public Dictionary<string, GameObject> tileDict;

    public List<GameObject> wallDecoration;
    public List<GameObject> floorDecoration;
    public void Awake()
    {
        if (i == null) i = this;
        else Destroy(this.gameObject);
        startPos = new Vector3(-1000, -1000, -1000);
        transform.position = startPos;
        tileDict = new Dictionary<string, GameObject>();
        string path = "GameObject/Tile/";
        string path2 = "GameObject/CellTile/";
        string path3 = "GameObject/";

        tileDict["wall"] = Resources.Load<GameObject>(path + "Wall");

        tileDict["floor"] = Resources.Load<GameObject>(path + "Floor");

        tileDict["path"] = Resources.Load<GameObject>(path + "Path");

        tileDict["door"] = Resources.Load<GameObject>(path + "Door");

        tileDict["upStair"] = Resources.Load<GameObject>(path + "upStair");

        tileDict["downStair"] = Resources.Load<GameObject>(path + "downStair");

        tileDict["startTile"] = Resources.Load<GameObject>(path2 + "Start Tile");
        tileDict["endTile"] = Resources.Load<GameObject>(path2 + "End Tile");


        wallDecoration = Resources.LoadAll<GameObject>(path3 + "WallDeco").ToList();
        floorDecoration = Resources.LoadAll<GameObject>(path3 + "FloorDeco").ToList();
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
        List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                TileData currentData = data[i, j];
                if (currentData == null) continue;
                Transform p = Instantiate(prefab, transform);
                p.localPosition = new Vector3(tileSize.x * i, 0, tileSize.z * j);
                //Debug.Log(currentData == null);
                surfaces.Add(currentData.loadInto(p));

            }
            yield return null;
        }
        navigationBaker.baker.bakeMap(surfaces);

    }
}
