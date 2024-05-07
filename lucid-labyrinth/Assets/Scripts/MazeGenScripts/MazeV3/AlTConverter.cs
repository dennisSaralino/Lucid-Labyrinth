using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class alDataConverter
{
    #region SETTING
    public static int MaxDoorNum = 1;
    public static int MaxStairNum = 1;
    public static int trapFrequency = 10;//20; //1 Trap for every {trapFrequency} tiles.
    public static int decoFrequency = 2; //1 Decoration object for every {decoFrequency} tiles
    public static int lucidityPickupFrequency = 5; //10; //1 Lucidity pickup every {lucidityPickupFrequency} tiles
                                                   //should be >= 4  for generation purposes
    #endregion
    #region REPORT
    static int stairCount;
    static int doorCount;
    static int trapCount;
    static int decoCount;
    #endregion


    static alTData[,] grid;
    static int solutionLength;
    static TileData[,] resultTileGrid;
    static int mWidth;
    static int mHeight;
    static int doorNum;
    static int stairNum;
    static float diagonal;
    static int decorationTimer;
    static int lucidityPickupTimer;
    static int trapTimer;
    public static int minLayer = 10000;
    public static int maxLayer = -10000;
    static List<TileData> tileBeforeDoor;
    static Dictionary<Vector2Int, int> deadEndDict;
    static List<TileData> solutionD;

    public static tileGridData convertToTiledata(GridData gridd)
    {
        deadEndDict = new Dictionary<Vector2Int, int>();
        solutionD = new List<TileData>();
        tileBeforeDoor = new List<TileData>();
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

        printReport();

        tileGridData d = new tileGridData(resultTileGrid, deadEndDict, solutionD, minLayer, maxLayer);
        return d;
    }
    public static void printReport()
    {
        string report = "";
        report += "PLACED DOOR: " + doorCount + "\n";
        report += "PLACED STAIR: " + stairCount + "\n";
        report += "PLACED TRAP: " + trapCount + "\n";
        report += "PLACED DECORATION: " + decoCount + "\n";
        StaticTool.printReport(report, "MAZE CONVERSION");
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
                    TileData t0 = resultTileGrid[i, 1];
                    setOuterTile(tileD, t0);
                    resultTileGrid[i, 0] = tileD;


                    TileData tileD1 = new TileData(grid[i, mHeight - 1]);
                    TileData t = resultTileGrid[i, mHeight - 2];
                    setOuterTile(tileD1, t);
                    resultTileGrid[i, mHeight - 1] = tileD1;
                }

            }
            if (i < mHeight)
            {
                if (i != 0 && i != mHeight - 1)
                {
                    TileData tileD = new TileData(grid[0, i]);
                    TileData t0 = resultTileGrid[1, i];
                    setOuterTile(tileD, t0);
                    resultTileGrid[0, i] = tileD;


                    TileData tileD1 = new TileData(grid[mWidth - 1, i]);
                    TileData t = resultTileGrid[mWidth - 2, i];
                    setOuterTile(tileD1, t);
                    resultTileGrid[mWidth - 1, i] = tileD1;
                }
            }
        }
    }

    public static void setOuterTile(TileData tileD1, TileData t)
    {
        tileD1.layer = t.layer;
        if (t.isStair)
        {
            int offset = 0;
            offset = tileD1.isStartTile ? 0 : t.isStairUp ? 1 : -1;
            tileD1.layer = t.layer + offset;
        }
        tileD1.setBaseOnSides();
    }












    public static void handleASolution(alTData ct, ref int layer)
    {
        
        int x = ct.fullPos.x;
        int y = ct.fullPos.y;
        if (resultTileGrid[x, y] != null && resultTileGrid[x, y].visited) return;
        TileData tileD = new TileData(ct);
        tileD.layer = layer;
        if (doorNum > 0 && canPlaceDoor(ct.solutionIndex) && !ct.isEndT && !ct.isStartT)
        {
            tileBeforeDoor = tileBeforeDoor.FindAll(x=>!x.haveLpickup);
            TileData keyTile = tileBeforeDoor[Random.Range(0, tileBeforeDoor.Count)];
            tileBeforeDoor.Clear();
            keyTile.setHaveKey();
            doorCount++;
            tileD.getSide(ct.indir - ct.fullPos) = SideType.Door;
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
                
                handleBranch(tileD,GridDataGen.fullGrid[branch.x, branch.y], ref subLayer, 0);
            }
        }


        tileD.setBaseOnSides();
        checkForLucidityPickup(tileD);
        checkForDecoration(tileD);
        checkForTrap(tileD);

        if (!tileD.isStartTile && !tileD.isDoor) 
            tileBeforeDoor.Add(tileD);
        solutionD.Add(tileD);
        resultTileGrid[x, y] = tileD;
    }

    public static void handleBranch(TileData solutionT,alTData ct, ref int layer, int branchIndex)
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
                handleBranch(solutionT, GridDataGen.fullGrid[branch.x, branch.y], ref subLayer, 0);
            }
        }
        tileD.setBaseOnSides();
        checkForLucidityPickup(tileD);
        checkForDecoration(tileD);
        checkForTrap(tileD);

        tileBeforeDoor.Add(tileD);
        resultTileGrid[x, y] = tileD;
        if (ct.isDeadEnd)
        {
            deadEndDict[ct.fullPos] = solutionT.solutionIndex;
        }
        if (!ct.isDeadEnd && !ct.isInOuterEdges())
        {
            handleBranch(solutionT, GridDataGen.fullGrid[ct.outdir.x, ct.outdir.y], ref layer, branchIndex++);
        }
    }


    public static bool canPlaceDoor(int solutionIndex)
    {
        int comparer = Mathf.FloorToInt((solutionLength - 2) / MaxDoorNum) - 1;
        for (int i = 1; i < MaxDoorNum + 1; i++)
        {
            if (solutionIndex % comparer == 0 && solutionIndex / comparer == i)
            {
                return true;
            }
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
        tileD.getSide(ct.outdir - ct.fullPos) =  isUp? SideType.upStair : SideType.downStair;
        layer += isUp ? 1 : -1;
        if (layer > maxLayer) maxLayer = layer;
        else if (layer < minLayer) minLayer = layer;
        stairCount++;
        stairNum--;
        previousStair.Add(ct.fullPos);
        
    }
    public static void checkForLucidityPickup(TileData tileD)
    {
        lucidityPickupTimer++;
        if (lucidityPickupTimer >= lucidityPickupFrequency)
        {
            tileD.haveLpickup = true;
            lucidityPickupTimer = 0;
        }
    }
    public static void checkForDecoration(TileData tileD)
    {
        decorationTimer++;
        if (decorationTimer >= decoFrequency && tileD.setDecorationTrue())
        {
            decorationTimer = 0;
            decoCount++;
        }
    }
    public static void checkForTrap(TileData tileD)
    {
        trapTimer++;
        if(trapTimer >= trapFrequency && tileD.setTrap())
        {
            trapTimer = 0;
            trapCount++;
        }
    }
    
}

public class tileGridData
{
    public TileData[,] data;
    public Dictionary<Vector2Int, int> deadEndDict;
    public List<TileData> solution;
    public int minLayer;
    public int maxLayer;

    public tileGridData(TileData[,] d, Dictionary<Vector2Int, int> deade, List<TileData> so, int minl, int maxl)
    {
        data = d;
        deadEndDict = deade;
        solution = so;
        minLayer = minl;
        maxLayer = maxl;
    }
}