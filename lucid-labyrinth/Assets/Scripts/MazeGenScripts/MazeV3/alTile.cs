using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public enum SideLogic
{
    opnSide,   // open side for potential path or wall
    fWall,     // forced wall
    solPath,   // solution path direction
    brPath,    // branch path direction
    brIn,      // 
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

        // default;
        u = false;
        d = false;
        l = false;
        r = false;

        isBranching = false;
        isSolution = false;
        isDeadEnd = false;
        isInBranch = false;
        pathCompl = false;
        tileDepth = 0;
        timesChkd = 0;

        outdir = GridDataGen.nullOutDir;
        branchTiles = new();
        outBranches = new List<Vector2Int>();
    }
    #endregion

    #region FOR BRANCHING 
    public bool isDeadEnd;
    public bool isInBranch;

    public int tileDepth;
    public int timesChkd;
    public List<alTData> branchTiles;
    public bool pathCompl;
    public BranchOpts branchOpts;


    public class BranchOpts
    {
        public List<int> nwBrOpts = new List<int>();
        public List<int> currBrs  = new List<int>();
    }
    #endregion

    #region FOR MAZE CONVERSION
    public Vector2Int currentPos; //mazePos
                     

    public bool u, d, l, r;

    public bool isBranching;
    public List<Vector2Int> outBranches;
    //public List<List<alTData>> branchElements;  // 

    public bool isSolution;
    public Vector2Int indir;
    public Vector2Int outdir;
    public int solutionIndex;
    #endregion


    #region ACCESSORS
    public alTData GetNeighbor(Vector2Int pos)
    {
        Vector2Int neiPos = currentPos + pos;
        return GridDataGen.fullGrid[neiPos.x, neiPos.y];
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
    /// take a direction index from  
    /// {u,d,l,r} = {0, 1, 2, 3}
    /// </summary>
    /// <param name="dirIndx"></param>
    /// <returns> the tile in specified direction </returns>
    /// 
    public alTData GetNbrInDir(int dirIndx)
    {
        alTData nbrTile = null;

        switch(dirIndx)
        {
            case 0:
                nbrTile = this.GetLogNeighbor(Vector2Int.up);
                break;
            case 1:
                nbrTile = this.GetLogNeighbor(Vector2Int.down);
                break;
            case 2:
                nbrTile = this.GetLogNeighbor(Vector2Int.left);
                break;
            case 3:
                nbrTile = this.GetLogNeighbor(Vector2Int.right);
                break;
            default:
                Debug.LogWarning("GetNbrInDir recieved unexpected dir Index");
                break;
        }

        //Debug.Log("GetNbrInDir found: " + nbrTile.fullPos.ToString());

        return nbrTile;

    }

    public string CollectSides()
    {
        string sideData = ("for Tile: " + this.fullPos.ToString());
        SideLogic[] sides = CheckSides();

        int sideCount = 0;
        foreach (SideLogic side in sides)
        {
            sideData += (" Side # [" + sideCount + "] is: " + sides[sideCount]);
            ++sideCount;
        }

        return sideData += "\n";
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

        //Debug.Log("this tile: " + this.fullPos.ToString() + " adjTile is: " + adjTile.fullPos.ToString());
        if (indxOut == -1)
            Debug.LogWarning("BorderSide used on non-adjacent Tiles");
        return indxOut;
    }

    // getting BranchOpts
    public BranchOpts GetBrOpts()//, List<int> brOpts, List<int> bDirs)
    {
        BranchOpts tileOpts = new();

        this.ChkBrOptions(ref tileOpts.nwBrOpts, ref tileOpts.currBrs);

        return tileOpts;
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
            // side matches bordering edge, except end
            SideLogic sideLogic = borders[indexDir] ? SideLogic.fWall : SideLogic.opnSide;
            //if (borders[indexDir]) 
            //    this.SetSides(indexDir, sideLogic);
            //else
            //    this.SetSides(indexDir, SideLogic.opnSide);
            this.SetSides(indexDir, sideLogic);

            ++indexDir;
        }
        isSolution = false;  // testing potentially remove
    }

    public alTData SetNextTile(int dirIndex, SideLogic pathType, SideLogic pathInType)
    {
        bool setNwBranch = (this.isBranching);
        //bool setBranch = (pathType == SideLogic.brPath);
        //if(setBranch && (this.outBranches.Count == ))

        #region CASE DATA FIELDS
        int outLogicIndx = dirIndex;  // same as input index
        Vector2Int outDirV = GridDataGen.nullOutDir;
        int nxtInLogicIndx = 0;

        // set up opposite arry = { d , u , r , l } 
        int[] oppDir = new int[4] { 1, 0, 3, 2 };

        bool corrCase = false;  // allows use of the data retrieved from Switch
        #endregion

        Vector2Int outNbr;
        alTData nxtTile = this;
        
        switch (dirIndex)
        {
            case 0:  // up
                outLogicIndx = dirIndex;            // sets to this.upLog index
                outDirV = Vector2Int.up;            // sets outdir vector to up
                nxtInLogicIndx = oppDir[dirIndex];  // sets to nxtTile.downLog index
                corrCase = true;
                break;
            case 1:  // down
                outLogicIndx = dirIndex;            // sets to this.downLog index
                outDirV = Vector2Int.down;          // sets outdir vector to down
                nxtInLogicIndx = oppDir[dirIndex];  // sets to nxtTile.upLog index
                corrCase = true;
                break;
            case 2:  // left
                outLogicIndx = dirIndex;            // sets to this.leftLog index
                outDirV = Vector2Int.left;          // sets outdir vector to left
                nxtInLogicIndx = oppDir[dirIndex];  // sets to nxtTile.rightLog index
                corrCase = true;
                break;
            case 3:  // right
                outLogicIndx = dirIndex;            // sets to this.rightLog index
                outDirV = Vector2Int.right;         // sets outDir vector to right
                nxtInLogicIndx = oppDir[dirIndex];  // sets to nxtTile.leftLog index
                corrCase = true;
                break;
            default:
                Debug.LogWarning("Unexpected direction returned");
                break;
        }


        if (corrCase) // if a correct index given this will run with case data
        {
            // set outdir, pathType, nxtTile
            #region USE CASE DATA TO SET nxtTile
            this.SetSides(outLogicIndx, pathType);              // set outLogic of this Tile
            outNbr = this.GetLogNeighbor(outDirV).fullPos;      // get outNbr fullPos

            // checks if branching
            if (setNwBranch)
                this.outBranches.Add(outNbr);  // add outNbr to outBranches if branching
            else
                this.outdir = outNbr;          // set outNbr fullPos to outdir

            nxtTile = GridDataGen.fullGrid[outNbr.x, outNbr.y]; // set nxtTile from fullGrid by using outNbr
            nxtTile.SetSides(nxtInLogicIndx, pathInType);       // set inLogic of nxtTile
            #endregion
        }

        //Debug.Log("Setting nextTile from: " + this.fullPos.ToString() + " to " + nxtTile.fullPos.ToString());
        return nxtTile;
    }

    //public alTData SetNextTile(int dirIndex, SideLogic pathType, SideLogic pathInType)
    //{
    //    bool setBranch = (pathType == SideLogic.brPath);
    //    //if(setBranch && (this.outBranches.Count == ))
    //    Vector2Int nxtCoord;

    //    alTData nxtTile = this;
    //    switch (dirIndex)
    //    {                
    //        // set outdir, pathType, nxtTile

    //        case 0:  // up
    //            this.upLog = pathType;
    //            nxtCoord = this.GetLogNeighbor(Vector2Int.up).fullPos;

    //            // type of outData depends on branch or not
    //            if (setBranch)
    //                this.outBranches.Add(nxtCoord);
    //            else this.outdir = nxtCoord;

    //            nxtTile = GridDataGen.fullGrid[nxtCoord.x, nxtCoord.y];
    //            nxtTile.downLog = pathInType;
    //            break;
    //        case 1:  // down
    //            this.downLog = pathType;
    //            nxtCoord = this.GetLogNeighbor(Vector2Int.down).fullPos;

    //            // type of outData depends on branch or not
    //            if (setBranch) 
    //                this.outBranches.Add(nxtCoord);
    //            else this.outdir = nxtCoord;

    //            nxtTile = GridDataGen.fullGrid[nxtCoord.x, nxtCoord.y];
    //            nxtTile.upLog = pathInType;
    //            break;
    //        case 2:  // left
    //            this.leftLog = pathType;
    //            nxtCoord = this.GetLogNeighbor(Vector2Int.left).fullPos;

    //            // type of outData depends on branch or not
    //            if (setBranch) 
    //                this.outBranches.Add(nxtCoord);
    //            else this.outdir = nxtCoord;

    //            nxtTile = GridDataGen.fullGrid[nxtCoord.x, nxtCoord.y];
    //            nxtTile.rightLog = pathInType;
    //            break;
    //        case 3:  // right
    //            this.rightLog = pathType;
    //            nxtCoord = this.GetLogNeighbor(Vector2Int.right).fullPos;

    //            // type of outData depends on branch or not
    //            if (setBranch)
    //                this.outBranches.Add(nxtCoord);
    //            else this.outdir = nxtCoord;

    //            nxtTile = GridDataGen.fullGrid[nxtCoord.x, nxtCoord.y];
    //            nxtTile.leftLog = pathInType;
    //            break;
    //        default:
    //            Debug.LogWarning("Unexpected direction reversed");
    //            break;
    //    }

    //    return nxtTile;
    //}
    #endregion

    #region BRANCHING 
    public alTData MakeBranch(int dirIndx, int tileLayer)//, int totalDepth, int currDepth)
    {

        alTData nxtBrTile = this.SetNextTile(dirIndx, SideLogic.brPath, SideLogic.brIn);
        nxtBrTile.indir = this.fullPos;

        this.branchTiles.Add(nxtBrTile);
        nxtBrTile.isInBranch = true;
        nxtBrTile.tileDepth = tileLayer;

        return nxtBrTile;
    }

    public void ChkBrOptions(ref List<int> branchOptions, ref List<int> branchDs)
    {
        // prep and store branch options with most recent logic
        branchOptions.Clear();
        branchDs.Clear();
        ++this.timesChkd;
        alTData nbrTile;
        string tileNbrPos;
        string chkBrDB = ("ChkBrOp is checking tile: " + fullPos.ToString() +
                          "for time #: " + timesChkd + "\n");


        SideLogic[] branchDirs = this.CheckSides();
        int dirCount = 0;
        int wallCount = 0;
        foreach (SideLogic branchDir in branchDirs)
        {
            // track if there are active branches or if open directions

            if (branchDir == SideLogic.brPath)
            {
                #region BRANCH DIRECTION DEBUG STRINGS
                nbrTile = GetNbrInDir(dirCount);
                tileNbrPos = nbrTile.fullPos.ToString();
                string incPath = ("found open brDir " + dirCount + " to: " + tileNbrPos + "\n");
                string complPath = ("ChkBr found nbr: " + tileNbrPos + " in brDir " + 
                                dirCount + " to be complete! \n");
                #endregion

                // counts number of branches the tile has, excluding complete from options                
                if (!nbrTile.pathCompl)
                    branchDs.Add(dirCount);

                chkBrDB += !nbrTile.pathCompl ? incPath : complPath; 
            }
            else if (branchDir == SideLogic.opnSide)
            {
                #region WALL/OPEN LOGIC AND DEBUG STRINGS
                nbrTile = GetNbrInDir(dirCount);
                tileNbrPos = nbrTile.fullPos.ToString();
                bool nbrIsSol = nbrTile.isSolution;
                bool nbrIsInBr = nbrTile.isInBranch;
                bool nbrClosed = (nbrIsSol || nbrIsInBr);
                chkBrDB += ("Side: " + dirCount + "has nbrTile that has bool for Sol/InBr: " +
                            nbrIsSol + "/" + nbrIsInBr + "\n");
                string wallNbr = ("made a wall between this and tile: " + tileNbrPos + "\n");
                string openNbr = ("Tile: " + fullPos.ToString() + " has open nbr in: " +
                               tileNbrPos + "\n");
                #endregion

                // nbring Tile is in a solution or branch path, make a wall                  
                if (nbrClosed) 
                    this.SetSides(dirCount, SideLogic.fWall);
                else
                    branchOptions.Add(dirCount);

                chkBrDB += nbrClosed ? wallNbr : openNbr;
            }

            // counts new and old walls to check for deadend
            if (branchDir == SideLogic.fWall) 
                ++wallCount;

            ++dirCount;
        }

        // closes off the branch if there are no options
        if (this.isInBranch && branchOptions.Count == 0 && branchDs.Count == 0)
        {
            chkBrDB += ("this tile should be marked as compl \n");
            //Debug.LogWarning("Tile: " + fullPos.ToString() + " should be marked as compl");
            this.pathCompl = true;
            if (wallCount == 3) this.isDeadEnd = true;
        }

        chkBrDB += ("branchOptions.Count is: " + branchOptions.Count + "\n");
        chkBrDB += ("branchDs count is: " + branchDs.Count);
        //Debug.Log(chkBrDB);
    }





    #endregion

    public bool isInOuterEdges()
    {
        return fullPos.x == 0 || fullPos.x == GridDataGen.fullGrid.GetLength(0) - 1 || fullPos.y == 0 || fullPos.y == GridDataGen.fullGrid.GetLength(1) - 1;
    }
    public void PrepTile()
    {
        SideLogic[] tileSides = this.CheckSides();

        bool[] dirFace = new bool[4] {this.u, this.d, this.l, this.r };

        int indxDir = 0;
        foreach(SideLogic tileSide in tileSides)
        {
            bool needsFixed = false;
            // clean up remaining missing walls
            if (tileSide == SideLogic.opnSide)
                if (this.NeighborLog(indxDir) == SideLogic.fWall)
                {
                    //Debug.Log("Prep: needed to finish Tile: " + fullPos.ToString());
                    this.SetSides(indxDir, SideLogic.fWall);
                    needsFixed = true;
                }


            if (tileSide == SideLogic.opnSide || tileSide == SideLogic.solPath || 
                tileSide == SideLogic.brPath  || tileSide == SideLogic.brIn)
                dirFace[indxDir] = true;
            else if (tileSide == SideLogic.fWall || tileSide == SideLogic.stPath || needsFixed)
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
    public void PrepTile2()
    {
        float h = GridDataGen.fullGrid.GetLength(1);
        float w = GridDataGen.fullGrid.GetLength(0);

        //fullPos.y == h- 1 || upneighbor.d == wall

        if (!((fullPos.y != h - 1) && GetLogNeighbor(new Vector2Int(0, 1)).d))
            u = false;
        if (!((fullPos.y != 0) && GetLogNeighbor(new Vector2Int(0, -1)).u))
            d = false;
        if (!((fullPos.x != 0) && GetLogNeighbor(new Vector2Int(-1, 0)).r))
            l = false;
        if (!((fullPos.x != w - 1) && GetLogNeighbor(new Vector2Int(1, 0)).l))
            r = false;
        int count = 0;
        count += !u ? 1 : 0;
        count += !d ? 1 : 0;
        count += !l ? 1 : 0;
        count += !r ? 1 : 0;
        isDeadEnd = count == 3 && !isInOuterEdges();
        
    }

}