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
    public int st2endDistLimit = 6;  // minimum limit of distance between the start and end tiles 
    public Vector2Int nullInDir;     // used to clear data when backtracking
    public Vector2Int nullOutDir;    // used to clear data when backtracking

    public const int brDepth = 3;            // default depth of making a branch before skipping to the next

    public void Awake()
    {
        if (thisGrid != null) { Destroy(gameObject); return; }
        thisGrid = this;
        StartCoroutine(GenGridData());
        
    }


    public void GenGrid()
    {
        fullGridSize = (xMaxSize + 2) * (yMaxSize + 2);
        fullGrid = new alTData[xMaxSize+2,yMaxSize+2];

        edgesSize = xMaxSize + yMaxSize;   // prep array size for zeroEdges and maxEdges
        zeroEdges = new alTData[edgesSize];
        maxEdges = new alTData[edgesSize];

        mazeTilesSize = xMaxSize * yMaxSize;
        mazeGrid = new alTData[xMaxSize,yMaxSize];

        int zEdge = 0;
        int mEdge = 0;

        // loop to fill grid
        for (int i = 0; i <= xMaxSize + 1; ++i)
        {
            for (int j = 0; j <= yMaxSize + 1; ++j)
            {
                // Ensure fullPos is set
                //fullGrid[i, j].fullPos.Set(i, j);
                fullGrid[i,j] = new alTData(new Vector2Int(i, j));
                alTData thisTile = new alTData(new Vector2Int(i,j));
                fullGrid[i, j] = thisTile;

                // create easy logic handles for edges
                bool leftEdge = (i < 1);
                bool rightEdge = (i > xMaxSize);
                bool bottomEdge = (j < 1);
                bool topEdge = (j > yMaxSize);
                bool outerEdges = (leftEdge || rightEdge || bottomEdge || topEdge);

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

        nullInDir = fullGrid[0, 0].fullPos;
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


        // looping solution path algorithm
        #region FIND SOLUTION PATH
        while (currTile != endTile)
        //while (currTile.outdir != endTile.fullPos)
        {

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
                    int strtX = startTile.fullPos.x;
                    int strtY = startTile.fullPos.y;
                    int endX  = endTile.fullPos.x;
                    int endY  = endTile.fullPos.y;

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
                    if (outerEdgeLimit || nextToEnd || belowTheEnd)
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
                //Debug.Log("Backtracking from: " + currTile.fullPos.x + ", "+ currTile.fullPos.y);
                alTData ignoreTile = currTile;
                currTile = fullGrid[ignoreTile.indir.x, ignoreTile.indir.y];
                Debug.Log("Backtracked from: "+ ignoreTile.fullPos.ToString()+ 
                            " to: " + currTile.fullPos.ToString());
                // remove data or make a wall of some sort.
                #region BLOCK DIRECTION CHOICE

                currTile.SetSides(ignoreTile.BorderSide(currTile), SideLogic.stPath);
                currTile.outdir = nullOutDir;
                pathTiles.Remove(currTile);    // remove from pathTiles
                
                ignoreTile.indir = nullInDir;  // remove indir
                ignoreTile.ResetTile(xMaxSize, yMaxSize);
                //if (ignoreTile.fullPos == currTile.GetLogNeighbor(Vector2Int.left).fullPos)

                #endregion
            }
            else
            {
                #region PICK NEXT TILE
                // pick a random direction from options
                int dirChoice = remainFaces[Random.Range(0, remainFaces.Count)];

                alTData tempTile = currTile; // allows currTile to be updated
                alTData nextTile;
                
                SideLogic[] currFaces = currTile.CheckSides();
                int sideNum = 0;
                // determine which direction was the chosen one and set it up as the next tile.
                foreach(SideLogic currFace in currFaces)
                {
                    if(dirChoice == sideNum)
                    {
                        // set outdir, solPath, nextTile, indir, and pathTile
                        nextTile = currTile.SetNextTile(dirChoice);
                        pathTiles.Add(currTile);
                        currTile = nextTile;
                    }                     
                    else if (currFace == SideLogic.opnSide)
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
            pathTile.isSolution = true;
            pathlist += pathTile.fullPos.ToString();
            //print("(" + pathTile.fullPos.x + ", " + pathTile.fullPos.y + "), ");
        }
        #endregion
        print(pathlist);
    }

    public void CreateBranches(List<alTData> pathTs, int branchDepth = brDepth)  //alTdata root = startTile
    {
        int currBranchDepth = 0;
        int noBranch = 4; // gives a none option to the logic
        foreach(alTData pathT in pathTs)
        {
            // prep and store branch options
            List<int> branchOptions = new();
            branchOptions.Add(noBranch);  // "none option"
 
            SideLogic[] branchDirs = pathT.CheckSides();
            int dirCount = 0;
            foreach(SideLogic branchDir in branchDirs)
            {
                if((branchDir == SideLogic.opnSide) || (branchDir == SideLogic.stPath)
                    || (branchDir == SideLogic.brPath))
                {
                    branchOptions.Add(dirCount);
                }

                ++dirCount;
            }

            // pick an option
            int dirPicked = branchOptions[Random.Range(0, dirCount)];

            alTData branchStep = pathT;

            if(dirPicked == noBranch)
            {
                foreach(SideLogic branchDir in branchDirs)
                {
                    if(branchDir == SideLogic.brPath)
                    {
                        CreateBranches(pathT.branch, branchDepth - currBranchDepth);
                    }
                }
            }

            foreach(int branchOption in branchOptions)
            {
                //if()
            }
            

        }

    }

    public IEnumerator GenGridData()    
    {
        GenGrid();
        yield return null;  // update loading progress bar
        CreatePath();
        yield return null;  // update loading progress bar
        //CreateBranches(pathTiles);  // incomplete
    }

    
}
