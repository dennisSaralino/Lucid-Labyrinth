using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SideLogic
{
    opnSide,   // open side for potential path or wall
    fWall,     // forced wall
    solPath,   // solution path direction
    brPath,    // branch path direction
    stPath,    // stopped path, temporarily a wall
    altPath,   // alt opnSide to be solPath, for backtracking and logic 
    nullLog,   // used for instantiating before being set
}
    
[Serializable]
public class alTData
{
    #region FOR MAZE GENERATION
    public Vector2Int fullPos;

    public SideLogic upLog;
    public SideLogic downLog;
    public SideLogic leftLog;
    public SideLogic rightLog;

    public bool isStartT = false;
    public bool isEndT = false;
    
    public alTData(Vector2Int tilePos)
    {
        fullPos = tilePos;
        upLog = SideLogic.nullLog;
        downLog = SideLogic.nullLog;
        leftLog = SideLogic.nullLog;
        rightLog = SideLogic.nullLog;
    }
    #endregion

    #region FOR MAZE CONVERSION
    public Vector2Int currentPos; //mazePos
                     

    public bool u, d, l, r;

    public bool isBranching;
    public Vector2Int outBranch;
    public List<alTData> branch;


    public bool isSolution;
    public Vector2Int indir;
    public Vector2Int outdir;
    #endregion


    public alTData GetNeighbor(Vector2Int pos)
    {
        Vector2Int neiPos = currentPos + pos;
        return GridDataGen.mazeGrid[neiPos.x, neiPos.y];
    }

    // get the logic values which ret
    public alTData GetLogNeighbor(Vector2Int offsetN)
    {
        Vector2Int neibPos = fullPos + offsetN;
        return GridDataGen.fullGrid[neibPos.x, neibPos.y];
    }

    /// <summary>
    /// store current SideLogic values and access them via index.
    /// used for comparison, Loops and switch statements
    /// </summary>
    /// <returns> [0-3] = this.{upLog, downLog, leftLog, rightLog} </returns>
    public SideLogic[] CheckSides()
    {
        // check current tile status
        SideLogic[] tileSides = new SideLogic[4];
        tileSides[0] = this.upLog;
        tileSides[1] = this.downLog;
        tileSides[2] = this.leftLog;
        tileSides[3] = this.rightLog;
        return tileSides;
    }

    /// <summary>
    /// get the neighboring tile's adjacent logic
    /// </summary>
    /// <param name="dirIndx"></param>
    /// <returns> </returns>
    public SideLogic NeighborLog(int dirIndx)
    {
        SideLogic nbrLog = SideLogic.nullLog; //  
        switch(dirIndx)
        {
            case 0:
                nbrLog = this.GetLogNeighbor(Vector2Int.up).downLog;
                break;
            case 1:
                nbrLog = this.GetLogNeighbor(Vector2Int.down).upLog;
                break;
            case 2:
                nbrLog = this.GetLogNeighbor(Vector2Int.left).rightLog;
                break;
            case 3:
                nbrLog = this.GetLogNeighbor(Vector2Int.right).leftLog;
                break;
            default:
                Debug.LogWarning("Unexpected dir Index");
                break;
        }
        return nbrLog;
    }

    /// <summary>
    /// used in conjunction with CheckSides to set value 
    /// </summary>
    /// <param name="sideIndex"></param>
    /// <param name="newLogValue"></param>
    public void SetSides(int sideIndex, SideLogic newLogValue) 
    {
        switch (sideIndex)
        {
            case 0:
                this.upLog = newLogValue;
                break;
            case 1:
                this.downLog = newLogValue;
                break;
            case 2:
                this.leftLog = newLogValue;
                break;
            case 3:
                this.rightLog = newLogValue;
                break;
            default:
                Debug.LogWarning("Unexpected sideLogic index");
                break;
        }
    }

    /// <summary>
    /// used to check which side this shares with the adjTile
    /// </summary>
    /// <param name="adjTile"></param>
    /// <returns> an index for direction {u,d,l,r} </returns>
    public int BorderSide(alTData adjTile) 
    {
        int indxOut = -1;  // default for function
        int indx = 0;
        Vector2Int[] directions = new Vector2Int[4] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};


        foreach (Vector2Int direction in directions)
        {
            if (this.fullPos == adjTile.GetLogNeighbor(direction).fullPos)
                indxOut = indx;

            ++indx;
        }

        
        if (indxOut == -1)
            Debug.LogWarning("BorderSide used on non-adjacent Tiles");
        return indxOut;
    }

    public void ResetTile(int maxX, int maxY)
    {
        SideLogic[] dirFaces = this.CheckSides();
        bool isTopBorder = (this.fullPos.y == maxY);
        bool isBottomBorder = (this.fullPos.y == 1);
        bool isLeftBorder = (this.fullPos.x == 1);
        bool isRightBorder = (this.fullPos.x == maxX);
        bool notBorder = (!(isTopBorder  || isBottomBorder || 
                            isLeftBorder || isRightBorder));

        bool[] borders = new bool[4] {isTopBorder, isBottomBorder, isLeftBorder, isRightBorder};

        int indexDir = 0;
        foreach(SideLogic dirFace in dirFaces)
        {
            if (notBorder)
                this.SetSides(indexDir, SideLogic.opnSide);
            else if (borders[indexDir])
                this.SetSides(indexDir, SideLogic.fWall);
           
            ++indexDir;
        }
    }

    public alTData SetNextTile(int dirIndex)
    {
        alTData nxtTile = this;
        switch (dirIndex)
        {
            case 0:
                // set outdir, solPath, nxtTile, and indir
                this.upLog = SideLogic.solPath;
                this.outdir = this.GetLogNeighbor(Vector2Int.up).fullPos;

                nxtTile = GridDataGen.fullGrid[this.outdir.x, this.outdir.y];
                nxtTile.downLog = SideLogic.solPath;
                nxtTile.indir = this.fullPos;
                break;
            case 1:
                // set outdir, solPath, nxtTile, and indir
                this.downLog = SideLogic.solPath;
                this.outdir = this.GetLogNeighbor(Vector2Int.down).fullPos;

                nxtTile = GridDataGen.fullGrid[this.outdir.x, this.outdir.y];
                nxtTile.upLog = SideLogic.solPath;
                nxtTile.indir = this.fullPos;
                break;
            case 2:
                // set outdir, solPath, nxtTile, and indir
                this.leftLog = SideLogic.solPath;
                this.outdir = this.GetLogNeighbor(Vector2Int.left).fullPos;

                nxtTile = GridDataGen.fullGrid[this.outdir.x, this.outdir.y];
                nxtTile.rightLog = SideLogic.solPath;
                nxtTile.indir = this.fullPos;
                break;
            case 3:
                // set outdir, solPath, nxtTile, and indir
                this.rightLog = SideLogic.solPath;
                this.outdir = this.GetLogNeighbor(Vector2Int.right).fullPos;

                nxtTile = GridDataGen.fullGrid[this.outdir.x, this.outdir.y];
                nxtTile.leftLog = SideLogic.solPath;
                nxtTile.indir = this.fullPos;
                break;
            default:
                Debug.LogWarning("Unexpected direction reversed");
                break;
        }

        return nxtTile;
    }


}