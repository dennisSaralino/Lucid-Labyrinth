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

    public alTData startTile;  
    public alTData endTile;
    public List<alTData> pathTiles;    // dynamically made list of tiles in the solutionPath
    public int st2endDistLimit;  // = 6;  // minimum limit of distance between the start and end tiles 
    public int minDistCheck = 2;     // used to set range of minPathL checking against dist from endTile
    public int minPathL;
    public int maxPathL;
    public int modPathDiv = 3;
    public int modPathMult = 2;
    public Vector2Int nullInDir;     // used to clear data when backtracking
    public Vector2Int nullOutDir;    // used to clear data when backtracking

    public List<alTData> incompleteTiles; 
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

                // create easy logic handles for edges
                bool leftEdge = (i < 1);
                bool rightEdge = (i > xMaxSize);
                bool bottomEdge = (j < 1);
                bool topEdge = (j > yMaxSize);
                bool outerEdges = (leftEdge || rightEdge || bottomEdge || topEdge);
                #endregion

                // store fullGrid values, noting outer edges to be walls and center open
                #region SET INITIAL TILE LOGIC STATES FOR fullGrid
                // Left
                if ((i == 1) || leftEdge || rightEdge)
                    fullGrid[i, j].leftLog = SideLogic.fWall; 
                else
                    fullGrid[i, j].leftLog = SideLogic.opnSide;
                
                // Right
                if ((i == xMaxSize) || rightEdge || leftEdge)
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

    public void CreatePath()    
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
            startTile.outdir = startTile.GetLogNeighbor(Vector2Int.right).fullPos;
            startTile.GetLogNeighbor(Vector2Int.right).leftLog = SideLogic.solPath;
            startTile.GetLogNeighbor(Vector2Int.right).indir = startTile.fullPos;
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

        st2endDistLimit = edgesSize / modPathDiv;
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

        minPathL = mazeTilesSize / modPathDiv;
        maxPathL = (mazeTilesSize * modPathMult / modPathDiv) + minDistCheck; // allows for the start and end tile to be in length
        #endregion


        // looping solution path algorithm
        #region FIND SOLUTION PATH
        while (currTile != endTile)
        //while (currTile.outdir != endTile.fullPos)
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
            /*sideLogic[] tileFaces = new sideLogic[4];
            tileFaces[0] = currTile.upLog;
            tileFaces[1] = currTile.downLog;
            tileFaces[2] = currTile.leftLog;
            tileFaces[3] = currTile.rightLog; */

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
                //currTile = fullGrid[ignoreTile.indir.x, ignoreTile.indir.y];
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
                        nextTile = currTile.SetNextTile(dirChoice,SideLogic.solPath);
                        pathTiles.Add(currTile);
                        currTile = nextTile;
                    }                     
                    else if (tileFace == SideLogic.opnSide)
                            tempTile.SetSides(sideNum, SideLogic.altPath);

                    ++sideNum;
                }
                #endregion
                
            }
        }
        #endregion

        pathTiles.Add(endTile);
        Debug.Log("Solution Path has been generated");

        string pathlist = "solution values: ";
        int currTileIndx = 0;
        #region PATH LOGIC CLEANUP
        foreach (alTData pathTile in pathTiles)
        {
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
            if(currTileIndx != 0)
                pathTile.indir = pathTiles[currTileIndx - 1].fullPos;

            pathTile.PrepTile();

            pathTile.isSolution = true;
            pathlist += pathTile.fullPos.ToString();
            //print("(" + pathTile.fullPos.x + ", " + pathTile.fullPos.y + "), ");

            ++currTileIndx;
        }
        #endregion
        print(pathlist);
    }

    public IEnumerator CreateBranches(List<alTData> pathTs, int branchDepth = brDepth)  //alTdata root = startTile
    {
        int currBranchDepth = 0;  // 
        bool needToPush = false;  // for when all branches end but grid not filled

        while(!TilesFilled() && branchDepth < 6)
        {
            foreach(alTData pathT in pathTs)
            {
                // check if there are any branches or open options
                List<int> branchOptions, branchDs;                //int brCount;
                pathT.ChkBrOptions(needToPush, out branchOptions, out branchDs, out int brCount);

                // skip to next tile 
                if(branchOptions.Count <= 1 && branchDs.Count == 0)
                {
                    Debug.Log("moving to next pathTile from: " + pathT.fullPos.ToString());
                }
                else
                {
                    alTData rootT = pathT;
                    StartCoroutine(BuildBranch(rootT, pathT, branchDepth, currBranchDepth, needToPush));
                }

            }
            ++branchDepth;

            yield return null;
        }
        Debug.Log("Branch Creation complete");
    }

    // static

    public IEnumerator BuildBranch(alTData rootTile, alTData currTile, int totalDepth, int currDepth, bool pushThru)    
    {

        //int newDepth = currDepth +1; 
        bool skipBranching = false;
        alTData brRoot = currTile;

        while ((currDepth <= totalDepth) && !skipBranching)
        {
            #region BRANCH PREP AND OPTION STORAGE
            List<int> branchOptions, branchDs;
            int brCount;
            currTile.ChkBrOptions(pushThru, out branchOptions, out branchDs, out brCount);
            #endregion

            alTData brRoot = currTile;

            if (branchOptions.Count <= 1 && branchDs.Count == 0)
            {
                if (currTile.isInBranch && rootTile.isInBranch)
                {
                    //currTile = rootTile;
                    Debug.Log("reached a deadend within a branch at tile: " + currTile.fullPos.ToString());
                }
                else
                    Debug.Log("should close this branch, as it has been completed. root is: " + rootTile.fullPos.ToString());

            }
            if (brCount > 0)
            {
                // traverse each branch and look for potential branching options
                int brDirCount = 0;
                foreach (int branchD in branchDs)
                {
                    int layerDepth = currDepth + 1;

                    Vector2Int gridCoord = brRoot.outBranches[brDirCount];
                    currTile = fullGrid[gridCoord.x, gridCoord.y];
                    Debug.Log("Entering nested recursive call into layer: " + layerDepth);
                    Debug.Log("original root is: " + rootTile.fullPos.ToString() + " current root is: " + brRoot.fullPos.ToString());
                    StartCoroutine(BuildBranch(brRoot, currTile, totalDepth, layerDepth, pushThru));
                    ++brDirCount;
                }
            }
            else
            {
                // pick an option 
                int dirPicked = branchOptions[Random.Range(0, branchOptions.Count)];

                if (dirPicked != noBranch)
                {

                    //List<alTData> currBranch = new();
                    //brRoot.branchElements.Add(currBranch);

                    if (!brRoot.isBranching)
                    {
                        brRoot.outBranches = new();
                        brRoot.outBranchCompl = new();
                        brRoot.branchTiles = new();

                        Debug.Log("made first branch for tile: " + currTile.fullPos.ToString());

                        if (currTile.isSolution || (currTile.isInBranch && currTile.outBranches.Count > 1))
                        {
                            brRoot.isBranching = true;
                            Debug.Log("tile: " + currTile.fullPos.ToString() + " recieved isBranching");
                        }

                    }

                    Debug.Log("making branch");
                    currTile = brRoot.MakeBranch(dirPicked);//, totalDepth, currDepth);
                }
                else
                    skipBranching = true;
                

                //branchRoot.outBranches.Add(branchRoot.GetLogNeighbor())


            }

            //if(brRoot.isBranching || )
            //secondPass = true;
            yield return null;
        }


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

    public bool TilesFilled()
    {
        bool complete = false;

        // go through incompleteTiles
        foreach (alTData incompleteTile in incompleteTiles)
        {
            // check if each tile is either in the solution or part of a branch
            if (incompleteTile.isSolution || incompleteTile.isBranching || 
                incompleteTile.isInBranch || incompleteTile.isDeadEnd)
            {
                incompleteTiles.Remove(incompleteTile);
            }
        }

        if (incompleteTiles.Count == 0)
            complete = true;

        return complete;
    }

    public IEnumerator GenGridData()    
    {
        GenGrid();
        yield return null;  // update loading progress bar
        CreatePath();
        yield return null;  // update loading progress bar
        TrackIncompleteTiles();
        yield return null;  // update loading progress bar
        //StartCoroutine(CreateBranches(pathTiles));  // incomplete
    }

    
}
