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

    public bool isDeadEnd;
    public bool isInBranch;

    public List<alTData> branchTiles;
    public List<bool> outBranchCompl;
    
    public alTData(Vector2Int tilePos)
    {
        fullPos = tilePos;
        upLog = SideLogic.nullLog;
        downLog = SideLogic.nullLog;
        leftLog = SideLogic.nullLog;
        rightLog = SideLogic.nullLog;

        isBranching = false;
        isSolution = false;
        isDeadEnd = false;
        isInBranch = false;
    }
    #endregion

    #region FOR MAZE CONVERSION
    public Vector2Int currentPos; //mazePos
                     

    public bool u, d, l, r;

    public bool isBranching;
    public List<Vector2Int> outBranches;
    public List<List<alTData>> branchElements;  // 


    public bool isSolution;
    public Vector2Int indir;
    public Vector2Int outdir;
    #endregion


    #region ACCESSORS
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
    /// checks for if the tile in dirIndx direction is in
    /// a path of the Solution or a Branch already
    /// </summary>
    /// <param name="dirIndx"></param>
    /// <returns> Nbr.isSolution || Nbr.isInBranch </returns>
    public bool NeighborPathLog(int dirIndx)
    {

        bool nbrPLog = false;
        alTData nbrTile;

        switch (dirIndx)
        {
            case 0:
                nbrTile = this.GetLogNeighbor(Vector2Int.up);
                nbrPLog = (nbrTile.isSolution || nbrTile.isInBranch);               
                break;
            case 1:
                nbrTile = this.GetLogNeighbor(Vector2Int.down);
                nbrPLog = (nbrTile.isSolution || nbrTile.isInBranch);
                break;
            case 2:
                nbrTile = this.GetLogNeighbor(Vector2Int.left);
                nbrPLog = (nbrTile.isSolution || nbrTile.isInBranch);
                break;
            case 3:
                nbrTile = this.GetLogNeighbor(Vector2Int.right);
                nbrPLog = (nbrTile.isSolution || nbrTile.isInBranch);
                break;
            default:
                Debug.LogWarning("Unexpected dir Index");
                break;
        }

        return nbrPLog;
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

    #endregion

    #region MODIFIERS

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

    public void ResetTile(int maxX, int maxY, int mzEndX, int mzEndY)
    {
        SideLogic[] dirFaces = this.CheckSides();
        int thisX = this.fullPos.x;
        int thisY = this.fullPos.y;

        // prepare Grid logic for determining tile's SideLogic
        bool isTopBorder = ((thisY == maxY) && !(thisX == mzEndX));
        bool isBottomBorder = (thisY == 1);
        bool isLeftBorder = (thisX == 1);
        bool isRightBorder = ((thisX == maxX) && !(thisY == mzEndY)); 
        //bool notBorder = (!(isTopBorder  || isBottomBorder || 
        //                    isLeftBorder || isRightBorder));

        // prepare pairing of grid logic and tile logic
        bool[] borders = new bool[4] {isTopBorder, isBottomBorder, isLeftBorder, isRightBorder};

        int indexDir = 0;
        foreach(SideLogic dirFace in dirFaces)
        {
            if (borders[indexDir]) // side matches bordering edge, except end
                this.SetSides(indexDir, SideLogic.fWall);
            else
                this.SetSides(indexDir, SideLogic.opnSide);

            ++indexDir;
        }
    }

    public alTData SetNextTile(int dirIndex, SideLogic pathType)
    {
        bool setBranch = (pathType == SideLogic.brPath);
        Vector2Int nxtCoord;

        alTData nxtTile = this;
        switch (dirIndex)
        {                
            // set outdir, pathType, nxtTile

            case 0:  // up
                this.upLog = pathType;
                nxtCoord = this.GetLogNeighbor(Vector2Int.up).fullPos;

                // type of outData depends on branch or not
                if (setBranch) this.outBranches.Add(nxtCoord);
                else this.outdir = nxtCoord;

                nxtTile = GridDataGen.fullGrid[nxtCoord.x, nxtCoord.y];
                nxtTile.downLog = pathType;
                break;
            case 1:  // down
                this.downLog = pathType;
                nxtCoord = this.GetLogNeighbor(Vector2Int.down).fullPos;

                // type of outData depends on branch or not
                if (setBranch) this.outBranches.Add(nxtCoord);
                else this.outdir = nxtCoord;

                nxtTile = GridDataGen.fullGrid[nxtCoord.x, nxtCoord.y];
                nxtTile.upLog = pathType;
                break;
            case 2:  // left
                this.leftLog = pathType;
                nxtCoord = this.GetLogNeighbor(Vector2Int.left).fullPos;

                // type of outData depends on branch or not
                if (setBranch) this.outBranches.Add(nxtCoord);
                else this.outdir = nxtCoord;

                nxtTile = GridDataGen.fullGrid[nxtCoord.x, nxtCoord.y];
                nxtTile.rightLog = pathType;
                break;
            case 3:  // right
                this.rightLog = pathType;
                nxtCoord = this.GetLogNeighbor(Vector2Int.right).fullPos;

                // type of outData depends on branch or not
                if (setBranch) this.outBranches.Add(nxtCoord);
                else this.outdir = nxtCoord;

                nxtTile = GridDataGen.fullGrid[nxtCoord.x, nxtCoord.y];
                nxtTile.leftLog = pathType;
                break;
            default:
                Debug.LogWarning("Unexpected direction reversed");
                break;
        }

        return nxtTile;
    }
    
    public alTData MakeBranch(int dirIndx)//, int totalDepth, int currDepth)
    {
        alTData nxtBrTile = this.SetNextTile(dirIndx, SideLogic.brPath);
        nxtBrTile.indir = this.fullPos;

        this.outBranchCompl.Add(false);
        this.branchTiles.Add(nxtBrTile);

        return nxtBrTile;
    }

    public void ChkBrOptions(bool pushThru, out List<int> branchOptions, out List<int> branchDs, out int brCount)
    {
        // prep and store branch options
        branchOptions = new();
        branchDs = new();
        branchOptions.Add(GridDataGen.noBranch);  // "none option"

        SideLogic[] branchDirs = this.CheckSides();
        int dirCount = 0;
        brCount = 0;
        foreach (SideLogic branchDir in branchDirs)
        {
            if (branchDir == SideLogic.brPath)
            {
                // counts number of branches the tile has                
                if (!this.outBranchCompl[brCount])
                    branchDs.Add(dirCount);
                ++brCount;
            }
            else if (branchDir == SideLogic.opnSide ||
                    (branchDir == SideLogic.stPath && pushThru))
            {
                // neighboring Tile is in a solution or branch path, make a wall                  
                if (this.NeighborPathLog(dirCount))
                    this.SetSides(dirCount, SideLogic.fWall);
                else
                    branchOptions.Add(dirCount);
            }

            ++dirCount;
        }
    }

    public void PrepTile()
    {
        SideLogic[] tileSides = this.CheckSides();

        bool[] dirFace = new bool[4] {this.u, this.d, this.l, this.r };

        int indxDir = 0;
        foreach(SideLogic tileSide in tileSides)
        {
            if (tileSide == SideLogic.opnSide || tileSide == SideLogic.solPath || tileSide == SideLogic.brPath)
                dirFace[indxDir] = true;
            else if (tileSide == SideLogic.fWall || tileSide == SideLogic.stPath)
                dirFace[indxDir] = false;
            else
            {
                Debug.LogWarning("PrepTile has come across an unfinished tile.");
                Debug.LogWarning("Tile: " + this.fullPos.ToString() + 
                    " has a value of " + tileSide + " on side: " + indxDir);
            }

            ++indxDir;
        }

        this.u = dirFace[0];
        this.d = dirFace[1];
        this.l = dirFace[2];
        this.r = dirFace[3];
    }

    #endregion
}