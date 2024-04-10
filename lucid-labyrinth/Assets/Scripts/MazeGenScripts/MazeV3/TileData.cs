
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum sideType
{
    wall, 
    path,
    door,
    upStair,
    downStair,
}
[Serializable]
public class TileData
{
    public sideType up;
    public sideType down;
    public sideType left;
    public sideType right;
    public bool isSolutionPath;
    public float layer = 0;
    public TileData()
    {
        up = sideType.wall;
        down = sideType.wall;
        left = sideType.wall;
        right = sideType.wall;
    }
    public TileData(alTData d)
    {
        up = d.u ? sideType.path: sideType.wall;
        down = d.d ? sideType.path : sideType.wall;
        left = d.l ? sideType.path : sideType.wall;
        right = d.r ? sideType.path : sideType.wall;
        isSolutionPath = d.isSolution;
        layer = 0;
    }
    public TileData(TileData t)
    {
        this.up = t.up;
        this.down = t.down;
        this.left = t.left;
        this.right = t.right;
        this.isSolutionPath = t.isSolutionPath;
    }
    public void setSide(Vector2Int side, sideType value)
    {
        if (side.x == 1) right = value;
        else if (side.x == -1) left = value;
        else if (side.x == 1) up = value;
        else if(side.y == -1) down = value;
    }


    public void loadInto(Transform p)
    {
        #region WALLS
        p.transform.position = new Vector3(p.transform.position.x, layer * 3.9f,p.transform.position.z);
        GameObject floor = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict["floor"],p);
        GameObject rightside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[right.ToString()], p);
        GameObject upside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[up.ToString()], p);
        GameObject downside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[down.ToString()], p);
        GameObject leftside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[left.ToString()], p);
        downside.transform.Rotate(Vector3.up, 90f);
        leftside.transform.Rotate(Vector3.up, 180f);
        upside.transform.Rotate(Vector3.up, 270f);
        #endregion

        if (isSolutionPath)
        {
            Material solutionmaterial = Resources.Load<Material>("Material/SolutionPath");
            floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
            
        }


        #region TRAP
        #endregion
    }
}
public class alDataConverter
{
    alTData[,] grid;
    public static List<TileData> convertToTiledata(alTData[,] grid)
    {
        List<TileData> tile = new List<TileData>();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); i++)
            {
                alTData ct = grid[i, j];
                TileData d = new TileData(ct);
                tile.Add(d);
                
            }
        }
        return tile;
    }
    //public void processACell(alTData[,] grid ,int i, int j, int layer)
    //{
    //    alTData dt = grid[i, j];
    //    dt.finished = true;




    //    TileData tiled = new TileData();
    //    tiled.layer = layer;
    //    List<KeyValuePair<Vector2Int, bool>> dimen = new List<KeyValuePair<Vector2Int, bool>>();
    //    dimen[0] = new KeyValuePair<Vector2Int, bool>(new Vector2Int(0, 1), dt.u);
    //    dimen[1] = new KeyValuePair<Vector2Int, bool>(new Vector2Int(0, -1), dt.d);
    //    dimen[2] = new KeyValuePair<Vector2Int, bool>(new Vector2Int(-1, 0), dt.l);
    //    dimen[3] = new KeyValuePair<Vector2Int, bool>(new Vector2Int(0, 1), dt.r);

    //    //wall
    //    for (int m = 3; m >= 0; m--)
    //    {
    //        if (dimen[m].Value == false)
    //        {
    //            tiled.setSide(dimen[m].Key, sideType.wall);
    //            dimen.Remove(dimen[m]);
    //        }
    //    }
    //    if (dt.isBranching)
    //    {
    //        for (int z = 0; z < dt.outBranch.Count; z++)
    //        {
    //            //dealing with layer
    //            //end
    //            processACell(grid, dt.outBranch[z].x, dt.outBranch[z].y, layer);
    //        }
    //    }

    //}

}