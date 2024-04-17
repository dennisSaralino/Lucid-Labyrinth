
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
    public bool isBranching;
    public bool isInBranch;
    public bool isDeadEnd;
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
        if (d == null) return;
        up = d.u ? sideType.path: sideType.wall;
        down = d.d ? sideType.path : sideType.wall;
        left = d.l ? sideType.path : sideType.wall;
        right = d.r ? sideType.path : sideType.wall;
        isSolutionPath = d.isSolution;
        isBranching = d.isBranching;
        isInBranch = d.isInBranch;
        isDeadEnd = d.isDeadEnd;
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
    public ref sideType getSide(Vector2Int side)
    {
        if (side.x == -1) return ref left;
        else if (side.x == 1) return ref right;
        else if (side.y == 1) return ref up;
        else if (side.y == -1) return ref down;
        else
        {
            Debug.Log("WTF are you doing here");
            return ref left;
        }
    }

    public void loadInto(Transform p)
    {
        #region WALLS
        p.transform.position = new Vector3(p.transform.position.x, layer * 3.9f, p.transform.position.z);
        GameObject floor = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict["floor"], p);
        GameObject rightside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[right.ToString()], p);
        GameObject upside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[up.ToString()], p);
        GameObject downside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[down.ToString()], p);
        GameObject leftside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[left.ToString()], p);
        downside.transform.Rotate(Vector3.up, 90f);
        leftside.transform.Rotate(Vector3.up, 180f);
        upside.transform.Rotate(Vector3.up, 270f);
        #endregion


        bool isThereDOOR = (right == sideType.door || left == sideType.door || up == sideType.door || down == sideType.door);

        if (isDeadEnd)
        {
            Material solutionmaterial = Resources.Load<Material>("Material/isDeadEnd");
            floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
        }
        else if (isBranching)
        {
            Material solutionmaterial = Resources.Load<Material>("Material/BranchingPath");
            floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
        }
        else if (isInBranch)
        {
            Material solutionmaterial = Resources.Load<Material>("Material/inBranch");
            floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
        }
        else if (isThereDOOR)
        {
            Material solutionmaterial = Resources.Load<Material>("Material/isDoor");
            floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
        }
        else if (isSolutionPath)
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
    static int MaxDoorNum = 3;
   


    public static TileData[,] convertToTiledata(alTData[,] grid)
    {
        TileData[,] tile = new TileData[grid.GetLength(0), grid.GetLength(1)];
        Debug.Log("FULLGRID SZiE" + GridDataGen.thisGrid.xMaxSize + " " + GridDataGen.thisGrid.yMaxSize);
        int mWidth = GridDataGen.fullGrid.GetLength(0);
        int mHeigth = GridDataGen.fullGrid.GetLength(1);
        int doorPlaced = 0;
        Debug.Log("WARN GRID SIZE" + grid.GetLength(0) + " " + grid.GetLength(1));
        int doorNum = MaxDoorNum;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                alTData ct = grid[i, j];
                TileData tileD = null;
               
                //if outerEdges
                if (ct.currentPos.x == 0 || ct.currentPos.x == mWidth - 1 || ct.currentPos.y == 0 || ct.currentPos.y == mHeigth - 1)
                {
                    tileD = new TileData(ct);

                }
                //if qualified for a DOOR
                else if (ct.isSolution && doorNum > 0 && (i > ((mWidth - 2) * (doorPlaced + 1) / MaxDoorNum) || (j > (mHeigth - 2) * (doorPlaced + 1) / MaxDoorNum)))
                {
                    tileD = new TileData(ct);
                    tileD.getSide(ct.outdir - ct.currentPos) = sideType.door;
                    doorNum--;
                    doorPlaced++;
                }
                else
                {
                    tileD = new TileData(ct);
                }


                tile[i, j] = tileD;
            }
        }
        return tile;
    }
    
}