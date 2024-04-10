
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum sideType
{
    wall, 
    path,
    door,
}
[Serializable]
public class TileData
{
    public sideType up;
    public sideType down;
    public sideType left;
    public sideType right;
    public bool isSolutionPath;
    public Vector2 pdir1;
    public Vector2 pdir2;
    public TileData()
    {
        up = sideType.wall;
        down = sideType.wall;
        left = sideType.wall;
        right = sideType.wall;
    }
    public TileData(TileData t)
    {
        this.up = t.up;
        this.down = t.down;
        this.left = t.left;
        this.right = t.right;
        this.isSolutionPath = t.isSolutionPath;
    }
    public void loadInto(Transform p)
    {
        #region WALLS
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




[Serializable]
public class alTData
{
    public Vector2Int currentPos;

    public bool u, d, l, r;

    public bool isBranching;
    public Vector2Int outBranch;
    public List<alTData> branch;


    public bool isSolution;
    public Vector2Int indir;
    public Vector2Int outdir;


    public alTData getNeighbor(alTData[,] grid, Vector2Int pos)
    {
        Vector2Int neiPos = currentPos + pos;
        return grid[neiPos.x, neiPos.y];
    }
}