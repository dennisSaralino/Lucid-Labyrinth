using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;

public class GridDataGen : MonoBehaviour
{
    public int xMaxSize = 12;  // number of main tiles across
    public int yMaxSize = 12;  // number of main tiles up
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
    public float endDistMult = 0.18f;
    public float endDistDiv   = 2.5f;     // keeps the end Distance about 1/3 of the map away at least
    public float minDistCheck = 2.5f;     // used to set range of minPathL checking against dist from endTile
    public float minPathL;
    public float maxPathL;
    public float minPathMult = 0.3f;      // makes the min PathL size
    //public float minPathDiv = 5.0f;
    public float mxPathMult = 0.55f;
    //public float mxPathDiv = 2.0f;
    public Vector2Int nullInDir;     // used to clear data when backtracking
    public static Vector2Int nullOutDir;    // used to clear data when backtracking
    #endregion

    public List<alTData> incompleteTiles;
    public List<alTData> allBrTiles;
    public const int brDepth = 3;    // default depth of making a branch before skipping to the next
    public static int noBranch = 4;                //  setup of a stop option for branch logic

    public void Awake()
    {
        if (thisGrid != null) { Destroy(gameObject); return; }
        thisGrid = this;
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
                //bool makeUwall = ((j == yMaxSize) || topEdge || bottomEdge);
                //bool makeDwall = ((j == 1) || bottomEdge || topEdge);
                #endregion

                // store fullGrid values, noting outer edges to be walls and center open
                #region SET INITIAL TILE LOGIC STATES FOR fullGrid
                // Left
                if ((i == 1) || leftEdge || rightEdge)
                    fullGrid[i, j].leftLog = SideLogic.fWall; 
                else
                    fullGrid[i, j].leftLog = SideLogic.opnSide;
                //thisTile.leftLog = makeLwall ? SideLogic.fWall : SideLogic.opnSide;

                // Right
                if (makeRwall) //(i == xMaxSize) || rightEdge || leftEdge)
                    fullGrid[i, j].rightLog = SideLogic.fWall; 
                else 
                    fullGrid[i, j].rightLog = SideLogic.opnSide;
                
                // Down
                if ((j == 1) || bottomEdge || topEdge)
                    fullGrid[i, j].downLog = SideLogic.fWall;        
                else 
                    fullGrid[i, j].downLog = SideLogic.opnSide;
                
                // Up
                if ((j == yMaxSize) || topEdge || bottomEdge) 
                    fullGrid[i, j].upLog = SideLogic.fWall; 
                else 
                    fullGrid[i, j].upLog = SideLogic.opnSide;

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
                    thisTile.currentPos = fullGrid[i-1, j-1].fullPos; // get Vector2Int
                    mazeGrid[i-1, j-1] = thisTile;   
                }
                #endregion
            }
        }

        // needed values for backtracking cleanup, keep aware of these
        nullInDir = fullGrid[0, 0].fullPos;   // new Vector2Int(-1,-1)
        nullOutDir = fullGrid[xMaxSize + 1, yMaxSize + 1].fullPos;
    }

    public IEnumerator CreatePath()    
    {

        pathTiles = new List<alTData>();

        // pick the start of the maze
        #region PICK START AND DO SETUP
        print("picking start Tile");
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
        #endregion
        
        startTile.isSolution = true;
        pathTiles.Add(startTile);

        // prep the next path tile to start for looping
        alTData currTile = fullGrid[startTile.outdir.x, startTile.outdir.y];

        // pick the end of the maze
        #region PICK END AND DO SETUP

        //st2endDistLimit = mazeTilesSize * endDistMult;
        st2endDistLimit = edgesSize / endDistDiv;      // PathDiv;

        endTile = maxEdges[Random.Range(0, edgesSize)];
        // reroll to another end if the distance is too close to the start
        while (Vector2Int.Distance(startTile.fullPos, endTile.fullPos) < st2endDistLimit)
        {
            Debug.Log("endTile required a Reroll");
            endTile = maxEdges[Random.Range(0, edgesSize)];
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

        #endregion


        #region DEFINE PATH LIMITS
        int strtX = startTile.fullPos.x;
        int strtY = startTile.fullPos.y;
        int endX  = endTile.fullPos.x;
        int endY  = endTile.fullPos.y;

        minPathL = mazeTilesSize * minPathMult;  // / minPathDiv;
        maxPathL = (mazeTilesSize * mxPathMult)  + minDistCheck; // / mxPathDiv)
        // allows for the start and end tile to be in good length range
        #endregion

        //yield return null;
        // looping solution path algorithm
        #region FIND SOLUTION PATH
        while (currTile != endTile)
        {
            #region DISTANCE FROM END TRACKING
            string tileCoords = currTile.fullPos.ToString();
            float distFromEnd = Vector2Int.Distance(currTile.fullPos, endTile.fullPos);

            bool pathSizeLimit = (maxPathL < pathTiles.Count);
            if (pathSizeLimit)
            {
                Debug.Log("path limit was reached at: " + tileCoords);
                PrintPath(pathTiles);
            }

            bool pathTooSmall = ((minPathL > pathTiles.Count) && (distFromEnd < minDistCheck));
            if (pathTooSmall)
            {
                Debug.Log("path was too small at: " + pathTiles.Count + " tiles:" + tileCoords);
                PrintPath(pathTiles);
            }

            bool tooFar = ((maxPathL - pathTiles.Count) < distFromEnd);
            if (tooFar)
            {
                Debug.Log("path is getting tooFar from the end at Tile: " + tileCoords);
                Debug.Log("path is: " + pathTiles.Count + " and currTile is: " + distFromEnd + " tiles from End");

                PrintPath(pathTiles);
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
            #region DETERMINE PATH OPTIONS
            int pOptions = 0;
            int currSide = 0;  // track if {up, down, left, right}
            foreach (SideLogic tileFace in tileFaces)
            {

                // allow open sides // and stopped paths to be checked, stopped paths branches only
                if ((tileFace == SideLogic.opnSide) || (tileFace == SideLogic.altPath))
                {
                    #region LOGIC VALUES SETUP
                    int currX = currTile.fullPos.x;
                    int currY = currTile.fullPos.y;

                    bool checkUp    = (currSide == 0);  //tileFace == currTile.upLog);
                    bool checkDown  = (currSide == 1);  //tileFace == currTile.downLog);
                    bool checkLeft  = (currSide == 2);  // tileFace == currTile.leftLog);
                    bool checkRight = (currSide == 3);  //tileFace == currTile.rightLog);
                    #endregion

                    #region DETERMINE DEADEND RESULTANT OPTIONS ON EDGES
                    // left edge's y is greater than start on down check,
                    //                  or less than start on up check
                    bool leftEdgeLimit = ((currX == 1) && 
                        ((checkDown && (currY > strtY)) || (checkUp && (currY < strtY))));

                    // right edge's y is less than end on down check,
                    //             or greater than end on up check
                    bool rightEdgeLimit = ((currX == xMaxSize) &&
                        ((checkDown && (currY < endY)) || (checkUp && (currY > endY))));

                    // bottom edge's x is greater than start on left check,
                    //                    or less than start on right check
                    bool bottomEdgeLimit = ((currY == 1) &&
                        ((checkLeft && (currX > strtX)) || (checkRight && (currX < strtX))));

                    // top edge's x is less than end on left check,
                    //           or greater than end on right check
                    bool topEdgeLimit = ((currY == yMaxSize) &&
                        ((checkLeft && (currX < endX)) || (checkRight && (currX > endX))));

                    bool nextToEnd = (((currX == xMaxSize) && (currY == endY)) &&
                                       (checkUp || checkDown || checkLeft));

                    bool belowTheEnd = (((currY == yMaxSize) && (currX == endX)) &&
                                        (checkDown || checkLeft || checkRight));

                    bool outerEdgeLimit = (leftEdgeLimit   || rightEdgeLimit ||
                                           bottomEdgeLimit || topEdgeLimit);
                    #endregion

                    // remove options which deadend or increment available options
                    if (outerEdgeLimit || nextToEnd || belowTheEnd || pathSizeLimit || pathTooSmall || tooFar)
                        currTile.SetSides(currSide, SideLogic.stPath);                    
                    else if (currTile.NeighborLog(currSide) == SideLogic.altPath)
                        currTile.SetSides(currSide, SideLogic.fWall);
                    else
                    {                     
                        remainFaces.Add(currSide);
                        ++pOptions;
                    }

                }

                ++currSide;
            }
            #endregion

            // backtrack to previous tile if the tile has no options. 
            if (pOptions == 0)
            {
                Debug.Log("Backtracking from: " + currTile.fullPos.x + ", "+ currTile.fullPos.y);
                alTData ignoreTile = currTile;
                currTile = pathTiles[pathTiles.Count-1];

                Debug.Log("Backtracked from: "+ ignoreTile.fullPos.ToString()+ 
                            " to: " + currTile.fullPos.ToString());
                // remove data or make a wall of some sort.
                #region BLOCK DIRECTION CHOICE
                /*if (currTile.indir == nullInDir)
                    Debug.LogWarning("BorderSide would have a nullInDir input");*/
                
                currTile.SetSides(ignoreTile.BorderSide(currTile), SideLogic.stPath);
                currTile.outdir = nullOutDir;
                pathTiles.Remove(currTile);    // remove from pathTiles
                
                //ignoreTile.indir = nullInDir;  // remove indir
                ignoreTile.ResetTile(xMaxSize, yMaxSize, endX, endY);

                #endregion
            }
            else
            {
                #region PICK NEXT TILE
                // pick a random direction from options
                int dirChoice = remainFaces[Random.Range(0, remainFaces.Count)];

                alTData tempTile = currTile; // allows currTile to be updated
                alTData nextTile;
                
                //SideLogic[] tileFaces = currTile.CheckSides();  //tileFaces
                int sideNum = 0;
                // determine which direction was the chosen one and set it up as the next tile.
                foreach(SideLogic tileFace in tileFaces)
                {
                    if(dirChoice == sideNum)
                    {
                        // set outdir, solPath, nextTile, indir, and pathTile
                        nextTile = currTile.SetNextTile(dirChoice,SideLogic.solPath,SideLogic.solPath);
                        pathTiles.Add(currTile);
                        currTile = nextTile;
                    }                     
                    else if (tileFace == SideLogic.opnSide)
                            tempTile.SetSides(sideNum, SideLogic.altPath);

                    ++sideNum;
                }
                #endregion
                
            }
            //yield return null;
        }
        #endregion

        pathTiles.Add(endTile);
        Debug.Log("Solution Path has been generated");

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
        print(pathlist);

        yield return null;
    }

    public IEnumerator CreateBranches(List<alTData> pathTs, int branchDepth = brDepth)  //alTdata root = startTile
    {
        int currBranchDepth = 0;  // 
        bool needToPush = false;  // for when all branches end but grid not filled

        while((incompleteTiles.Count !=0))// && branchDepth < 6)
        {
            int solTileNum = 0;
            foreach(alTData pathT in pathTs)
            {
                alTData.BranchOpts allOpts = pathT.GetBrOpts(needToPush);
                // check if there are any open branch options or current branches for the pathTile
                List<int> pthBrOpts = allOpts.nwBrOpts;
                List<int> pthBrDirs = allOpts.currBrs;
                bool noBranchOpts = (pthBrOpts.Count < 1 && pthBrDirs.Count == 0);
                //pathT.ChkBrOptions(needToPush, out pthBrOpts, out pthBrDirs); //, out int brCount);


                // skip to next tile if no option to branch and no current active branches
                if(noBranchOpts)
                {
                    Debug.Log("CrBranches moving to next currTile from: " + pathT.fullPos.ToString());
                }
                else
                {
                    alTData rootT = pathT;
                    StartCoroutine(BuildBranch(rootT, pathT, branchDepth, currBranchDepth, needToPush, allOpts));
                }
                Debug.Log("CrBranches:foreach loop finished with solTile: " + solTileNum);
                ++solTileNum;
            }
            ++branchDepth;

            TilesFilled();
            Debug.Log("TilesFilled updated to be size: " + incompleteTiles.Count);
            Debug.Log("CrBranches after foreach loop solTile is: " + solTileNum);
            Debug.Log("CrBranches is at branchDepth: " + branchDepth);
            //yield return null;
        }
        Debug.Log("Branch Creation complete");
        yield return null;
    }

    // static

    public IEnumerator BuildBranch(alTData rootTile, alTData currTile, int totalDepth, 
                                   int currDepth, bool pushThru, alTData.BranchOpts rootOpts)    
    {
        Debug.Log("Starting BldBranch for currTile: " + currTile.fullPos.ToString());
        //int newDepth = currDepth +1; 
        bool skipBranching = false;
        //alTData brRoot = currTile;
        alTData.BranchOpts tempOpts = rootOpts;
        int whileCount = 0;
        while ((currDepth <= totalDepth) && !skipBranching)
        {
            #region BRANCH PREP AND OPTION STORAGE
            List<int> rtBrOpts = tempOpts.nwBrOpts; // optional directions
            List<int> rtBrDirs = tempOpts.currBrs;  // already branch directions
            bool noOptions = (rtBrOpts.Count < 1 && rtBrDirs.Count == 0);
            #endregion

            Debug.Log("at start of BldBranch while loop number: " + whileCount);
            alTData tempRtTile = currTile;

            if (noOptions)
            {
                // for closing out a branch that dead ends
                if (currTile.isInBranch && rootTile.isInBranch)
                {
                    
                    currTile.pathCompl = true; // consider stop options, potentially use push
                    skipBranching = true; //currTile = rootTile;
                    Debug.LogWarning("BldBranch (noptions, curr+root inBr) reached a deadend within a branch at currTile: "
                        + currTile.fullPos.ToString());
                }
                else
                {
                    Debug.LogWarning("BldBranch: should close this branch, as it has been completed. currTile's root is: " + rootTile.fullPos.ToString());
                    skipBranching = true;  // temporarty
                }


            }
            else if (rtBrDirs.Count > 0) //brCount > 0)
            {
                Debug.Log("BldBranch: the currTile is: " + currTile.fullPos.ToString() + "at depth " + currDepth);

                // traverse each branch and look for potential branching options
                int brDirCount = 0;
                int layerDepth = currDepth + 1;
                foreach (int rtBrDir in rtBrDirs)
                {

                    alTData tempNxtTile;
                    Debug.Log("BldBranch: about to get brDirCount of: " + brDirCount + " for brRoot to find the coords");
                    Debug.Log("BldBranch outbranches has a size of: " + tempRtTile.outBranches.Count);

                    Vector2Int gridCoord = currTile.outBranches[brDirCount];
                    tempNxtTile = fullGrid[gridCoord.x, gridCoord.y];
                    alTData.BranchOpts nextOpts = tempNxtTile.GetBrOpts(pushThru);

                    if (tempNxtTile.pathCompl && !tempRtTile.isSolution)
                    {
                        Debug.Log("BldBranch: Entering nested recursive call into layer: " + layerDepth);
                        Debug.Log("BldBranch: original root is: " + rootTile.fullPos.ToString() +
                            " current root is: " + tempRtTile.fullPos.ToString());
                        tempOpts = currTile.GetBrOpts(pushThru);
                        bool tempPush = true;  // pushing through if needed. 
                        StartCoroutine(BuildBranch(tempRtTile, currTile, totalDepth, layerDepth, tempPush, tempOpts));
                        Debug.Log("BldBranch: Exiting the recursive call");
                    }
                    else
                    {

                        currTile = tempNxtTile;
                        tempOpts = nextOpts;
                        Debug.Log("BldBranch: continue to traverse the branch as currTile: " + currTile.fullPos.ToString());
                    }


                    Debug.Log("inside BldBranches: rtBrDirs > 0, resultant currTile: " + currTile.fullPos.ToString());
                    ++brDirCount;
                }
            }
            else
            {
                // pick an option from openSides or to stop
                if (!pushThru) rtBrOpts.Add(noBranch);
                
                int dirPicked = rtBrOpts[Random.Range(0, rtBrOpts.Count)];

                if (dirPicked != noBranch)
                {
                    //List<alTData> currBranch = new();
                    //brRoot.branchElements.Add(currBranch);

                    if (!tempRtTile.isBranching)  //brRoot.isBranching)
                    {
                        tempRtTile.outBranches = new List<Vector2Int>();  //brRoot.outBranches = new();
                        tempRtTile.branchTiles = new();  //brRoot.branchTiles = new();

                        Debug.Log("BldBranch made first branch for currTile: " + tempRtTile.fullPos.ToString());

                        if (tempRtTile.isSolution || (tempRtTile.isInBranch && tempRtTile.outBranches.Count > 1))
                        {
                            currTile.isBranching = true; //brRoot.isBranching = true;
                            Debug.Log("BldBranch: currTile: " + currTile.fullPos.ToString() + " recieved isBranching");
                        }
                        
                        Debug.Log("BldBranch making branch");
                        currTile = tempRtTile.MakeBranch(dirPicked);
                        if (tempRtTile.isInBranch)
                            rootTile.branchTiles.Add(currTile);

                        tempOpts = currTile.GetBrOpts(pushThru);

                        PrintPath(rootTile.branchTiles);
                        ++currDepth;
                    }
                    else
                        Debug.LogWarning("BldBranch Picked a direction that was already branching at currTile: " + currTile.fullPos.ToString());

                    //currTile = brRoot.MakeBranch(dirPicked);//, totalDepth, currDepth);
                }
                else
                {
                    skipBranching = true;
                    if (currTile.isSolution)
                        Debug.Log("BldBranch:(currT solution) moving to next pathTile from currTile: " + currTile.fullPos.ToString());
                    else
                    {
                        Debug.LogWarning("BldBranch:(else) moving to next branchTile from currTile: " + currTile.fullPos.ToString());
                    }
                }

                

                //branchRoot.outBranches.Add(branchRoot.GetLogNeighbor())
            }

            //if(brRoot.isBranching || )
            //secondPass = true;
            Debug.Log("BldBranch end of while loop, curr count is" + whileCount);
            ++whileCount;

            //yield return null;
        }

        Debug.Log("Exiting BldBranch at layer " + currDepth + " and currTile: " +currTile.fullPos.ToString());
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
                Debug.Log("removing tile from incompleteTile list" + incompleteTile.currentPos.ToString() +
                    " aka " + incompleteTile.fullPos.ToString());

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
        Debug.Log("entering fullCleanup");
        // loop to fill grid
        for (int i = 0; i <= xMaxSize + 1; ++i)
        {
            for (int j = 0; j <= yMaxSize + 1; ++j)
            {
                fullGrid[i, j].PrepTile();
            }
        }
        Debug.Log("fullCleanUp complete");
    }


    public IEnumerator GenGridData()    
    {
        GenGrid();
        yield return null;  // update loading progress bar
        StartCoroutine(CreatePath());
        yield return null;  // update loading progress bar
        TrackIncompleteTiles();
        yield return null;  // update loading progress bar
        StartCoroutine(CreateBranches(pathTiles));  // incomplete
        FullCleanUp();
        DataToMaze.i.dataToMaze(alDataConverter.convertToTiledata(fullGrid));

    }

    
}
