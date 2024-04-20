using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class alDataConverter
{
    #region SETTING
    static int MaxDoorNum = 6;
    static int MaxStairNum = 5;
    #endregion
    static alTData[,] grid;
    static int solutionLength;
    static TileData[,] resultTileGrid;
    static int mWidth;
    static int mHeight;
    static int doorNum;
    static int stairNum;
    static float diagonal;
    
    public static TileData[,] convertToTiledata(GridData gridd)
    {
        grid = gridd.data;
        solutionLength = gridd.solution.Count;
        resultTileGrid = new TileData[grid.GetLength(0), grid.GetLength(1)];
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

        handleOuterEdges();
        return resultTileGrid;
    }


    public static void handleOuterEdges()
    {
        int maxLength = Mathf.Max(mWidth, mHeight);
        for (int i = 0; i < maxLength; i++)
        {
            if (i < mWidth)
            {

                if (i != 0 && i != mWidth - 1)
                {
                    TileData tileD = new TileData(grid[i, 0]);
                    tileD.layer = resultTileGrid[i, 1].layer;
                    resultTileGrid[i, 0] = tileD;

                    TileData tileD1 = new TileData(grid[i, mHeight - 1]);
                    tileD1.layer = resultTileGrid[i, mHeight - 2].layer;
                    resultTileGrid[i, mHeight - 1] = tileD1;
                }

            }
            if (i < mHeight)
            {
                if (i != 0 && i != mHeight - 1)
                {
                    TileData tileD = new TileData(grid[0, i]);
                    tileD.layer = resultTileGrid[1, i].layer;
                    resultTileGrid[0, i] = tileD;

                    TileData tileD1 = new TileData(grid[mWidth - 1, i]);
                    tileD1.layer = resultTileGrid[mWidth - 2, i].layer;
                    resultTileGrid[mWidth - 1, i] = tileD1;
                }
            }
        }
    }

    public static void handleASolution(alTData ct, ref int layer)
    {
        int x = ct.fullPos.x;
        int y = ct.fullPos.y;
        if (resultTileGrid[x, y] != null && resultTileGrid[x, y].visited) return;
        TileData tileD = new TileData(ct);
        tileD.layer = layer;



        if (doorNum > 0 && canPlaceDoor(ct.solutionIndex) && !ct.isEndT && ct.isStartT)
        {
            tileD.getSide(ct.outdir - ct.fullPos) = sideType.door;
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

            
        resultTileGrid[x, y] = tileD;
    }

    public static void handleBranch(alTData ct, ref int layer, int branchIndex)
    {
        int x = ct.fullPos.x;
        int y = ct.fullPos.y;
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
        resultTileGrid[x, y] = tileD;
        if (!ct.isDeadEnd && !ct.isInOuterEdges())
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
        if (d.isBranching || d.isDeadEnd || d.isStartT || d.isEndT) return false;

        Vector2Int preToc = d.indir - d.fullPos;
        Vector2Int nextToc = d.outdir - d.fullPos;
        Vector2Int off = nextToc + preToc;
        if (off.x != 0) return false;

        Vector2Int c = new Vector2Int(d.fullPos.x, d.fullPos.y);
        float min = 0;
        min = previousStair.Min(x => Vector2Int.Distance(x, c));
        return min > diagonal / MaxStairNum;
    }
    public static void placeAStair(alTData ct, TileData tileD, ref int layer)
    {
        bool isUp = Random.Range(0, 2) == 0;
        tileD.getSide(ct.outdir - ct.fullPos) =  isUp? sideType.upStair : sideType.downStair;
        layer += isUp ? 1 : -1;
        stairNum--;
        previousStair.Add(ct.fullPos);
        
    }
}