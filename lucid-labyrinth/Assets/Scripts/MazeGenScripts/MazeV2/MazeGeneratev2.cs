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
        mazeGrid[1, 1].isSolution = true;
        finalMazeData[1, 1] = new TileData(mazeGrid[1, 1].finalTile.tileData);
        finalMazeData[1, 1].isSolutionPath = mazeGrid[1, 1].isSolution;
        finalMazeData[1, 1].right = SideType.door;
        int stepCount = 0;
        string previous = "";
        int cLayer = 0;
        while (currentPoint != endPoint)
        {
            int i = currentPoint.x == endPoint.x ? 1: currentPoint.y == endPoint.y? 0: Random.Range(0, 2);
            bool isStair = false;
            MazeCellV3 target = mazeGrid[currentPoint.x, currentPoint.y];
            if (i == 0)
            {
                if (stepCount > 5 && previous == "r" && target.finalOptionList.Find(x => x.name == "lr"))
                {

                    target.finishThisCell("lr", false);
                    target.isStair = true;
                    isStair = true;
                    stepCount = 0;
                }
                else
                {
                    target.finishThisCell("r", true);
                    previous = "r";
                }



                target.isSolution = true;
                currentPoint.x += 1;
            }
            else
            {
                target.finishThisCell("lu",true);
                previous = "lu";
                target.isSolution = true;
                currentPoint.y += 1;
            }
            target.layer = cLayer;
            if (isStair) cLayer++;
            stepCount++;
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
                if (finalMazeData[i, j] == null)
                {
                    MazeCellV3 mazeCell = mazeGrid[i, j];

                    finalMazeData[i, j] = new TileData(mazeCell.finalTile.tileData);

                    TileData data = finalMazeData[i, j];
                    data.isSolutionPath = mazeCell.isSolution;
                    data.layer = mazeCell.layer;
                    if(mazeCell.isStair)
                        data.right = SideType.upStair;
                }

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