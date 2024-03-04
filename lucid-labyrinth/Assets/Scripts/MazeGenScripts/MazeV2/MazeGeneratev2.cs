using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;
public class MazeGeneratev2 : MonoBehaviour
{
    public MazeCellV3 pref;
    public int width = 10;
    public int height = 10;
    public navigationBaker baker;
    public static MazeGeneratev2 i;
    public MazeCellV3[,] mazeGrid;
    public TileData[,] finalMazeData;
#if UNITY_EDITOR
    public tileRulesDatabase tileRulesDatabase;
#endif
    List<MazeCellV3> allCell;
    public void Awake()
    {
        if (i != null) { Destroy(gameObject); return; }
        i = this;
        StartCoroutine(generateMaze());
        baker = GetComponent<navigationBaker>();
    }


    #region SOLUTION PATH
    public IEnumerator generateSolutionPath()
    {
        Vector2Int startPoint = new Vector2Int(2, 1);
        Vector2Int endPoint = new Vector2Int(width - 2, height - 2);
        Vector2Int currentPoint = startPoint;
        mazeGrid[1, 1].finishThisCell("r", true);
        while (currentPoint != endPoint)
        {
            int i = currentPoint.x == endPoint.x ? 1: currentPoint.y == endPoint.y? 0: Random.Range(0, 2);
            if (i == 0)
            {
                mazeGrid[currentPoint.x, currentPoint.y].finishThisCell("r",true);
                mazeGrid[currentPoint.x, currentPoint.y].isSolution = true;
                Debug.Log("Horizontal:::" + currentPoint + "---" + mazeGrid[currentPoint.x, currentPoint.y].finalTile.name);
                currentPoint.x += 1;
            }
            else
            {
                mazeGrid[currentPoint.x, currentPoint.y].finishThisCell("lu",true);
                mazeGrid[currentPoint.x, currentPoint.y].isSolution = true;
                Debug.Log("Vertical:::" + currentPoint + "---" + mazeGrid[currentPoint.x, currentPoint.y].finalTile.name);
                currentPoint.y += 1;
            }
        }
        yield return null;
    }
    #endregion


    public void initMazeData()
    {
        allCell = new List<MazeCellV3>();
        mazeGrid = new MazeCellV3[width, height];
        finalMazeData = new TileData[width, height];



        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                MazeCellV3 m = Instantiate(pref, transform);
                m.initCell(j, i);
                m.transform.localPosition = new Vector3(j, 0,i);
                mazeGrid[j, i] = m;
                allCell.Add(m);
            }
        }

        


        for (int i = 0; i < width; i++)
        {
            MazeCellV3 m = mazeGrid[i, 0];
            m.finishThisCell("lrd");
        }
        for (int i = 0; i < width; i++)
        {
            MazeCellV3 m = mazeGrid[i, height - 1];
            m.finishThisCell("lru");
        }
        MazeCellV3 c = mazeGrid[0, 1];

        c.finishThisCell("lu");



        c = mazeGrid[0, height - 2];
        c.finishThisCell("ld");


        c = mazeGrid[width - 1, 1];
        c.finishThisCell("ru");


        c = mazeGrid[width - 1, height - 2];
        c.finishThisCell("rd");
        for (int i = 2; i < height - 2; i++)
        {
            MazeCellV3 m = mazeGrid[0, i];
            m.finishThisCell("udl");
        }
        for (int i = 2; i < height - 2; i++)
        {
            MazeCellV3 m = mazeGrid[width - 1,i];
            m.finishThisCell("udr");
        }
        //Generate Solution path
        

    }
    public IEnumerator generateMaze()
    {
        initMazeData();
        yield return StartCoroutine(generateSolutionPath());
        int currenti = 0;
        int count = allCell.Count;
        while (count > 1)
        {
            allCell.RemoveAll(x => x.finished);
            allCell = allCell.OrderBy(x => x.finalOptionList.Count).ToList();
            allCell[0].finishThisCell();
            allCell[0].isSolution = false;
            currenti++;
            yield return null;
            count = allCell.Count;
        }
        baker.bakeMap();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                finalMazeData[i, j] = new TileData(mazeGrid[i, j].finalTile.tileData);
                finalMazeData[i, j].isSolutionPath = mazeGrid[i, j].isSolution;
            }
        }
        DataToMaze.i.dataToMaze(finalMazeData);
    }
}



[Serializable]
public class tileRulesDatabase
{
    public List<mazeTile> pup;
    public List<mazeTile> pdown;
    public List<mazeTile> pleft;
    public List<mazeTile> pright;

    public List<mazeTile> wup;
    public List<mazeTile> wdown;
    public List<mazeTile> wleft;
    public List<mazeTile> wright;


    public tileRulesDatabase()
    {
        pup = new List<mazeTile>();
        pdown = new List<mazeTile>();
        pleft = new List<mazeTile>();
        pright = new List<mazeTile>();


        wup = new List<mazeTile>();
        wdown = new List<mazeTile>();
        wleft = new List<mazeTile>();
        wright = new List<mazeTile>();
    }

}