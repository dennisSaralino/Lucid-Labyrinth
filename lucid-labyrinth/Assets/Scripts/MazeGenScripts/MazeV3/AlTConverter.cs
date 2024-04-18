using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class alDataConverter
{
    #region SETTING
    static int MaxDoorNum = 10;
    static int MaxStairNum = 5;
    #endregion




    static alTData[,] grid;
    static int solutionLength;
    static TileData[,] tile;
    static int mWidth;
    static int mHeight;
    static int doorNum;
    static int stairNum;
    static float diagonal;
    
    public static TileData[,] convertToTiledata(GridData gridd)
    {
        grid = gridd.data;
        solutionLength = gridd.solution.Count;
        tile = new TileData[grid.GetLength(0), grid.GetLength(1)];
        mWidth = GridDataGen.fullGrid.GetLength(0);
        mHeight = GridDataGen.fullGrid.GetLength(1);
        doorNum = MaxDoorNum;
        stairNum = MaxStairNum;
        diagonal = Mathf.Sqrt(Mathf.Pow(mWidth - 2, 2) + Mathf.Pow(mHeight - 2, 2));

        previousStair = new List<Vector2Int>() { new Vector2Int(1,1)};


        int layer = 0;
        foreach (alTData i in gridd.solution)
        {
            handleASolution(i, ref layer);
        }
        return tile;
    }

    public static void handleASolution(alTData ct, ref int layer)
    {
        int x = ct.currentPos.x;
        int y = ct.currentPos.y;
        if (tile[x, y] != null && tile[x, y].visited) return;
        TileData tileD = new TileData(ct);
        tileD.layer = layer;



        if (doorNum > 0 && canPlaceDoor(ct.solutionIndex))
        {
            tileD.getSide(ct.outdir - ct.currentPos) = sideType.door;
            doorNum--;
        }
        else if (stairNum > 0 && !ct.isBranching && canPlaceStair(ct))
        {
            placeAStair(ct, tileD, ref layer);
        }
        if (ct.isBranching)
        {
            foreach (Vector2Int branch in ct.outBranches)
            {
                int subLayer = layer;
                
                handleBranch(GridDataGen.fullGrid[branch.x, branch.y], ref subLayer, 0);
            }
        }

            
        tile[x, y] = tileD;
    }

    public static void handleBranch(alTData ct, ref int layer, int branchIndex)
    {
        int x = ct.currentPos.x;
        int y = ct.currentPos.y;
        TileData tileD = new TileData(ct);
        tileD.layer = layer;

        if (stairNum > 0 &&canPlaceStair(ct))
        {
            placeAStair(ct, tileD, ref layer);
        }

        if (ct.isBranching)
        {
            foreach (Vector2Int branch in ct.outBranches)
            {
                int subLayer = layer;
                handleBranch(GridDataGen.fullGrid[branch.x, branch.y], ref subLayer, 0);
            }
        }
        tile[x, y] = tileD;
        Debug.Log("BRANCH: " + ct.currentPos);
        if (!ct.isDeadEnd)
        {
            handleBranch(GridDataGen.fullGrid[ct.outdir.x, ct.outdir.y], ref layer, branchIndex++);
        }
    }


    public static bool canPlaceDoor(int solutionIndex)
    {
        int comparer = Mathf.FloorToInt((solutionLength - 2) / MaxDoorNum) - 1;
        for (int i = 1; i < MaxDoorNum + 1; i++)
        {
            if (solutionIndex % comparer == 0 && solutionIndex / comparer == i) return true;
        }
        return false;
    }
    static List<Vector2Int> previousStair;
    public static bool canPlaceStair(alTData d)
    {
        if (d.isBranching || d.isDeadEnd) return false;

        Vector2Int preToc = d.indir - d.currentPos;
        Vector2Int nextToc = d.outdir - d.currentPos;
        Vector2Int off = nextToc + preToc;
        if (off.x != 0) return false;

        Vector2Int c = new Vector2Int(d.currentPos.x, d.currentPos.y);
        float min = 0;
        min = previousStair.Min(x => Vector2Int.Distance(x, c));
        return min > diagonal / MaxStairNum;
    }
    public static void placeAStair(alTData ct, TileData tileD, ref int layer)
    {
        bool isUp = Random.Range(0, 2) == 0;
        tileD.getSide(ct.outdir - ct.currentPos) =  isUp? sideType.upStair : sideType.downStair;
        layer += isUp ? 1 : -1;
        stairNum--;
        previousStair.Add(ct.currentPos);
    }
}