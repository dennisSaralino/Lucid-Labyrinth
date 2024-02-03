using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MazeGeneratev2 : MonoBehaviour
{
    public MazeCellV3 pref;
    public int width = 10;
    public int height = 10;
    Vector2 startPoint = new Vector2(0, 0);
    Vector2 endPoint;
    public static MazeGeneratev2 i;
    public MazeCellV3[,] mazeGrid;
#if UNITY_EDITOR
    public tileRulesDatabase tileRulesDatabase;
#endif
    List<MazeCellV3> allCell;

    bool enter = true;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            enter = true;
            Debug.Log("continue");
        }
    }
    public void Awake()
    {
        if (i != null) { Destroy(gameObject); return; }
        i = this;
        endPoint = new Vector2(width - 1, height - 1);
        StartCoroutine(generateMaze());
    }
    public void initMazeData()
    {
        allCell = new List<MazeCellV3>();
        mazeGrid = new MazeCellV3[width, height];
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
    }
    public IEnumerator generateMaze()
    {
        initMazeData();
        int count = allCell.Count;
        int currenti = 0;


        while (currenti < count)
        {
            allCell.RemoveAll(x => x.finished);
            allCell = allCell.OrderBy(x => x.tileOptions.Count).ToList();
            allCell[0].finishThisCell();
            currenti++;
            //while (!enter)
            //{
            //    yield return new WaitForSeconds(0.5f);
            //    Debug.Log(enter);
            //}
            yield return null;
            enter = false;
        }
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