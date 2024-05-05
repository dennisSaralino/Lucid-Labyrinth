using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;

public class GridDataGen : MonoBehaviour
{
    public int xMaxSize = 5;  // number of main tiles across
    public int yMaxSize = 5;  // number of main tiles up
    public int fullGridSize;   // number of grid tiles including the outside edge
    public int edgesSize;      // number of edge tiles that border the main tiles, on start and end side
    public int mazeTilesSize;  // number of main tiles in the center
    public static alTData[,] fullGrid;  // 
    public alTData[] zeroEdges;
    public alTData[] maxEdges;
    public static alTData[,] mazeGrid;  // make a singleton set for tile referencing
    public static GridDataGen thisGrid;
    //List<alTData> fullGrid;

    public List<alTData> pathTiles;    // dynamically made list of tiles in the solutionPath

    #region PATHFINDING DATA FOR MOSTLY CreatePath
    public alTData startTile;  
    public alTData endTile;
    // Size controls
    public float st2endDistLimit;         // minimum limit of distance between the start and end tiles 
    public float minDistCheck = 4.0f;     // used to set range of minPathL checking against dist from endTile
    public float minPathL;
    public float maxPathL;
    public float endDistDiv  = 2.0f;      // keeps the end Distance about 1/3 of the map away at least
    public float minPathMult = 0.3f;      // makes the min PathL size
    public float mxPathMult  = 0.6f;
    public Vector2Int nullInDir;          // used to clear data when backtracking
    public static Vector2Int nullOutDir;  // used to clear data when backtracking
    #endregion

    public List<alTData> incompleteTiles;
    public List<alTData> allBrTiles;
    public const int brDepth = 3;    // default depth of making a branch before skipping to the next
    public static int noBranch = 4;                //  setup of a stop option for branch logic
    public string compLog;




    public void Awake()
    {
        if (thisGrid != null) { Destroy(gameObject); return; }
        thisGrid = this;
        //if (PlayerPrefs.HasKey("mazeSize"))
        //    xMaxSize = yMaxSize = PlayerPrefs.GetInt("mazeSize");
        StartCoroutine(GenGridData());
    }

    public void GenGrid()
    {
        #region GRID CONTAINERS SETUP
        fullGridSize = (xMaxSize + 2) * (yMaxSize + 2);
        fullGrid = new alTData[xMaxSize+2,yMaxSize+2];

        edgesSize = xMaxSize + yMaxSize;   // prep array size for zeroEdges and maxEdges
        zeroEdges = new alTData[edgesSize];
        maxEdges = new alTData[edgesSize];

        mazeTilesSize = xMaxSize * yMaxSize;
        mazeGrid = new alTData[xMaxSize,yMaxSize];

        int zEdge = 0;
        int mEdge = 0;

        // needed values for backtracking cleanup, keep aware of these
        nullInDir = new Vector2Int(0, 0);   // new Vector2Int(-1,-1)
        nullOutDir = new Vector2Int(xMaxSize + 1, yMaxSize + 1);
        #endregion

        // loop to fill grid
        for (int i = 0; i <= xMaxSize + 1; ++i)
        {
            for (int j = 0; j <= yMaxSize + 1; ++j)
            {

                #region GRID TILE CONSTRUCTION AND EDGE DECLARATION
                fullGrid[i,j] = new alTData(new Vector2Int(i, j));
                alTData thisTile = new alTData(new Vector2Int(i,j));
                fullGrid[i, j] = thisTile;

                // create easy logic handles for edges and borders
                bool leftEdge = (i < 1);
                bool rightEdge = (i > xMaxSize);
                bool bottomEdge = (j < 1);
                bool topEdge = (j > yMaxSize);
                bool outerEdges = (leftEdge || rightEdge || bottomEdge || topEdge);
                bool makeLwall = ((i == 1) || leftEdge || rightEdge);
                bool makeRwall = ((i == xMaxSize) || rightEdge || leftEdge);
                bool makeUwall = ((j == yMaxSize) || topEdge || bottomEdge);
                bool makeDwall = ((j == 1) || bottomEdge || topEdge);
                #endregion

                // store fullGrid values, noting outer edges to be walls and center open
                #region SET INITIAL TILE LOGIC STATES FOR fullGrid ============
                SideLogic tileWall = SideLogic.fWall;
                SideLogic tileOpen = SideLogic.opnSide;
                    
                thisTile.leftLog  = makeLwall ? tileWall : tileOpen;  // Left             
                thisTile.rightLog = makeRwall ? tileWall : tileOpen;  // Right               
                thisTile.downLog  = makeDwall ? tileWall : tileOpen;  // Down              
                thisTile.upLog    = makeUwall ? tileWall : tileOpen;  // Up
                #endregion

                // store potential start points to pick from
                #region START POINTS
                if ( (leftEdge   && !bottomEdge && !topEdge) || 
                     (bottomEdge && !leftEdge   && !rightEdge) )
                {
                    zeroEdges[zEdge] = fullGrid[i, j];
                    ++zEdge;
                }
                #endregion

                // store potential end points to pick from
                #region END POINTS
                if ( (rightEdge && !bottomEdge && !topEdge) ||
                     (topEdge   && !leftEdge   && !rightEdge) )
                {
                    maxEdges[mEdge] = fullGrid[i, j];
                    ++mEdge;
                }
                #endregion

                // store the central grid used for maze generation
                #region STORE CENTER GRID
                if ( !outerEdges )
                {
                    // fullGrid[1,1] is mazeGrid[0,0] and so on
                    thisTile.currentPos = new Vector2Int(i,j); // get Vector2Int
                    mazeGrid[i-1, j-1] = thisTile;   
                }
                #endregion
            }
        }
    }

    public IEnumerator CreatePath()    
    {

        pathTiles = new List<alTData>();

        // pick the start of the maze
        #region PICK START AND DO SETUP =======================================
        //print("picking start Tile");
        startTile = zeroEdges[Random.Range(0, edgesSize)];
        startTile.isStartT = true; // store the value for later logic
        
        // Start tile on Left Edge
        if(startTile.fullPos.x == 0) 
        {
            // setup tile for which side is opening
            startTile.upLog = SideLogic.fWall;
            startTile.downLog = SideLogic.fWall;
            startTile.rightLog = SideLogic.solPath;

            // note path directions of itself and neighbor
            alTData rLogNbr = startTile.GetLogNeighbor(Vector2Int.right);
            startTile.outdir = rLogNbr.fullPos;
            rLogNbr.leftLog = SideLogic.solPath;
            rLogNbr.indir = startTile.fullPos;
        }
        
        // start tile on Bottom Edge
        if(startTile.fullPos.y == 0)
        {
            // setup tile for which side is opening
            startTile.upLog = SideLogic.solPath;
            startTile.leftLog = SideLogic.fWall;
            startTile.rightLog = SideLogic.fWall;

            // note path directions of itself and neighbor
            startTile.outdir = startTile.GetLogNeighbor(Vector2Int.up).fullPos;
            startTile.GetLogNeighbor(Vector2Int.up).downLog = SideLogic.solPath;
            startTile.GetLogNeighbor(Vector2Int.up).indir = startTile.fullPos;
        }
        #endregion ============================================================
        
        startTile.isSolution = true;

        pathTiles.Add(startTile);

        // prep the next path tile to start for looping
        alTData currTile = fullGrid[startTile.outdir.x, startTile.outdir.y];

        // pick the end of the maze
        #region PICK END AND DO SETUP =========================================

        st2endDistLimit = (edgesSize / endDistDiv) + 1;

        List<alTData> bestEnds = new();
        foreach(alTData maxEdge in maxEdges)
        {
            if (TileDistCheck(maxEdge.fullPos, startTile.fullPos) > st2endDistLimit)
                bestEnds.Add(maxEdge);
        }

        
        endTile = bestEnds[Random.Range(0, bestEnds.Count)];
        // reroll to another end if the distance is too close to the start
        while (TileDistCheck(startTile.fullPos, endTile.fullPos) < st2endDistLimit+1)
        {
            Debug.Log("endTile required a Reroll");
            endTile = bestEnds[Random.Range(0, bestEnds.Count)];
            //endTile = maxEdges[Random.Range(0, edgesSize)];
        }  
        endTile.isEndT = true;

        // End tile on the Right Edge
        if(endTile.fullPos.x == xMaxSize + 1) 
        {
            // setup tile for which side is opening
            endTile.upLog = SideLogic.fWall;
            endTile.downLog = SideLogic.fWall;  // to close down door
            endTile.leftLog = SideLogic.solPath;

            // note path directions of itself and neighbor
            endTile.indir = endTile.GetLogNeighbor(Vector2Int.left).fullPos;
            endTile.GetLogNeighbor(Vector2Int.left).rightLog = SideLogic.opnSide;//solPath;
            endTile.GetLogNeighbor(Vector2Int.left).outdir = endTile.fullPos;
        }

        // End tile on Top Edge
        if(endTile.fullPos.y == yMaxSize+1)
        {
            // setup tile for which side is opening
            endTile.downLog = SideLogic.solPath;
            endTile.leftLog = SideLogic.fWall;  // closes deft door
            endTile.rightLog = SideLogic.fWall;

            // note path directions of itself and neighbor
            endTile.indir = endTile.GetLogNeighbor(Vector2Int.down).fullPos;
            endTile.GetLogNeighbor(Vector2Int.down).upLog = SideLogic.opnSide;//solPath;
            endTile.GetLogNeighbor(Vector2Int.down).outdir = endTile.fullPos;
        }

        #endregion ============================================================


        #region DEFINE PATH LIMITS ============================================
        int strtX = startTile.fullPos.x;
        int strtY = startTile.fullPos.y;
        int endX  = endTile.fullPos.x;
        int endY  = endTile.fullPos.y;

        minPathL = mazeTilesSize * minPathMult;
        maxPathL = (mazeTilesSize * mxPathMult);
        // allows for the start and end tile to be in good length range
        #endregion ============================================================

        // looping solution path algorithm
        #region FIND SOLUTION PATH ============================================
        while (currTile != endTile)
        {

            //Debug.Log("Starting string for currTile: " + currTile.fullPos.ToString());
            string CrPathDB = ("Post while loop Summary for currTile: " +
                                      currTile.fullPos.ToString() + "\n");
            #region DISTANCE FROM END TRACKING ================================
            string tileCoords = currTile.fullPos.ToString();
            float distFromEnd = TileDistCheck(currTile.fullPos, endTile.fullPos);

            bool pathSizeLimit = (maxPathL < pathTiles.Count);
            if (pathSizeLimit)
            {
                CrPathDB += ("path limit was reached at: " + tileCoords + "\n");
                CrPathDB += CollectPath(pathTiles);
            }

            bool pathTooSmall = ((minPathL > pathTiles.Count) && 
                                 (distFromEnd < minDistCheck));
            if (pathTooSmall)
            {
                CrPathDB += ("path was too small at: " + pathTiles.Count + " tiles:" +
                             tileCoords + "\n");
                CrPathDB += CollectPath(pathTiles);
            }

            bool tooFar = ((maxPathL - pathTiles.Count) < distFromEnd);
            if (tooFar)
            {
                CrPathDB += ("path is getting tooFar from the end at Tile: " +
                             tileCoords + "\n");
                CrPathDB += ("path is: " + pathTiles.Count + " and currTile is: " +
                             distFromEnd + " tiles from End \n");

                CrPathDB += CollectPath(pathTiles);
            }
            #endregion

            // check current tile status
            //sideLogic[] tileFaces = new sideLogic[4];
            //tileFaces[0] = currTile.upLog;
            //tileFaces[1] = currTile.downLog;
            //tileFaces[2] = currTile.leftLog;
            //tileFaces[3] = currTile.rightLog;

            SideLogic[] tileFaces = currTile.CheckSides();
            List<int> remainFaces = new();


            // use the status to determine amount of options
            #region DETERMINE PATH OPTIONS ====================================
            int pOptions = 0;
            int currSide = 0;  // track if {up, down, left, right}
            foreach (SideLogic tileFace in tileFaces)
            {

                // allow open sides // and stopped paths to be checked, stopped paths branches only
                if ((tileFace == SideLogic.opnSide) || (tileFace == SideLogic.altPath))
                {
                    #region LOGIC VALUES SETUP ================================
                    int currX = currTile.fullPos.x;
                    int currY = currTile.fullPos.y;

                    bool checkUp    = (currSide == 0);  //tileFace == currTile.upLog);
                    bool checkDown  = (currSide == 1);  //tileFace == currTile.downLog);
                    bool checkLeft  = (currSide == 2);  // tileFace == currTile.leftLog);
                    bool checkRight = (currSide == 3);  //tileFace == currTile.rightLog);
                    bool dNbrInPath = currTile.NeighborLog(currSide) == SideLogic.altPath;
                    bool dNbrSolTile = currTile.GetNbrInDir(currSide).isSolution;

                    if (dNbrSolTile && !dNbrInPath)
                        CrPathDB += ("____dNbrInPath has shown to be unreliable_____");
                    #endregion

                    #region DETERMINE DEADEND RESULTANT OPTIONS ON EDGES ======
                    // left edge's y is greater than start on down check,
                    //                  or less than start on up check
                    bool leftEdgeLimit = ((currX == 1) && 
                                           ((checkDown && (currY > strtY)) ||
                                              (checkUp && (currY < strtY))));

                    // right edge's y is less than end on down check,
                    //             or greater than end on up check
                    bool rightEdgeLimit = ((currX == xMaxSize) &&
                                            ((checkDown && (currY < endY)) ||
                                               (checkUp && (currY > endY))));

                    // bottom edge's x is greater than start on left check,
                    //                    or less than start on right check
                    bool bottomEdgeLimit = ((currY == 1) &&
                                             ((checkLeft  && (currX > strtX)) || 
                                              (checkRight && (currX < strtX))));

                    // top edge's x is less than end on left check,
                    //           or greater than end on right check
                    bool topEdgeLimit = ((currY == yMaxSize) &&
                                          ((checkLeft  && (currX < endX)) || 
                                           (checkRight && (currX > endX))));

                    // make cases for when next to or below the endTile
                    bool nextToEnd = (((currX == xMaxSize) && (currY == endY)) &&
                                       (checkUp || checkDown || checkLeft));

                    bool belowTheEnd = (((currY == yMaxSize) && (currX == endX)) &&
                                        (checkDown || checkLeft || checkRight));

                    bool outerEdgeLimit = (leftEdgeLimit   || rightEdgeLimit ||
                                           bottomEdgeLimit || topEdgeLimit);
                    #endregion

                    // remove options which deadend or increment available options
                    if (outerEdgeLimit || nextToEnd || belowTheEnd ||
                        pathSizeLimit || pathTooSmall || tooFar)
                    {
                        // ensure length limits do not stop necessary wall placement
                        if (dNbrSolTile) //dNbrInPath) // direction nbr is a pathtile covering
                            currTile.SetSides(currSide, SideLogic.fWall);
                        else
                            currTile.SetSides(currSide, SideLogic.stPath);
                    }
                    else if (dNbrSolTile) //dNbrInPath) // direction nbr is a pathtile
                        currTile.SetSides(currSide, SideLogic.fWall);
                    else
                    {
                        CrPathDB += ("Adding: " + currTile.GetNbrInDir(currSide).fullPos.ToString() +
                            " to pathOptions for: " + tileCoords + "\n");
                        remainFaces.Add(currSide);
                        ++pOptions;
                    }

                }

                ++currSide;
            }
            #endregion

            CrPathDB += currTile.CollectSides();
            CrPathDB += ("Deciding for Tile: " + currTile.fullPos.ToString() + 
                " will be pathTile #: " + pathTiles.Count + "\n");
            // backtrack to previous tile if the tile has no options. 
            if (pOptions == 0)
            {
                //Debug.Log("Backtrack option taken:");
                CrPathDB += ("Backtracking from: " + currTile.fullPos.ToString() + "\n");
                alTData ignoreTile = currTile;
                CrPathDB += CollectPath(pathTiles);
                CrPathDB += ("(PP) previous end of pathTiles is: " + 
                             pathTiles[pathTiles.Count - 1] + "\n");

                // set the last tile on path to the current tile
                currTile = pathTiles[pathTiles.Count-1];

                CrPathDB += ("Backtracked from: "+ ignoreTile.fullPos.ToString()+ 
                            " to: " + currTile.fullPos.ToString() + "\n");

                #region BLOCK DIRECTION CHOICE ================================
                // remove data or make a wall of some sort.                
                currTile.SetSides(ignoreTile.BorderSide(currTile), SideLogic.stPath);
                currTile.outdir = nullOutDir;

                if (pathTiles.IndexOf(currTile) != pathTiles.LastIndexOf(currTile))
                    Debug.LogWarning("Tile " + currTile.fullPos.ToString() + " is duplicated or missing!");
                pathTiles.Remove(currTile);    // remove from pathTiles

                CrPathDB += ("after currTile cleanup " + currTile.CollectSides());
                CrPathDB += CollectPath(pathTiles);
                //ignoreTile.indir = nullInDir;  // remove indir
                ignoreTile.ResetTile(xMaxSize, yMaxSize, endX, endY);

                CrPathDB += ("after ignoreTile reset " + ignoreTile.CollectSides());
                #endregion
            }
            else
            {
                //Debug.Log("Choosing an option: ");
                #region PICK NEXT TILE ========================================
                // pick a random direction from options
                int dirChoice = remainFaces[Random.Range(0, remainFaces.Count)];

                alTData tempTile = currTile; // allows currTile to be updated
                alTData nextTile;
                
                //SideLogic[] tileFaces = currTile.CheckSides();  //tileFaces
                int sideNum = 0;
                // determine which direction was the chosen one
                // and set it up as the next tile.
                foreach(SideLogic tileFace in tileFaces)
                {
                    if(dirChoice == sideNum)
                    {
                        if (currTile.GetNbrInDir(sideNum).isSolution)
                            Debug.LogError("Tried to select a direction that loops on path");
                        
                        // set outdir, solPath, nextTile, indir, and pathTile
                        nextTile = currTile.SetNextTile(dirChoice,
                                                SideLogic.solPath, SideLogic.solPath);
                        CrPathDB += ("currTile added: " + currTile.fullPos.ToString() + "\n");
                        pathTiles.Add(currTile);
                        currTile.isSolution = true;  // testing potentially remove
                        currTile = nextTile;

                    }                     
                    else if (tileFace == SideLogic.opnSide)
                    {
                        CrPathDB += ("setting tile: " + tempTile.GetNbrInDir(sideNum).fullPos.ToString() + " to altOption \n");
                        tempTile.SetSides(sideNum, SideLogic.altPath);
                    }

                    ++sideNum;

                }
                #endregion
                CrPathDB += ("tile after (else) foreach loop: " + currTile.fullPos.ToString() +
                    "\n" + currTile.CollectSides());
                //Debug.Log("tile after (else) foreach loop: " + currTile.fullPos.ToString());
                CrPathDB += CollectPath(pathTiles);
            }
            //Debug.Log(CrPathDB);
            compLog += (CrPathDB + "\n ===================== \n");
            //yield return null;
        }
        #endregion

        pathTiles.Add(endTile);
        //Debug.Log("Solution Path has been generated");

        string pathlist = "solution values: ";
        int currTileIndx = 0;
        #region PATH LOGIC CLEANUP
        foreach (alTData pathTile in pathTiles)
        {

            #region MAKE pathTiles READY FOR BRANCHING
            SideLogic[] solSides = pathTile.CheckSides();
            int sideIndex = 0;
            foreach (SideLogic solSide in solSides)
            {
                // do a check to avoid doing the inside logic check
                // unnecessarily or on tiles which lack a neighbor 
                if( !((solSide == SideLogic.fWall) ||
                      (solSide == SideLogic.solPath)))
                {
                    // finalize walls where adjacent to other parts of path
                    // otherwise open up tilefaces for branches.
                    if (pathTile.NeighborLog(sideIndex) == SideLogic.altPath ||
                        pathTile.NeighborLog(sideIndex) == SideLogic.fWall)
                    {
                        pathTile.SetSides(sideIndex, SideLogic.fWall);
                    }
                    else
                        pathTile.SetSides(sideIndex, SideLogic.opnSide);
                }
                ++sideIndex;
            }
            #endregion

            // set the indir of each pathTile now that they are final
            if (currTileIndx != 0)
                pathTile.indir = pathTiles[currTileIndx - 1].fullPos;

            pathTile.PrepTile();

            pathTile.isSolution = true;
            pathlist += pathTile.fullPos.ToString();
            //print("(" + pathTile.fullPos.x + ", " + pathTile.fullPos.y + "), ");

            ++currTileIndx;
        }
        #endregion
        //print(pathlist);

        yield return null;
    }

    public IEnumerator CreateBranches(List<alTData> pathTs, int branchDepth = brDepth)  //alTdata root = startTile
    {
        int currBranchDepth = 0;  // 
        bool needToPush = false;  // for when all branches end but grid not filled

        while((incompleteTiles.Count !=0) && branchDepth< maxPathL)
        {
            // stops the potential of skipping branching for a tile
            if (branchDepth == 7) needToPush = true;

            int solTileNum = 0;
            foreach(alTData pathT in pathTs)
            {
                alTData.BranchOpts allOpts = pathT.GetBrOpts();
                // check if there are any open branch options or current branches for the pathTile
                List<int> pthBrOpts = allOpts.nwBrOpts;
                List<int> pthBrDirs = allOpts.currBrs;
                bool noBranchOpts = (pthBrOpts.Count < 1 && pthBrDirs.Count == 0);

                // skip to next tile if no option to branch and no current active branches
                if(noBranchOpts)
                {
                    //Debug.Log("CrBranches moving to next currTile from: " + pathT.fullPos.ToString());
                }
                else
                {
                    alTData rootT = pathT;
                    yield return StartCoroutine(BuildBranch(rootT, pathT, branchDepth, 
                                       currBranchDepth, needToPush, allOpts));
                }
                //Debug.Log("CrBranches:foreach loop finished with solTile: " + solTileNum);
                ++solTileNum;
            }
            ++branchDepth;

            TilesFilled();
            //Debug.Log("TilesFilled updated to be size: " + incompleteTiles.Count);
            string solTile = ("CrBranches after foreach loop solTile is: " + solTileNum);
            //Debug.Log(solTile + " at branchDepth: " + branchDepth);
            //yield return null;
            
            // should allow one final runthrough after completion of tiles to do cleanup
            if (incompleteTiles.Count == 0) branchDepth = (int)maxPathL-1;
        }
        yield return null;
    }

    // static

    public IEnumerator BuildBranch(alTData rootTile, alTData currTile, int totalDepth, 
                                   int currDepth, bool pushThru, alTData.BranchOpts rootOpts)    
    {
        string debugBldBr = ("Starting BldBranch for currTile: " + currTile.fullPos.ToString() + "\n");
        bool skipBranching = false;
        //alTData brRoot = currTile;

        alTData.BranchOpts tempOpts = rootOpts;
        int whileCount = 0;
        while ((currDepth < totalDepth) && !skipBranching)
        {
            #region BRANCH PREP AND OPTION STORAGE
            List<int> rtBrOpts = tempOpts.nwBrOpts; // optional directions
            List<int> rtBrDirs = tempOpts.currBrs;  // already branch directions
            bool noOptions = (rtBrOpts.Count == 0 && rtBrDirs.Count == 0);
            #endregion

            debugBldBr += ("at start of BldBranch while loop number: " + whileCount + "\n");
            alTData tempRtTile = currTile;


            if (noOptions) // if this tile has no options
            {
                debugBldBr += ("BldBranch: (noOptions) entered \n");
                // for closing out a branch that dead ends
                // if both itself and the root are in the branch, complete the tile
                if (currTile.isInBranch && rootTile.isInBranch && currTile!=rootTile)
                {
                    if (currTile.pathCompl != true) 
                        Debug.LogWarning("BldBr: noOpt, should have already been set to compl by chkBr");
                    currTile.pathCompl = true; // consider stop options, potentially use push
                    currTile = rootTile; //skipBranching = true;
                    tempOpts = currTile.GetBrOpts();
                    currDepth = currTile.tileDepth;
                    Debug.Log("BldBranch (noptions, curr+root inBr) reached a deadend " +
                        "within a branch at currTile: " + currTile.fullPos.ToString() + "\n");
                }
                else
                {
                    debugBldBr += ("BldBranch: (nooptions) should close this branch, as it " +
                        "has been completed. currTile's root is: " + rootTile.fullPos.ToString() + "\n");
                    skipBranching = true;  // temporarty
                }
            }
            // traverse an active branch
            else if (rtBrDirs.Count > 0)
            {
                debugBldBr += ("BldBranch: (brDir > 0) the currTile is: " + currTile.fullPos.ToString() + 
                    "at depth " + currDepth + "\n");

                int layerDepth = currDepth + 1;
                // traverse each branch and look for potential branching options
                if(currTile.outdir != nullOutDir && currTile.outBranches.Count == 0)
                {
                    alTData tempNxtTile;

                    // peek ahead at nextTile to see if it is complete
                    Vector2Int gridCoord = currTile.outdir;
                    tempNxtTile = fullGrid[gridCoord.x, gridCoord.y];
                    alTData.BranchOpts nextOpts = tempNxtTile.GetBrOpts();

                    #region OUTDIR BRANCH OR TRAVERSE
                    // if complete stay here and attempt to branch
                    if (tempNxtTile.pathCompl && !tempRtTile.isSolution)
                    {

                        debugBldBr += ("BldBranch: Entering potential nested recursive call into layer: " +
                                         layerDepth + "\n");
                        tempOpts = currTile.GetBrOpts();

                        // if potential for a new branch recursively call
                        if (tempOpts.nwBrOpts.Count > 0)
                        {
                            //Debug.LogWarning("BldBranch: Entering nested rc call into layer: " + layerDepth);
                            yield return StartCoroutine(BuildBranch(tempRtTile, currTile, totalDepth,
                                                                 layerDepth, pushThru, tempOpts));
                            debugBldBr += ("BldBranch: Exiting the recursive call \n");
                        }
                        else
                            debugBldBr += ("skipped OutDir recursion"); 
                    }
                    // else traverse the up the branch
                    else
                    {
                        // move to next tile in branch
                        currTile = tempNxtTile;
                        tempOpts = nextOpts;
                        debugBldBr += ("BldBranch: continue to traverse the branch as currTile: " +
                                      currTile.fullPos.ToString() + "\n");
                        ++currDepth;
                    }
                    #endregion

                    debugBldBr += ("inside BldBranches: rtBrDirs > 0, resultant currTile: " +
                                   currTile.fullPos.ToString() + "\n");
                }
                else
                {
                    int brDirCount = 0;
                    foreach (Vector2Int outBranch in currTile.outBranches)
                    {

                        alTData tempNxtTile;
                        debugBldBr += ("BldBranch: about to get brDirCount of: " +
                                    brDirCount + " for brRoot to find the coords \n");
                        debugBldBr += ("BldBranch outbranches has a size of: " +
                                    currTile.outBranches.Count + "\n");

                        // peek ahead at nextTile to see if it is complete
                        Vector2Int gridCoord = tempRtTile.outBranches[brDirCount]; //currTile
                        tempNxtTile = fullGrid[gridCoord.x, gridCoord.y];
                        alTData.BranchOpts nextOpts = tempNxtTile.GetBrOpts();

                        #region OUTBRANCH BRANCH OR TRAVERSE
                        // if complete stay here and attempt to branch
                        if (tempNxtTile.pathCompl && !tempRtTile.isSolution)
                        {

                            debugBldBr += ("BldBranch: Entering potential nested recursive call into layer: " +
                                            layerDepth + "\n");
                            tempOpts = currTile.GetBrOpts();

                            // if potential for new branch recursively call
                            if (tempOpts.nwBrOpts.Count > 0)
                            {
                                Debug.LogWarning("BldBranch: Entering nested rc call into layer: " + layerDepth);
                                debugBldBr += ("BldBranch: original root is: " + rootTile.fullPos.ToString() +
                                              " current root is: " + tempRtTile.fullPos.ToString() + "\n");

                                //bool tempPush = true;  // pushing through if needed. 

                                //yield return StartCoroutine(BuildBranch(tempRtTile, currTile, totalDepth,
                                //                                    layerDepth, tempPush, tempOpts));
                                debugBldBr += ("BldBranch: Exiting the recursive call \n");
                            }
                            else
                                debugBldBr += ("Skipped outbranch recursion");
                        }
                        // else traverse up that branch
                        else
                        {
                            // move to next tile in branch
                            currTile = tempNxtTile;
                            tempOpts = nextOpts;
                            debugBldBr += ("BldBranch: continue to traverse the branch as currTile: " +
                                          currTile.fullPos.ToString() + "\n");
                            ++currDepth;
                        }
                        #endregion

                        debugBldBr += ("inside BldBranches: rtBrDirs > 0, resultant currTile: " +
                                       currTile.fullPos.ToString() + "\n");
                        ++brDirCount;
                    }
                }

                
            }
            // choose to make a branchTile or not
            else
            {
                // pick an option from openSides or to stop
                if (!pushThru) rtBrOpts.Add(noBranch);
                
                int dirPicked = rtBrOpts[Random.Range(0, rtBrOpts.Count)];

                // if an actual option chosen, make a branch
                if (dirPicked != noBranch)
                {
                    // if the Tile isn't branching yet make sure it is set up and flagged
                    if (!tempRtTile.isBranching)
                    {
                        //tempRtTile.branchTiles = new();

                        // check if the currTile is in a branch and if it has an out already
                        bool readyForBr2 = false;
                        if (currTile.outdir != nullOutDir && tempRtTile.isInBranch)
                            readyForBr2 = true;
                                
                        debugBldBr += ("BldBranch made first branch for currTile: " +
                                       tempRtTile.fullPos.ToString() + "\n");

                        // note and flag a new outbranch
                        if (tempRtTile.isSolution || (readyForBr2)) //tempRtTile.outBranches.Count > 0))
                        {
                            tempRtTile.isBranching = true; //currTile.isBranching = true;
                            debugBldBr += ("BldBranch: currTile/tempRtTile: " + 
                                currTile.fullPos.ToString() + " recieved isBranching \n");
                        }
                    }
                    else
                    {
                        // else note that another branch is made
                        string secondBranch = ("BldBranch Picked a new direction from a tile that was" +
                            " already branching at currTile: " + currTile.fullPos.ToString());
                        //Debug.LogWarning(secondBranch);
                        debugBldBr += secondBranch;
                    }

                    debugBldBr += ("BldBranch making branch \n");
                    ++currDepth;
                    currTile = tempRtTile.MakeBranch(dirPicked, currDepth);
                    if (tempRtTile.isInBranch)
                        rootTile.branchTiles.Add(currTile);

                    tempOpts = currTile.GetBrOpts();

                    debugBldBr += CollectPath(rootTile.branchTiles) +"\n";
                }
                // skip Branching
                else
                {
                    // Picked to not Branch
                    skipBranching = true;

                    #region DEBUG STRINGS FOR EXIT
                    string pathCont = ("BldBranch:(currT solution) moving to next pathTile from " +
                            "currTile: " + currTile.fullPos.ToString() + "\n");
                    string leaveBr = ("BldBranch:(else) leaving currTile: " +
                            currTile.fullPos.ToString() + " and returning to solPath\n");

                    debugBldBr += currTile.isSolution ? pathCont : leaveBr;
                    #endregion
                }
            }

            //if(brRoot.isBranching || )
            //secondPass = true;
            debugBldBr += ("BldBranch end of while loop, curr count is" + whileCount + "\n");
            debugBldBr += ("=================================================\n");
            ++whileCount;

            //yield return null;
        }

        //PrintPath(rootTile.branchTiles);
        debugBldBr += ("Exiting BldBranch at layer " + currDepth + " and currTile: " +currTile.fullPos.ToString());
        //Debug.Log(debugBldBr);
        compLog += (debugBldBr + "\n======================\n");
        yield return null;
    }
    

    public void PrintPath(List<alTData> pPathTs)
    {
        string pPathTList = "curr path: ";
        foreach(alTData pPathT in pPathTs)
        {
            pPathTList += pPathT.fullPos.ToString();
        }
        print(pPathTList);
    }

    public string CollectPath(List<alTData> pPathTs)
    {
        string pPathTList = "curr path: ";
        foreach (alTData pPathT in pPathTs)
        {
            pPathTList += pPathT.fullPos.ToString();
        }

        return (pPathTList + "\n");
    }

    public void TrackIncompleteTiles()
    {
        incompleteTiles = new();
        allBrTiles = new();
        // go through central grid
        foreach(alTData mzTile in mazeGrid)
        {
            // check if current tile isn't used yet
            if ( !(mzTile.isSolution || mzTile.isBranching ||
                   mzTile.isInBranch || mzTile.isDeadEnd))
            {
                incompleteTiles.Add(mzTile);
            }
        }
    }

    public void TilesFilled()
    {
        List<alTData> tempTiles = new(); 
        // go through incompleteTiles
        foreach (alTData incompleteTile in incompleteTiles)
        {
            // check if each tile is either in the solution or part of a branch
            if (incompleteTile.isSolution || incompleteTile.isBranching || 
                incompleteTile.isInBranch || incompleteTile.isDeadEnd)
            {
                // remove it
                //Debug.Log("removing tile from incompleteTile list" + incompleteTile.currentPos.ToString() +
                //    " aka " + incompleteTile.fullPos.ToString());

                if (incompleteTile.isInBranch)
                {
                    allBrTiles.Add(incompleteTile);
                    tempTiles.Add(incompleteTile);
                }
                //incompleteTiles.Remove(incompleteTile);
            }
        }

        foreach (alTData tempTile in tempTiles)
        {
            incompleteTiles.Remove(tempTile);
        }        
    }

    public void FullCleanUp()
    {
        //Debug.Log("entering fullCleanup");
        //// loop to fill grid
        for (int i = 0; i <= xMaxSize + 1; ++i)
        {
            for (int j = 0; j <= yMaxSize + 1; ++j)
            {
                fullGrid[i, j].PrepTile();
            }
        }
        for (int i = 0; i <= xMaxSize + 1; ++i)
        {
            for (int j = 0; j <= yMaxSize + 1; ++j)
            {
                fullGrid[i, j].PrepTile2();
            }
        }


        //Debug.Log("fullCleanUp complete");
    }

    /// <summary>
    /// Check minimum Tile Distance away between two tiles 
    /// </summary>
    /// <param name="destTileV"></param>
    /// <param name="currTileV"></param>
    /// <returns> number of Tiles away including this </returns>
    public int TileDistCheck(Vector2Int currTileV, Vector2Int destTileV)
    {
        int destX = destTileV.x, destY = destTileV.y;
        int currX = currTileV.x, currY = currTileV.y;

        int distAway = Mathf.Abs(destX - currX) + Mathf.Abs(destY - currY) + 1;

        return distAway;
    }

    public IEnumerator GenGridData()    
    {
        GenGrid();
        yield return null;  // update loading progress bar
        yield return StartCoroutine(CreatePath());
        yield return null;  // update loading progress bar
        TrackIncompleteTiles();
        yield return null;  // update loading progress bar
        yield return StartCoroutine(CreateBranches(pathTiles));  // incomplete
        FullCleanUp();
        StopAllCoroutines();
        DataToMaze.i.dataToMaze(AlTConverter.convertToTiledata(new GridData(fullGrid, pathTiles)));
        //Debug.Log("Compiled Debugs from GenPath and BuildBranches: \n" + compLog +
        //     "=========== End of Log ========================");
    }
}


public class GridData
{
    public List<alTData> solution;
    public alTData[,] data;
    public GridData(alTData[,] al, List<alTData> solution)
    {
        this.data = al;
        this.solution = solution;
        for (int i = 0; i < solution.Count; i++)
        {
            solution[i].solutionIndex = i;
        }

    }
}