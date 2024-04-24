using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
public class DataToMaze : MonoBehaviour
{
    public static DataToMaze i;
    public Vector3 startPos;
    public Dictionary<string, GameObject> tileDict;
    public bool materialDebug;
    public List<GameObject> wallDecoration;
    public List<GameObject> floorDecoration;
    public List<TileData> tileFullGrid;
    public void Awake()
    {
        if (i == null) i = this;
        else Destroy(this.gameObject);
        startPos = new Vector3(0, 0, 0);
        transform.position = startPos;
        tileDict = new Dictionary<string, GameObject>();
        string path = "GameObject/Tile";
        string path2 = "GameObject/CellTile";
        string path3 = "GameObject/";

        List<GameObject> loadedtile = Resources.LoadAll<GameObject>(path).ToList();
        List<GameObject> loadedCell = Resources.LoadAll<GameObject>(path2).ToList();
        loadedtile.AddRange(loadedCell);
        loadedtile.ForEach(x =>
        {
            tileDict[x.name] = x;
        });
        wallDecoration = Resources.LoadAll<GameObject>(path3 + "WallDeco").ToList();
        floorDecoration = Resources.LoadAll<GameObject>(path3 + "FloorDeco").ToList();
    }


    public void dataToMaze(tileGridData data)
    {
        tileFullGrid = new List<TileData>();
        StartCoroutine(dataToMazeI(data));   
    }
    IEnumerator dataToMazeI(tileGridData griddata)
    {
        TileData[,] data = griddata.data;

        MazeController.i.mazeData = new mazeData(griddata.solution.Count);
        StaticTool.destroyChildOb(transform);
        yield return null;
        Transform prefab = new GameObject("Cell").transform;
        Vector3 tileSize = new Vector3(12, 0, 12);
        List<NavMeshSurface> surfaces = new List<NavMeshSurface>();

        foreach (TileData i in griddata.solution)
        {
            MazeController.i.mazeData.addSolution(i);
        }


        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                TileData currentData = data[i, j];
                

                if (currentData == null) continue;
                tileFullGrid.Add(currentData);
                Transform p = Instantiate(prefab, transform);
                p.name = "[" + i.ToString() + " , " + j.ToString() + "]";
                p.localPosition = new Vector3(tileSize.x * i, 0, tileSize.z * j);
                //Debug.Log(currentData == null);
                surfaces.AddRange(currentData.loadInto(p));

                Vector3 currentPos = currentData.position;

                if (currentData.isDeadEnd)
                {
                    int soIndex = 0;
                    griddata.deadEndDict.TryGetValue(new Vector2Int(i, j), out soIndex);
                    MazeController.i.mazeData.addSolutionSpawnPoint(soIndex, currentPos);
                }
                if (currentData.isSolutionPath)
                {
                    MazeController.i.mazeData.setSolutionPos(currentData.solutionIndex, currentPos);
                    if (currentData.solutionIndex == 0)
                    {
                        //Debug.Log("START TILE INDEX: " + currentData.solutionIndex + "  " + currentData.position);
                        
                        MazeController.i.mazeData.setStartPos(currentPos);
                    }
                    else if (currentData.solutionIndex == griddata.solution.Count - 1)
                    {
                        MazeController.i.mazeData.setEndPos(currentPos);
                    }
                }
                if (currentData.haveKey)
                {
                    MazeController.i.mazeData.addKey(currentPos);
                }
                if (currentData.haveLpickup)
                {
                    MazeController.i.mazeData.addLucidityPickup(currentPos);
                }

            }
            yield return null;
        }
        navigationBaker.baker.bakeMap(surfaces);
        MazeController.i.mazeData.isReady = true;
    }
}


public enum posType
{
    lucidityPickup,
    keyPickup,
    enemySpawn,
    door,
    branch,
}
[System.Serializable]
public class mazeData
{
    public bool isReady;
    public Vector3 startPos;
    public Vector3 endPos;
    public solutionPart[] part;
    public List<Vector3> lucidityPickupPos;
    public List<Vector3> keyPickupPos;

    public int currentIndex;
    public mazeData(int solutionLength)
    {
        part = new solutionPart[solutionLength];
        lucidityPickupPos = new List<Vector3>();
        keyPickupPos = new List<Vector3>();
    }
    public void setStartPos(Vector3 v)
    {
        startPos = v;
    }
    public void setEndPos(Vector3 v)
    {
        endPos = v;
    }
    public void addLucidityPickup(Vector3 v)
    {
        lucidityPickupPos.Add(v);
    }
    public void addKey(Vector3 v)
    {
        keyPickupPos.Add(v);
    }
    public void addSolution(TileData d)
    {
        part[d.solutionIndex] = new solutionPart(d);
    }
    public void addSolutionSpawnPoint(int d, Vector3 v)
    {
        part[d].addSpawnPoint(v);
    }
    public void setSolutionPos(int d, Vector3 v)
    {
        part[d].setPosition(v);
    }






    public void checkCurrentPos(PlayerController p)
    {
        Vector3 currentP = p.transform.position;
        if (currentIndex + 1 < part.Length)
        {
            Vector3 nextP = part[currentIndex + 1].pos;
            if (isInTile(currentP, nextP))
            {
                currentIndex++;
                return;
            }
        }
        else if (currentIndex - 1 >= 0)
        {
            Vector3 preP = part[currentIndex - 1].pos;
            if (isInTile(currentP, preP))
            {
                currentIndex--;
                return;
            }
        }
    }
    public List<Vector3> getEnemySpawnPoints()
    {
        List<Vector3> v = new List<Vector3>();
        for (int j = currentIndex; j < part.Length; j++)
        {
            if (part[j].haveEnemyPos())
            {
                v = part[j].enemySpawnPos;
                break;
            }
        }
        return v;
    }


    public static bool isInTile(Vector3 player, Vector3 center)
    {
        Vector3 absDis = new Vector3(Mathf.Abs(player.x - center.x), Mathf.Abs(player.y - center.y));
        return absDis.x <= 6 && absDis.y <= 6;
    }
}



[System.Serializable]
public class solutionPart
{
    public int index;
    public Vector3 pos;
    public List<Vector3> enemySpawnPos;

    #region CONSTRUCTOR
    public solutionPart()
    {
        enemySpawnPos = new List<Vector3>();
    }
    public solutionPart(TileData d)
    {
        enemySpawnPos = new List<Vector3>();
        index = d.solutionIndex;
    }
    public void setPosition(Vector3 p)
    {
        this.pos = p;
    }
    public void addSpawnPoint(Vector3 p)
    {
        enemySpawnPos.Add(p);
    }
    #endregion
    public bool haveEnemyPos()
    {
        return enemySpawnPos.Count > 0;
    }
}