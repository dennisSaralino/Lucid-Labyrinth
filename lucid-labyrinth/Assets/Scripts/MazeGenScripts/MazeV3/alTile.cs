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
}